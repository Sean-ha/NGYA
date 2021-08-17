using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
	[System.Serializable]
	public class Pool
	{
		public Tag tag;
		public GameObject prefab;
		public int size;
	}

	public static ObjectPooler instance;

	// To be set in inspector: the list of Objects to pool and their sizes (and a string to tag them by)
	public List<Pool> pools;

	public Dictionary<Tag, Queue<GameObject>> poolDictionary;

	private void Awake()
	{
		instance = this;

		poolDictionary = new Dictionary<Tag, Queue<GameObject>>();

		foreach (Pool pool in pools)
		{
			Queue<GameObject> objectPool = new Queue<GameObject>();

			for (int i = 0; i < pool.size; i++)
			{
				GameObject obj = Instantiate(pool.prefab);
				obj.SetActive(false);
				objectPool.Enqueue(obj);
			}
			poolDictionary.Add(pool.tag, objectPool);
		}
	}

	public GameObject Create(Tag tag, Vector3 position, Quaternion rotation)
	{
		// TODO: Remove
#if UNITY_EDITOR
		if (tag == Tag.ParticleHitEffects)
		{
			Debug.LogError("ERROR: DON'T CREATE PARTICLE HIT EFFECTS FROM HERE!!!");
		}
		else if (tag == Tag.CircleHitEffect)
		{
			Debug.LogError("ERROR: DON'T CREATE CIRCLE HIT EFFECTS FROM HERE!!!!");
		}
#endif

		if (!poolDictionary.ContainsKey(tag))
		{
			return null;
		}
		GameObject objectToSpawn = poolDictionary[tag].Dequeue();

		objectToSpawn.transform.position = position;
		objectToSpawn.transform.rotation = rotation;
		objectToSpawn.SetActive(true);

		poolDictionary[tag].Enqueue(objectToSpawn);

		return objectToSpawn;
	}

	public GameObject CreateHitParticles(Color color, Vector3 position)
	{
		GameObject objectToSpawn = poolDictionary[Tag.ParticleHitEffects].Dequeue();

		objectToSpawn.SetActive(true);
		ParticleSystem.MainModule main = objectToSpawn.GetComponent<ParticleSystem>().main;
		main.startColor = color;

		objectToSpawn.transform.position = position;
		objectToSpawn.transform.rotation = Quaternion.identity;

		poolDictionary[Tag.ParticleHitEffects].Enqueue(objectToSpawn);

		return objectToSpawn;
	}

	public GameObject CreateCircleHitEffect(Color color, Vector3 position, float startSize, float timeToShrink = 0.2f)
	{
		GameObject objectToSpawn = poolDictionary[Tag.CircleHitEffect].Dequeue();

		objectToSpawn.SetActive(true);

		CircleHitEffect hitEffect = objectToSpawn.GetComponent<CircleHitEffect>();
		hitEffect.SetHitEffect(color, startSize, timeToShrink);

		objectToSpawn.transform.position = position;

		poolDictionary[Tag.CircleHitEffect].Enqueue(objectToSpawn);

		return objectToSpawn;
	}

	public GameObject CreateTextObject(Vector3 position, Quaternion rotation, Color color, float fontSize, string text)
	{
		GameObject created = Create(Tag.TextObject, position, rotation);
		TextMeshPro textMesh = created.GetComponent<TextMeshPro>();
		textMesh.color = color;
		textMesh.fontSize = fontSize;
		textMesh.text = text;

		return created;
	}

	public GameObject CreateLaserSight(Vector3 position, Quaternion rotation, Color color, float timeToDisable)
	{
		GameObject created = Create(Tag.LaserSight, position, rotation);
		created.GetComponentInChildren<SpriteRenderer>().color = color;
		created.GetComponent<SetInactive>().DisableSelfAfter(timeToDisable);

		return created;
	}
}