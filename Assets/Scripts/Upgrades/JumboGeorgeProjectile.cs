using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumboGeorgeProjectile : MonoBehaviour
{
	public GameObject explosion;

	private Rigidbody2D rb;

	private float angle;
	private float damage;
	private float distance;
	private bool canCrit;
	private bool enableOnHitEffects;
	private Vector2 destination;

	private bool canHit = true;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	public void SetProjectile(float speed, float angle, float damage, float distance, bool canCrit, bool enableOnHitEffects)
	{
		canHit = true;
		this.angle = Mathf.Deg2Rad * angle;
		this.damage = damage;
		this.distance = distance;
		this.canCrit = canCrit;
		this.enableOnHitEffects = enableOnHitEffects;

		destination = (Vector2)transform.position + (new Vector2(Mathf.Cos(this.angle), Mathf.Sin(this.angle)) * distance);
		float timeToReachFinal = distance / speed;

		rb.DOMove(destination, timeToReachFinal).SetUpdate(UpdateType.Fixed).onComplete += () =>
		{
			CreateExplosion();
			Destroy(gameObject);
		};
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// Enemy layer or wall layer (same behavior either way)
		if (canHit && collision.gameObject.layer == 7 || collision.gameObject.layer == 9)
		{
			// Do not collide with enemy projectiles
			if (collision.gameObject.CompareTag("Projectile"))
				return;

			CreateExplosion();
			canHit = false;
			Destroy(gameObject);
		}
	}

	private void CreateExplosion()
	{
		CameraShake.instance.ShakeCamera(0.3f, 0.4f);
		SoundManager.instance.PlaySound(SoundManager.Sound.Explosion3);
		GameObject expl = Instantiate(explosion, transform.position, Quaternion.identity);
		expl.GetComponent<Explosion>().ActivateExplosion(damage, canCrit);
	}
}
