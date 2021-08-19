using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CollideWithEnemy : MonoBehaviour
{
	public int damage;

	private HashSet<GameObject> hitSet = new HashSet<GameObject>();

	private void OnEnable()
	{
		hitSet = new HashSet<GameObject>();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// Enemy layer
		if (collision.gameObject.layer == 7)
		{
			// Do not collide with enemy projectiles
			if (collision.gameObject.CompareTag("Projectile"))
				return;

			if (!hitSet.Contains(collision.gameObject))
			{
				collision.GetComponent<EnemyHealth>().TakeDamage(damage);

				ObjectPooler.instance.CreateHitParticles(Color.white, transform.position);
				ObjectPooler.instance.CreateCircleHitEffect(Color.white, transform.position, 1f);

				hitSet.Add(collision.gameObject);
			}
		}
	}
}
