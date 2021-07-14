using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyProjectile : Projectile
{
	private Rigidbody2D rb;
	private Transform projectileSpriteChild;
	// In radians
	private float angle;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		projectileSpriteChild = transform.GetChild(0);
	}

	private void OnEnable()
	{
		LeanTween.cancel(gameObject);
		projectileSpriteChild.localScale = new Vector3(1, 1, 1);
	}

	public void SetProjectile(float speed, float angle, float damage, float distance)
	{
		this.angle = Mathf.Deg2Rad * angle;
		this.damage = damage;
		this.distance = distance;

		Vector2 destination = (Vector2)transform.position + (new Vector2(Mathf.Cos(this.angle), Mathf.Sin(this.angle)) * distance);
		LeanTween.value(gameObject, (Vector2)transform.position, destination, distance / speed).setEaseOutQuad().setOnUpdate((Vector2 val) =>
		{
			rb.MovePosition(val);
		}).setOnComplete(() =>
		{
			LeanTween.scale(projectileSpriteChild.gameObject, new Vector3(0, 1, 1), 0.1f).setOnComplete(() => transform.gameObject.SetActive(false));
		});
	}
}
