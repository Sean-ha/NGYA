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

	// DEBUG: DELETE TESTVAL LATER, ONLY FOR DEBUGGING
	public GameObject Create(Tag tag, Vector3 position, Quaternion rotation, bool TESTVAL = true)
	{
		// TODO: Remove
#if UNITY_EDITOR
		if (tag == Tag.PlayerProjectile)
		{
			if (TESTVAL)
				Debug.LogError("ERROR: DON'T CREATE PLAYER PROJECTILE FROM HERE!!!");
		}
		else if (tag == Tag.ParticleHitEffects)
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

	public GameObject CreatePlayerProjectile(Vector3 position, float dAngle, float shotSpeed, float damage, 
		int numberOfTargets, float distance, bool canCrit, bool applyOnHitEffects)
	{
		GameObject proj = Create(Tag.PlayerProjectile, position, Quaternion.AngleAxis(dAngle, Vector3.forward), TESTVAL: false);
		proj.GetComponent<BasicProjectile>().SetProjectile(shotSpeed, dAngle, damage, numberOfTargets, distance, canCrit, applyOnHitEffects);

		return proj;
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

	public GameObject CreateCircleHitEffect(Color color, Vector3 position, float startSize, float timeToShrink = 0.2f, bool large = false)
	{
		GameObject objectToSpawn;
		if (large)
		{
			objectToSpawn = poolDictionary[Tag.CircleHitEffectBig].Dequeue();
			poolDictionary[Tag.CircleHitEffectBig].Enqueue(objectToSpawn);
		}
		else
		{
			objectToSpawn = poolDictionary[Tag.CircleHitEffect].Dequeue();
			poolDictionary[Tag.CircleHitEffect].Enqueue(objectToSpawn);
		}

		objectToSpawn.SetActive(true);

		CircleHitEffect hitEffect = objectToSpawn.GetComponent<CircleHitEffect>();
		hitEffect.SetHitEffect(color, startSize, timeToShrink);

		objectToSpawn.transform.position = position;

		return objectToSpawn;
	}

	public GameObject CreateUITextObject(Vector3 position, Quaternion rotation, Color color, float fontSize, string text)
	{
		GameObject created = Create(Tag.TextObject, position, rotation);
		TextMeshPro textMesh = created.GetComponent<TextMeshPro>();
		textMesh.color = color;
		textMesh.fontSize = fontSize;
		textMesh.text = text;

		return created;
	}

	public GameObject CreateTextObject(Vector3 position, Quaternion rotation, Color color, float fontSize, string text)
	{
		GameObject created = Create(Tag.GameTextObject, position, rotation);
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

	public GameObject CreateTendril(Vector2 startPos, Vector2 endPos)
	{
		GameObject created = Create(Tag.Tendril, new Vector2(0, 0), Quaternion.identity);

		Vector2 direction = endPos - startPos;
		Vector2 perp = Vector3.Cross(direction, Vector3.back);

		// How far from the start the midpoint should be
		float dist = Random.Range(4f, 7f);
		bool reverse = Random.value > 0.5f;
		if (reverse)
			dist *= -1;

		float xOffset = Random.Range(-4f, 4f);
		
		Vector2 midpoint = startPos + perp.normalized * dist + direction.normalized * xOffset;

		created.GetComponent<BezierCurve>().SetBezierCurve(startPos, midpoint, endPos, 20);

		return created;
	}

	// startDirection = bullet's default starting velocity. timeToActivate = time until bullet's homing capabilites are enabled
	// Banana parameter: false for regular bullet, true for banana bullet
	public GameObject CreateHomingProjectile(Vector2 position, Quaternion rotation, float damage, bool canCrit, Vector2 startDirection, float speed = 12f, 
		float timeToActivate = 0.07f, bool banana = false)
	{
		GameObject proj;
		if (banana)
			proj = Create(Tag.BananaProjectile, position, rotation);
		else
			proj = Create(Tag.HomingProjectile, position, rotation);

		CollideWithEnemy collider = proj.GetComponent<CollideWithEnemy>();
		collider.damage = damage;
		collider.canCrit = canCrit;
		proj.GetComponent<HomingProjectile>().ActivateBullet(startDirection, timeToActivate, speed);

		return proj;
	}

	public GameObject CreateElectricity(Vector2 start, Vector2 end, float time = 0.2f, float startWidth = 0.5f)
	{
		GameObject electricity = Create(Tag.Electricity, Vector3.zero, Quaternion.identity);
		ElectricityEffect effect = electricity.GetComponent<ElectricityEffect>();

		effect.CreateElectricEffect(start, end, time: time, startWidth: startWidth);

		return electricity;
	}

	// isPlayerProjectile: true if player projectile. False if enemy projectile
	public GameObject CreatePlayerLaserProjectile(Vector2 position, Quaternion rotation, float damage, bool canCrit, bool onHitEffects, 
		float circleSize = 1.75f, float beamSize = 0.5f)
	{
		GameObject laser = Create(Tag.PlayerLaserProjectile, position, rotation);

		laser.GetComponent<LaserProjectile>().ActivateLaser(damage, canCrit, onHitEffects, circleEffectScale: circleSize, beamSize: beamSize);

		return laser;
	}

	public GameObject CreateEnemyLaserProjectile()
	{
		// Do something here when if or when needed.
		// Note: GetComponent<LaserProjectile().SetProjectile ...
		return null;
	}

	public GameObject CreateExpandingExplosion(Vector2 position, Color color, float radius)
	{
		GameObject explosion = Create(Tag.ExpandingExplosion, position, Quaternion.identity);
		explosion.GetComponent<ExpandingExplosion>().SetExplosion(color, radius);

		return explosion;
	}
}