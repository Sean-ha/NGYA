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
	public bool isBoss;
	public UnityEvent onHit;
	public UnityEvent onDeath;

	public float currentHealth { get; set; }
	private bool isDead;

	private Tween scaleDownTween;

	private void Awake()
	{
		currentHealth = maxHealth;

		if (UpgradesManager.instance.obtainedUpgrades.Contains(Upgrade.Shrapnel))
		{
			onDeath.AddListener(CreateShrapnel);
		}
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
			if (!isBoss)
				SpawnManager.instance.EnemyIsKilled();

			if (UpgradesManager.instance)

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

	public void PlayDeathHitCircleEffect(float size)
	{
		ObjectPooler.instance.CreateCircleHitEffect(hitParticlesColor, transform.position, size);
	}

	public void PlaySound(int sound)
	{
		SoundManager.instance.PlaySound((SoundManager.Sound)sound);
	}

	public void CreateShrapnel()
	{
		float startDangle = Random.Range(0f, 90f);
		for (int i = 0; i < 4; i++)
		{
			float currDangle = startDangle + 90 * i;
			GameObject proj = ObjectPooler.instance.Create(Tag.PlayerProjectile, transform.position, Quaternion.AngleAxis(currDangle, Vector3.forward));
			proj.GetComponent<BasicProjectile>().SetProjectile(15, currDangle, ShootManager.instance.damage, 1, 15f);
		}
	}
}
