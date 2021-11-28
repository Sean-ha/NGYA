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

	private ShootManager sm;
	private Collider2D myColl;

	private void Awake()
	{
		currentHealth = maxHealth;
		// Collider2D should be on the same gameobject as EnemyHealth. If not, we might have to change stuff, not sure yet
		myColl = GetComponent<Collider2D>();
		if (myColl == null)
			Debug.LogError("Collider and EnemyHealth not on the same object: Object " + gameObject.name);
	}

	private void Start()
	{
		sm = ShootManager.instance;
	}

	public void TakeDamage(float toTake, bool canCrit = false)
	{
		if (isDead)
			return;

		if (currentHealth == maxHealth)
			toTake += sm.cloakedDaggerDM * sm.damage;

		float critChance = sm.critChance;

		if (IsBelowHealthThreshold(sm.scissorsHealthPercent))
			critChance += sm.scissorsCritChanceAddition;

		if (canCrit && MyRandom.RollProbability(critChance))
		{
			sm.OnCrit(transform, myColl);
			toTake *= sm.critDamage;
		}

		currentHealth -= toTake;
		onHit.Invoke();

		if (currentHealth <= 0)
		{
			isDead = true;

			if (!isBoss)
			{
				SpawnManager.instance.EnemyIsKilled(transform);
			}
			sm.OnProjectileKillEnemy(transform);

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

	// Given a percentage, return whether or not this enemy's health is below that percentage
	private bool IsBelowHealthThreshold(float percentage)
	{
		return currentHealth <= percentage * maxHealth;
	}
}
