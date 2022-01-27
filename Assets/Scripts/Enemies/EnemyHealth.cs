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
	public UnityEvent onWallBump;

	public float currentHealth { get; set; }
	private bool isDead;

	private Tween scaleDownTween;

	private ShootManager sm;
	private Collider2D myColl;
	private Rigidbody2D rb;

	private void Awake()
	{
		currentHealth = maxHealth;
		// Collider2D should be on the same gameobject as EnemyHealth. If not, we might have to change stuff, not sure yet
		myColl = GetComponent<Collider2D>();
#if UNITY_EDITOR
		if (myColl == null)
			Debug.LogError("Collider and EnemyHealth not on the same object: Object " + gameObject.name);
#endif
		rb = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		sm = ShootManager.instance;
		onWallBump.AddListener(() => SoundManager.instance.PlaySound(SoundManager.Sound.EnemyHitWall));
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		// Wall layer
		if (collision.gameObject.layer == 9)
		{
			// Bounce against wall
			Vector2 bounceForce = collision.GetContact(0).normal * 70;
			rb.AddForce(bounceForce);

			onWallBump.Invoke();
		}
	}

	public void TakeDamage(float toTake, bool canCrit = false)
	{
		// Modify damage to take based on upgrades and such things
		if (isDead)
			return;

		if (Mathf.Approximately(currentHealth, maxHealth) || currentHealth >= maxHealth)
			toTake += sm.cloakedDaggerDM * sm.damage;

		float critChance = sm.critChance;

		if (IsBelowHealthThreshold(sm.scissorsHealthPercent))
			critChance += sm.scissorsCritChanceAddition;

		Color damageTextColor = Color.white;
		if (canCrit && MyRandom.RollProbability(critChance))
		{
			sm.OnCrit(transform, myColl);
			toTake = toTake + (toTake * sm.critDamage);
			damageTextColor = GameAssets.instance.critColor;
			ObjectPooler.instance.CreateHitParticles(GameAssets.instance.critColor, transform.position);
		}

		toTake *= sm.damageMultiplier;

		// Actually take the damage here
		currentHealth -= toTake;
		onHit.Invoke();

		// Create damage number text
		Vector2 topOfEnemy = GetTopOfEnemy();
		string damageAmount = Mathf.RoundToInt(toTake * 10f).ToString();
		ObjectPooler.instance.CreateTextObject(topOfEnemy, Quaternion.identity, damageTextColor, 6.5f, damageAmount);		

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

	public void SetCurrHealth()
	{
		currentHealth = maxHealth;
	}

	public Vector2 GetTopOfEnemy()
	{
		return (Vector2)myColl.bounds.center + new Vector2(0, myColl.bounds.extents.y + 0.35f);
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
		toScale.localScale = new Vector3(currScale + 0.35f, currScale + 0.35f, 1);
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
