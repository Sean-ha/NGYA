using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

// Handles enemy health and collisions
public class EnemyHealth : MonoBehaviour
{
	public Color hitParticlesColor = Color.red;
	public float maxHealth;
	public UnityEvent onHit;
	public UnityEvent onDeath;

	private float currentHealth;
	private bool isDead;

	private Tween scaleDownTween;

	private void Awake()
	{
		currentHealth = maxHealth;
	}

	public void TakeDamage(float toTake)
	{
		if (isDead)
			return;

		currentHealth -= toTake;
		onHit.Invoke();

		if (currentHealth <= 0)
		{
			isDead = true;
			SpawnManager.instance.EnemyIsKilled();
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
		ObjectPooler.instance.CreateHitParticles(hitParticlesColor, transform.position);
	}

	public void PlayOnHitScale(Transform toScale)
	{
		scaleDownTween.Complete();

		float currScale = toScale.localScale.x;
		toScale.localScale = new Vector3(currScale + 0.5f, currScale + 0.5f, 1);
		scaleDownTween = toScale.DOScale(new Vector3(currScale, currScale, 1), 0.2f);
	}

	public void PlayDeathHitCircleEffect()
	{
		ObjectPooler.instance.CreateCircleHitEffect(hitParticlesColor, transform.position, 1.5f);
	}
}
