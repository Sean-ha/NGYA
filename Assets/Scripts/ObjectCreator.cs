using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Provides methods for creating un-pooled objects. Also provides references to prefabs if necessary (probably not)
public class ObjectCreator : MonoBehaviour
{
	public static ObjectCreator instance;

	public GameObject explosionParticles;
	public GameObject circleColliderInstance;

	private void Awake()
	{
		instance = this;
	}

	public GameObject CreateExplosionParticles(Vector2 position, Color color, float radius)
	{
		GameObject created = Instantiate(explosionParticles, position, Quaternion.identity);

		ParticleSystem ps = created.GetComponent<ParticleSystem>();

		ParticleSystem.MainModule main = ps.main;
		main.startColor = color;

		ParticleSystem.ShapeModule shape = ps.shape;
		shape.radius = radius;

		return created;
	}

	public GameObject CreateEnemyCircleCollider(Vector2 position, float radius, float damage, float duration)
	{
		GameObject created = Instantiate(circleColliderInstance, position, Quaternion.identity);

		created.GetComponent<CircleColliderInstance>().Setup(radius, damage, duration, destroyAfter: true);

		return created;
	}
}