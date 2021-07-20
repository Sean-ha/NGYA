using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SpawnManager : MonoBehaviour
{
	public static SpawnManager instance;

	public Transform bottomLeft;
	public Transform topRight;

	public GameObject TESTENEMY;

	public Transform worldCanvas;
	public GameObject circularProgressBar;
	public GameObject exclamationPoint;

	public List<Stage> spawnsList;

	private int currentStage;

	private int enemiesInCurrentWave;
	private int playerKillCountInCurrentWave;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		SpawnEnemies();
	}

	/*
	private List<Vector2> GetSpawnPositions(int numberOfPositions)
	{
		List<Vector2> positions = new List<Vector2>();

		for (int i = 0; i < numberOfPositions; i++)
		{
			float x = Random.Range(topLeft.position.x, bottomRight.position.x);
			float y = Random.Range(bottomRight.position.y, topLeft.position.y);

			Vector2 current = new Vector2(x, y);

			foreach (Vector2 pos in positions)
			{
				int rerollCount = 0;
				while (Vector2.Distance(pos, current) < MIN_SPAWN_DISTANCE && rerollCount < 10)
				{
					x = Random.Range(topLeft.position.x, bottomRight.position.x);
					y = Random.Range(bottomRight.position.y, topLeft.position.y);

					current = new Vector2(x, y);
					rerollCount++;
				}
				if (rerollCount == 10)
				{
					print("dang");
				}
			}

			positions.Add(current);
		}

		return positions;
	}
	*/

	private void SpawnEnemies()
	{
		enemiesInCurrentWave = 0;
		playerKillCountInCurrentWave = 0;

		List<Wave> possibleWaves = spawnsList[currentStage].possibleWaves;
		int randomWave = Random.Range(0, possibleWaves.Count);
		Wave chosenWave = possibleWaves[randomWave];

		int numberOfPositions = chosenWave.spawns.Count;

		List<Vector2> positions = PoissonDiscSampling.GeneratePoints(2.5f, topRight.position - bottomLeft.position, numberOfPositions);

		for (int i = 0; i < numberOfPositions; i++)
		{
			Vector2 pos = positions[i] + (Vector2)bottomLeft.position;

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
				// Enemy creation behavior
				CreateSpawn(chosenWave.spawns[tempIndex], pos);
			});
		}

		/*
		foreach (Vector2 pos in positions)
		{
			// Exclamation point behavior
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
			circleBarComponent.BeginBar(1.5f, () =>
			{
				// Enemy creation behavior
				Instantiate(TESTENEMY, pos, Quaternion.identity);
				ObjectPooler.instance.CreateHitParticles(Color.red, pos);
				ObjectPooler.instance.CreateCircleHitEffect(Color.red, pos, 1.5f);
			});
		}
		*/
	}

	private void CreateSpawn(GameObject spawn, Vector2 position)
	{
		Transform holder = Instantiate(spawn, position, Quaternion.identity).transform;

		// Unparent enemies and destroy holder
		while (holder.childCount > 0)
		{
			Transform child = holder.GetChild(0);
			child.parent = null;
			ObjectPooler.instance.CreateHitParticles(Color.red, child.position);
			ObjectPooler.instance.CreateCircleHitEffect(Color.red, child.position, 1.5f);
			enemiesInCurrentWave++;
		}

		Destroy(holder.gameObject);
	}

	public void EnemyIsKilled()
	{
		playerKillCountInCurrentWave++;
		print("New: " + playerKillCountInCurrentWave);
		if (playerKillCountInCurrentWave == enemiesInCurrentWave)
		{
			SpawnEnemies();
		}
	}
}

[System.Serializable]
public class Stage
{
	public List<Wave> possibleWaves;
}
