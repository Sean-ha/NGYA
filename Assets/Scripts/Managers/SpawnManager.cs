using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using NaughtyAttributes;
using System.Linq;

public class SpawnManager : MonoBehaviour
{
	public static SpawnManager instance;

	// Debug purposes only, REMOVE
	public bool endlessSpawn;
	[EnableIf("endlessSpawn")]
	public List<GameObject> possibleSpawnsList;
	[EnableIf("endlessSpawn")]
	public int maxEnemiesAtOnce;
	public bool enableDefaultSpawn;
	public int currStageDebug;

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

	// Set containing enemies that are currently alive and enabled.
	// IMPORTANT NOTE: Contains the Transform of the EnemyHealth component of each enemy, NOT THE OUTERMOST PARENT!
	public HashSet<Transform> enemySet { get; set; } = new HashSet<Transform>();

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		currentStage = currStageDebug;
		if (enableDefaultSpawn)
			StartCoroutine(StartCurrentStage());
		else if (endlessSpawn)
			StartCoroutine(EndlessMode());

#if UNITY_EDITOR
		// DEBUG: Test for nulls in enemySet. Only for testing purposes; only in editor
		InvokeRepeating(nameof(TestNulls), 1f, 1f);
#endif
	}

	// DEBUG: remove in final version
	private IEnumerator EndlessMode()
	{
		while (true)
		{
			maxEnemiesAtOnce = 10 + ExpManager.instance.level * 4;
			while (enemySet.Count >= maxEnemiesAtOnce)
				yield return new WaitForSeconds(0.15f);

			int randIndex = Random.Range(0, possibleSpawnsList.Count);
			List<GameObject> spawns = new List<GameObject>();
			spawns.Add(possibleSpawnsList[randIndex]);

			List<Vector2> positions = PoissonDiscSampling.GeneratePoints(2.5f, topRight.position - bottomLeft.position, spawns.Count);

			for (int i = 0; i < spawns.Count; i++)
			{
				Vector2 pos = positions[i] + (Vector2)bottomLeft.position;

				List<Transform> enemies = CreateSpawn(spawns[i], pos);

				GameObject exclaPoint = Instantiate(exclamationPoint, pos, Quaternion.identity);
				RectTransform epRt = exclaPoint.GetComponent<RectTransform>();

				epRt.localScale = new Vector3(1.75f, 1.75f, 1);
				epRt.DOScale(new Vector3(0.75f, 0.75f, 1), 0.2f).SetEase(Ease.OutCubic).onComplete += () =>
				{
					epRt.DOScale(new Vector3(1, 1, 1), 0.15f).SetEase(Ease.OutCubic).onComplete += () => Destroy(exclaPoint, 1.15f);
				};

				// Circle bar behavior
				SoundManager.instance.PlaySound(SoundManager.Sound.AlertLow, false);
				GameObject circleBar = Instantiate(circularProgressBar, pos, Quaternion.identity, worldCanvas);
				CircleProgressBar circleBarComponent = circleBar.GetComponent<CircleProgressBar>();

				circleBarComponent.SetColor(Color.red);

				circleBarComponent.BeginBar(1.5f, () =>
				{
					// SoundManager.instance.PlaySound(SoundManager.Sound.EnemySpawn, false);
					foreach (Transform enemy in enemies)
					{
						enemy.gameObject.SetActive(true);
						ObjectPooler.instance.CreateHitParticles(Color.red, enemy.position);
						ObjectPooler.instance.CreateCircleHitEffect(Color.red, enemy.position, 1.5f);

						enemySet.Add(enemy.GetComponentInChildren<EnemyHealth>().transform);
					}
				});
			}

			yield return new WaitForSeconds(2f - (ExpManager.instance.level * 0.08f));
		}
	}

	private IEnumerator StartCurrentStage()
	{
		currentWave = 0;
		enemiesInCurrentStage = 0;
		playerKillCountInCurrentStage = 0;

		stageText.text = "";
		stageText.gameObject.SetActive(true);

		yield return new WaitForSeconds(1f);

		string toType = "" + (currentStage + 1);

		foreach (char letter in toType)
		{
			stageText.text += letter;
			SoundManager.instance.PlaySound(SoundManager.Sound.ClickySound);

			if (letter != ' ')
				yield return new WaitForSeconds(0.1f);
		}

		yield return new WaitForSeconds(1f);

		/*
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
		*/

		DOTween.To(() => stageText.color, (Color val) => stageText.color = val, new Color(1, 1, 1, 0), 0.4f).OnComplete(() => {
			stageText.gameObject.SetActive(false);
			stageText.color = Color.white;
		});

		yield return new WaitForSeconds(0.75f);
		
		SpawnWave();
	}

	// Spawns the currentWave of the currentStage
	private void SpawnWave()
	{
		List<WaveAndTime> possibleWaves = spawnsList[currentStage].waves;

		List<List<GameObject>> spawns = new List<List<GameObject>>();

		// Collect all waves chained together by a zero timeUntilNextWave
		WaveAndTime chosenWave;
		do
		{
			chosenWave = possibleWaves[currentWave];
			spawns.Add(chosenWave.spawns);
			currentWave++;
		} while (Mathf.Approximately(chosenWave.timeUntilNextWave, 0f) && currentWave < possibleWaves.Count);
		currentWave--;

		List<Vector2> positions = PoissonDiscSampling.GeneratePoints(5f, topRight.position - bottomLeft.position, spawns.Count);

		for (int i = 0; i < spawns.Count; i++)
		{
			enemiesInCurrentStage += GetEnemiesInCurrentStage(spawns[i]);

			Vector2 pos = positions[i] + (Vector2)bottomLeft.position;

			GameObject exclaPoint = Instantiate(exclamationPoint, pos, Quaternion.Euler(0, 0, Random.Range(-15f, 15f)));
			RectTransform epRt = exclaPoint.GetComponent<RectTransform>();
			SoundManager.instance.PlaySound(SoundManager.Sound.WaterSplash2);

			int a = i;
			epRt.localScale = new Vector3(0f, 0f, 1);
			epRt.DOScale(new Vector3(2f, 2f, 1), 0.12f).SetEase(Ease.InOutQuad).onComplete += () =>
			{
				epRt.DOScale(new Vector3(1.6f, 1.6f, 1), 0.05f).SetEase(Ease.InOutQuad).onComplete += () => 
				{
					DOVirtual.DelayedCall(1.2f, () =>
					{
						StartCoroutine(SpawnEnemyList(pos, spawns[a]));
						Destroy(exclaPoint);
					}, ignoreTimeScale: false);
				};
			};
			

			/*
			List<Transform> enemies = CreateSpawn(spawns[i], pos);

			GameObject exclaPoint = Instantiate(exclamationPoint, pos, Quaternion.identity);
			RectTransform epRt = exclaPoint.GetComponent<RectTransform>();

			epRt.localScale = new Vector3(1.75f, 1.75f, 1);
			epRt.DOScale(new Vector3(0.75f, 0.75f, 1), 0.2f).SetEase(Ease.OutCubic).onComplete += () =>
			{
				epRt.DOScale(new Vector3(1, 1, 1), 0.15f).SetEase(Ease.OutCubic).onComplete += () => Destroy(exclaPoint, 1.15f);
			};

			// Circle bar behavior
			SoundManager.instance.PlaySound(SoundManager.Sound.AlertLow, false);
			GameObject circleBar = Instantiate(circularProgressBar, pos, Quaternion.identity, worldCanvas);
			CircleProgressBar circleBarComponent = circleBar.GetComponent<CircleProgressBar>();

			circleBarComponent.SetColor(Color.red);

			circleBarComponent.BeginBar(1.5f, () =>
			{
				SoundManager.instance.PlaySound(SoundManager.Sound.EnemySpawn, false);
				foreach (Transform enemy in enemies)
				{
					enemy.gameObject.SetActive(true);
					ObjectPooler.instance.CreateHitParticles(Color.red, enemy.position);
					ObjectPooler.instance.CreateCircleHitEffect(Color.red, enemy.position, 1.5f);

					enemySet.Add(enemy.GetComponentInChildren<EnemyHealth>().transform);
				}
			});
			*/
		}

		// Don't start Coroutine for next wave if last wave has been reached.
		if (currentWave != possibleWaves.Count - 1)
			currentWaitingToSpawnCR = StartCoroutine(WaitingToSpawnNextWave());
	}

	// Number of EnemyHealths that player must kill in this stage.
	public int GetEnemiesInCurrentStage(List<GameObject> spawnList)
	{
		int counter = 0;
		foreach (GameObject spawn in spawnList)
		{
			if (spawn.GetComponentInChildren<SplitterEnemy>() != null)
				counter += 5;
			else
				counter++;
		}

		return counter;
	}

	// Given a list of enemies and a center position, spawn them all around that center position 1 by 1
	private IEnumerator SpawnEnemyList(Vector2 spawnPos, List<GameObject> enemies)
	{
		foreach (GameObject enemy in enemies)
		{
			Vector2 randomizedPos = spawnPos + Random.insideUnitCircle * 1.5f;

			GameObject createdEnemy = Instantiate(enemy, randomizedPos, Quaternion.identity);
			enemySet.Add(createdEnemy.GetComponentInChildren<EnemyHealth>().transform);

			ObjectPooler.instance.CreateHitParticles(Color.red, randomizedPos);
			ObjectPooler.instance.CreateCircleHitEffect(Color.red, randomizedPos, 1.5f);
			SoundManager.instance.PlaySound(SoundManager.Sound.WaterSplash1, false);

			yield return new WaitForSeconds(0.05f);
		}
	}

	private IEnumerator WaitingToSpawnNextWave()
	{
		yield return new WaitForSeconds(spawnsList[currentStage].waves[currentWave].timeUntilNextWave);

		currentWave++;
		SpawnWave();
	}

	// DEPRECATED -- replaced by the coroutine SpawnEnemyList() 
	// Instantiates a spawn at a position. Properly increments enemiesInCurrentStage. Returns a list of transforms of the enemy objects that were created.
	// All enemies are set as inactive. They must be enabled elsewhere (i.e. after circle load completes)
	private List<Transform> CreateSpawn(GameObject spawn, Vector2 position)
	{
		List<Transform> createdEnemies = new List<Transform>();

		Transform holder = Instantiate(spawn, position, Quaternion.identity).transform;

		// Unparent enemies and destroy holder
		/*
		while (holder.childCount > 0)
		{
			Transform child = holder.GetChild(0);
			child.parent = null;
			child.gameObject.SetActive(false);
			enemiesInCurrentStage++;
			createdEnemies.Add(child);
		}

		Destroy(holder.gameObject);
		*/

		return createdEnemies;
	}

	public void EnemyIsKilled(Transform enemy)
	{
		playerKillCountInCurrentStage++;
		enemySet.Remove(enemy);

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
				Invoke(nameof(SpawnWave), 1f);
			}
		}
	}

	public Transform GetRandomEnemy()
	{
		if (enemySet.Count == 0)
			return null;

		int index = Random.Range(0, enemySet.Count);
		return enemySet.ElementAt(index);
	}

	// Gets n random enemies
	public HashSet<Transform> GetRandomEnemies(int amount)
	{
		int numberToReturn = Mathf.Min(amount, enemySet.Count);

		List<int> indices = MyRandom.GenerateRandomValues(numberToReturn, 0, enemySet.Count);
		List<Transform> enemyList = enemySet.ToList();
		HashSet<Transform> selected = new HashSet<Transform>();

		for (int i = 0; i < indices.Count; i++)
		{
			selected.Add(enemyList[indices[i]]);
		}
		return selected;
	}

	private void TestNulls()
	{
		int i = 0;
		foreach (Transform t in enemySet)
		{
			if (t == null)
				i++;
		}
		if (i != 0)
		{
			throw new System.Exception(i + " NULLS FOUND IN enemySet in SpawnManager!! (Should be 0)");
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
	public List<GameObject> spawns;
	[Tooltip("The time until the next wave in the list will be spawned")]
	public float timeUntilNextWave;
}
