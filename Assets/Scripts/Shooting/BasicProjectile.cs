using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : Projectile
{
	[Tooltip("How hard to push the enemy hit by this projectile")]
	public float pushForce;

	public GameObject temp;

	private Rigidbody2D rb;

	private float speed;
	// In radians
	private float angle;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		rb.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
	}

	public void SetProjectile(float speed, float angle)
	{
		this.speed = speed;
		this.angle = Mathf.Deg2Rad * angle;

		StartCoroutine(Shrink());
	}

	private IEnumerator Shrink()
	{
		for (int i = 0; i < 14; i++)
			yield return null;

		LeanTween.value(gameObject, speed, 0, 0.1f).setOnUpdate((float val) =>
		{
			speed = val;
		}).setOnComplete(() =>
		{
			LeanTween.scale(gameObject, new Vector3(0, 1, 1), 0.1f).setOnComplete(() => Destroy(transform.parent.gameObject));
		});
		
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// Enemy layer
		if (collision.gameObject.layer == 7)
		{
			collision.GetComponent<EnemyHealth>().TakeDamage(damage);

			Vector2 push = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * pushForce;
			collision.GetComponent<Rigidbody2D>().AddForce(push);

			LeanTween.cancel(gameObject);
			Destroy(transform.parent.gameObject);
		}
	}
}
