using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles enemy health and collisions
public class EnemyHealth : MonoBehaviour
{
	public float maxHealth;

	private ParticleSystem hitParticles;
	private float currentHealth;

	private void Awake()
	{
		currentHealth = maxHealth;
		hitParticles = transform.GetChild(0).GetComponent<ParticleSystem>();
	}

	public void TakeDamage(float toTake)
	{
		currentHealth -= toTake;
		hitParticles.Play();

		if (currentHealth <= 0)
		{
			// die
		}
	}
}
