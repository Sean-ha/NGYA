using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BasicProjectile : Projectile
{
	[Tooltip("How hard to push the enemy hit by this projectile")]
	public float pushForce;

	private Rigidbody2D rb;
	private Transform projectileSpriteChild;

	private Vector2 destination;
	private Vector2 startPosition;
	// In radians
	private float angle;
	private bool enableOnHitEffects;

	private int hitCount;
	private HashSet<GameObject> hitSet = new HashSet<GameObject>();

	private ShootManager sm;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		projectileSpriteChild = transform.GetChild(0);
	}

	private void Start()
	{
		sm = ShootManager.instance;
	}

	private void OnEnable()
	{
		CancelTweens();
		hitSet.Clear();
		hitCount = 0;
		projectileSpriteChild.localScale = new Vector3(1, 1, 1);
	}

	private void OnDisable()
	{
		CancelTweens();
	}

	/// <summary>
	/// Sets the projectile's fields and allows it to begin its travel
	/// </summary>
	/// <param name="angle">Angle in degrees</param>
	public void SetProjectile(float speed, float angle, float damage, int numberOfTargets, float distance, bool enableOnHitEffects = true)
	{
		this.angle = Mathf.Deg2Rad * angle;
		this.damage = damage;
		this.numberOfTargets = numberOfTargets;
		this.distance = distance;
		this.enableOnHitEffects = enableOnHitEffects;
		startPosition = transform.position;

		float firstDistance = distance - 1;
		Vector2 firstDestination = (Vector2)transform.position + (new Vector2(Mathf.Cos(this.angle), Mathf.Sin(this.angle)) * firstDistance);
		float timeToReachFirstDistance = firstDistance / speed;

		destination = (Vector2)transform.position + (new Vector2(Mathf.Cos(this.angle), Mathf.Sin(this.angle)) * distance);
		float timeToReachFinal = distance / speed;

		rb.DOMove(destination, timeToReachFinal).SetUpdate(UpdateType.Fixed).onComplete += () =>
		{
			projectileSpriteChild.DOScaleX(0, 0.1f).onComplete += () => gameObject.SetActive(false);
		};
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// Enemy layer
		if (collision.gameObject.layer == 7)
		{
			// Do not collide with enemy projectiles
			if (collision.gameObject.CompareTag("Projectile"))
				return;

			if (hitCount < numberOfTargets && !hitSet.Contains(collision.gameObject))
			{
				float adjustedDamage = damage;

				/*
				if (UpgradesManager.instance.obtainedUpgrades.Contains(Upgrade.Snipe))
				{
					float distance = Vector2.Distance(startPosition, transform.position);
					float multiplier = distance / 3f;
					if (multiplier >= 2.5f) multiplier = 2.5f;
					if (multiplier <= 0.25f) multiplier = 0.25f;
					adjustedDamage *= multiplier;
				}
				*/

				if (enableOnHitEffects)
					sm.OnProjectileHitEnemy(transform, collision.transform);				

				collision.GetComponent<EnemyHealth>().TakeDamage(adjustedDamage);

				Vector2 push = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * pushForce;
				collision.GetComponent<Rigidbody2D>().AddForce(push);

				ObjectPooler.instance.CreateHitParticles(Color.white, transform.position);
				ObjectPooler.instance.CreateCircleHitEffect(Color.white, transform.position, 1f);

				hitSet.Add(collision.gameObject);
				hitCount++;

				// Disable projectile; it has hit the maximum number of enemies
				if (hitCount >= numberOfTargets)
				{
					CancelTweens();

					transform.gameObject.SetActive(false);
				}
			}
		}
		// Wall layer
		else if (collision.gameObject.layer == 9)
		{
			Vector2 closestPoint = collision.ClosestPoint(transform.position);

			ObjectPooler.instance.CreateHitParticles(Color.white, closestPoint);
			ObjectPooler.instance.CreateCircleHitEffect(Color.white, closestPoint, 1f);
			SoundManager.instance.PlaySound(SoundManager.Sound.EnemyHit);

			CancelTweens();
			transform.gameObject.SetActive(false);
		}
	}

	private void CancelTweens()
	{
		rb.DOKill();
		projectileSpriteChild.DOKill();
	}
}
