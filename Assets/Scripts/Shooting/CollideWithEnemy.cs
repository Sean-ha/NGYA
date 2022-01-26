using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class CollideWithEnemy : Damager
{
	public bool canCrit;
	[Tooltip("Disable this object after it collides with a single enemy")]
	public bool disableOnHit;
	[Tooltip("Create a CircleHitEffect + HitParticles when collision occurs")]
	public bool createParticlesOnHit = true;

	public UnityEvent onCollide;

	private HashSet<GameObject> hitSet = new HashSet<GameObject>();

	private void OnEnable()
	{
		ClearHitSet();
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
				onCollide.Invoke();
				collision.GetComponent<EnemyHealth>().TakeDamage(damage, canCrit: canCrit);

				if (createParticlesOnHit)
				{
					ObjectPooler.instance.CreateHitParticles(Color.white, collision.transform.position);
					ObjectPooler.instance.CreateCircleHitEffect(Color.white, collision.transform.position, 1f);
				}

				hitSet.Add(collision.gameObject);
				if (disableOnHit)
					gameObject.SetActive(false);
			}
		}
	}

	public void ClearHitSet()
	{
		hitSet.Clear();
	}

	public void PlaySound(int sound)
	{
		SoundManager.instance.PlaySound((SoundManager.Sound)sound);
	}
}
