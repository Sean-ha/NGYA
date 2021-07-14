using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Handles enemy health and collisions
public class EnemyHealth : MonoBehaviour
{
	public Color hitParticlesColor;
	public float maxHealth;
	public UnityEvent onHit;
	public UnityEvent onDeath;

	private float currentHealth;

	private void Awake()
	{
		currentHealth = maxHealth;
	}

	public void TakeDamage(float toTake)
	{
		currentHealth -= toTake;
		onHit.Invoke();

		if (currentHealth <= 0)
		{
			onDeath.Invoke();
		}
	}

	public void DestroySelf()
	{
		Destroy(gameObject);
	}

	public void DestroyParent()
	{
		Destroy(transform.parent.gameObject);
	}

	public void PlayHitParticles()
	{
		ObjectPooler.instance.CreateHitParticles(hitParticlesColor, transform.position, Quaternion.identity);
	}
}
