using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles enemy health and collisions
public class EnemyHealth : MonoBehaviour
{
	public float maxHealth;

	private float currentHealth;

	private void Awake()
	{
		currentHealth = maxHealth;
	}

	public void TakeDamage(float toTake)
	{
		currentHealth -= toTake;
		transform.GetChild(0).GetComponent<ParticleSystem>().Play();

		if (currentHealth <= 0)
		{
			// die
		}
	}
}
