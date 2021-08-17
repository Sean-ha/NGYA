using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BasicEnemyProjectile : EnemyProjectile
{
	private Rigidbody2D rb;
	private Transform projectileSpriteChild;
	// In radians
	private float angle;

	private Damager damager;

	private Color projectileColor;
	private Vector3 originalSpriteSize;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		projectileSpriteChild = transform.GetChild(0);
		damager = GetComponent<Damager>();

		projectileColor = projectileSpriteChild.GetComponent<SpriteRenderer>().color;
		originalSpriteSize = projectileSpriteChild.localScale;
	}

	private void OnEnable()
	{
		rb.DOKill();
		projectileSpriteChild.localScale = originalSpriteSize;
	}

	public override void SetProjectile(float speed, float angle, float damage, float distance)
	{
		this.angle = Mathf.Deg2Rad * angle;
		this.damage = damage;
		this.distance = distance;
		damager.damage = damage;

		Vector2 destination = (Vector2)transform.position + (new Vector2(Mathf.Cos(this.angle), Mathf.Sin(this.angle)) * distance);

		float timeToReach = distance / speed;
		rb.DOMove(destination, timeToReach).SetUpdate(UpdateType.Fixed).onComplete += () =>
		{
			projectileSpriteChild.DOScaleX(0, 0.1f).onComplete += () => gameObject.SetActive(false);
		};
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// Wall layer
		if (collision.gameObject.layer == 9)
		{
			Vector2 closestPoint = collision.ClosestPoint(transform.position);
			ObjectPooler.instance.CreateHitParticles(projectileColor, closestPoint);
			ObjectPooler.instance.CreateCircleHitEffect(projectileColor, closestPoint, 1f);

			rb.DOKill();
			transform.gameObject.SetActive(false);
		}
	}
}
