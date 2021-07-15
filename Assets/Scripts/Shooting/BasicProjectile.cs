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
	// In radians
	private float angle;
	private bool isMoving;
	private float timeToReach;

	private int hitCount;
	private HashSet<GameObject> hitSet = new HashSet<GameObject>();

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		projectileSpriteChild = transform.GetChild(0);
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
		isMoving = false;
	}

	/*
	private void FixedUpdate()
	{
		if (isMoving)
		{
			Vector2 newPos = Vector2.Lerp(transform.position, destination, timeToReach);
			rb.MovePosition(newPos);
			timeToReach -= Time.fixedDeltaTime;

			if (timeToReach <= 0)
			{
				LeanTween.scale(projectileSpriteChild.gameObject, new Vector3(0, 1, 1), 0.1f).setOnComplete(() => transform.gameObject.SetActive(false));
			}
		}
	}
	*/

	/// <summary>
	/// Sets the projectile's fields and allows it to begin its travel
	/// </summary>
	/// <param name="angle">Angle in degrees</param>
	public void SetProjectile(float speed, float angle, float damage, int numberOfTargets, float distance)
	{
		this.angle = Mathf.Deg2Rad * angle;
		this.damage = damage;
		this.numberOfTargets = numberOfTargets;
		this.distance = distance;

		destination = (Vector2)transform.position + (new Vector2(Mathf.Cos(this.angle), Mathf.Sin(this.angle)) * distance);
		timeToReach = distance / speed;
		// isMoving = true;
		
		rb.DOMove(destination, timeToReach).SetEase(Ease.OutQuad).SetUpdate(UpdateType.Fixed).onComplete += () =>
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
				collision.GetComponent<EnemyHealth>().TakeDamage(damage);

				Vector2 push = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * pushForce;
				collision.GetComponent<Rigidbody2D>().AddForce(push);

				ObjectPooler.instance.CreateHitParticles(Color.white, transform.position, Quaternion.identity);
				ObjectPooler.instance.Create(Tag.CircleHitEffect, transform.position, Quaternion.identity);

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
	}

	private void CancelTweens()
	{
		DOTween.Kill(rb);
		DOTween.Kill(projectileSpriteChild);
	}
}
