using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BasicEnemyProjectile : Projectile
{
	private Rigidbody2D rb;
	private Transform projectileSpriteChild;
	// In radians
	private float angle;

	private Damager damager;

	private Color projectileColor;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		projectileSpriteChild = transform.GetChild(0);
		damager = GetComponent<Damager>();

		projectileColor = transform.GetComponentInChildren<SpriteRenderer>().color;
	}

	private void OnEnable()
	{
		rb.DOKill();
		projectileSpriteChild.localScale = new Vector3(1, 1, 1);
	}

	public void SetProjectile(float speed, float angle, float damage, float distance)
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
		if (collision.gameObject.layer == 9)
		{
			ObjectPooler.instance.CreateHitParticles(projectileColor, collision.ClosestPoint(transform.position));
			ObjectPooler.instance.CreateCircleHitEffect(projectileColor, collision.ClosestPoint(transform.position), 1f);

			rb.DOKill();
			transform.gameObject.SetActive(false);
		}
	}
}
