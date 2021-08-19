using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using NaughtyAttributes;

public class SpawnManager : MonoBehaviour
{
	public static SpawnManager instance;

	// Debug purposes only, REMOVE
	public bool enableSpawn;

	public Transform bottomLeft;
	public Transform topRight;

	public Transform worldCanvas;
	public GameObject circularProgressBar;
	public GameObject exclamationPoint;

	public TextMeshPro stageText;

	public List<Stage> spawnsList;

	private int currentStage;
	private int currentWave;

	private int enemiesInCurrentStage;
	private int playerKillCountInCurrentStage;

	private Coroutine currentWaitingToSpawnCR;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		if (enableSpawn)
			StartCoroutine(StartCurrentStage());
	}

	private IEnumerator StartCurrentStage()
	{
		currentWave = 0;
		enemiesInCurrentStage = 0;
		playerKillCountInCurrentStage = 0;

		stageText.text = "";
		stageText.gameObject.SetActive(true);

		yield return new WaitForSeconds(1f);

		string toType = "stage " + (currentStage + 1);

		foreach (char letter in toType)
		{
			stageText.text += letter;
			SoundManager.instance.PlaySound(SoundManager.Sound.ClickySound);

			if (letter != ' ')
				yield return new WaitForSeconds(0.1f);
		}

		yield return new WaitForSeconds(1f);

		foreach (TMP_CharacterInfo charInfo in stageText.textInfo.characterInfo)
		{
			if (!charInfo.isVisible)
				continue;

			Vector2 localCenter = new Vector2((charInfo.topRight.x + charInfo.bottomLeft.x) / 2, (charInfo.topRight.y + charInfo.bottomLeft.y) / 2);

			Vector2 worldPos = (Vector2)stageText.rectTransform.position + localCenter;

			ObjectPooler.instance.CreateHitParticles(Color.white, worldPos);
			ObjectPooler.instance.CreateCircleHitEffect(Color.white, worldPos, 2);
		}
		SoundManager.instance.PlaySound(SoundManager.Sound.EnemyDeath);
		stageText.gameObject.SetActive(false);

		yield return new WaitForSeconds(1f);
		
		SpawnWave();
	}

	// Spawns the currentWave of the currentStage
	private void SpawnWave()
	{
		List<WaveAndTime> possibleWaves = spawnsList[currentStage].waves;

		WaveAndTime chosenWave = possibleWaves[currentWave];

		int numberOfPositions = chosenWave.wave.spawns.Count;

		List<Vector2> positions = PoissonDiscSampling.GeneratePoints(2.5f, topRight.position - bottomLeft.position, numberOfPositions);

		for (int i = 0; i < numberOfPositions; i++)
		{
			Vector2 pos = positions[i] + (Vector2)bottomLeft.position;

			List<Transform> enemies = CreateSpawn(chosenWave.wave.spawns[i], pos);

			GameObject exclaPoint = Instantiate(exclamationPoint, pos, Quaternion.identity);
			RectTransform epRt = exclaPoint.GetComponent<RectTransform>();

			epRt.localScale = new Vector3(1.75f, 1.75f, 1);
			epRt.DOScale(new Vector3(0.75f, 0.75f, 1), 0.2f).SetEase(Ease.OutCubic).onComplete += () =>
			{
				epRt.DOScale(new Vector3(1, 1, 1), 0.15f).SetEase(Ease.OutCubic).onComplete += () => Destroy(exclaPoint, 1.15f);
			};

			// Circle bar behavior
			GameObject circleBar = Instantiate(circularProgressBar, pos, Quaternion.identity, worldCanvas);
			CircleProgressBar circleBarComponent = circleBar.GetComponent<CircleProgressBar>();

			circleBarComponent.SetColor(Color.red);

			// Without this, we get an off by 1 error
			int tempIndex = i;
			circleBarComponent.BeginBar(1.5f, () =>
			{
				foreach(Transform enemy in enemies)
				{
					enemy.gameObject.SetActive(true);
					ObjectPooler.instance.CreateHitParticles(Color.red, enemy.position);
					ObjectPooler.instance.CreateCircleHitEffect(Color.red, enemy.position, 1.5f);
				}
			});
		}

		// Don't start Coroutine for next wave if last wave has been reached.
		if (currentWave != possibleWaves.Count - 1)
			currentWaitingToSpawnCR = StartCoroutine(WaitingToSpawnNextWave());
	}

	private IEnumerator WaitingToSpawnNextWave()
	{
		yield return new WaitForSeconds(spawnsList[currentStage].waves[currentWave].timeUntilNextWave);

		currentWave++;
		SpawnWave();
	}

	// Instantiates a spawn at a position. Properly increments enemiesInCurrentStage. Returns a list of transforms of the enemy objects that were created.
	// All enemies are set as inactive. They must be enabled elsewhere (i.e. after circle load completes)
	private List<Transform> CreateSpawn(GameObject spawn, Vector2 position)
	{
		List<Transform> createdEnemies = new List<Transform>(); ;

		Transform holder = Instantiate(spawn, position, Quaternion.identity).transform;

		// Unparent enemies and destroy holder
		while (holder.childCount > 0)
		{
			Transform child = holder.GetChild(0);
			child.parent = null;
			child.gameObject.SetActive(false);
			enemiesInCurrentStage++;
			createdEnemies.Add(child);
		}

		Destroy(holder.gameObject);

		return createdEnemies;
	}

	public void EnemyIsKilled()
	{
		playerKillCountInCurrentStage++;

		if (playerKillCountInCurrentStage == enemiesInCurrentStage)
		{
			if (currentWaitingToSpawnCR != null)
				StopCoroutine(currentWaitingToSpawnCR);


			// All enemies in final wave were defeated, move on to next stage.
			if (currentWave >= spawnsList[currentStage].waves.Count - 1)
			{
				currentStage += 1;
				StartCoroutine(StartCurrentStage());
			}
			else
			{
				currentWave += 1;
				Invoke(nameof(SpawnWave), 0.75f);
			}
		}
	}
}

[System.Serializable]
public class Stage
{
	public List<WaveAndTime> waves;
}

[System.Serializable]
public class WaveAndTime
{
	[Expandable]
	public Wave wave;
	[Tooltip("The time until the next wave in the list will be spawned")]
	public float timeUntilNextWave;
}
