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
		float randomDangle = Random.Range(0, 359);
		GameObject expl = Instantiate(explosion, transform.position, Quaternion.Euler(0, 0, randomDangle));
		Transform circle = expl.transform.GetChild(0);
		SpriteRenderer circleSR = circle.GetComponent<SpriteRenderer>();

		CollideWithEnemy damager = expl.GetComponent<CollideWithEnemy>();
		damager.damage = damage;
		damager.canCrit = canCrit;

		// expl.transform.DORotate(new Vector3(0, 0, -300f), 1f, RotateMode.FastBeyond360).SetEase(Ease.OutSine).OnComplete(() => Destroy(expl));
		DOVirtual.DelayedCall(0.1f, () => {
			expl.GetComponent<Collider2D>().enabled = false;
			circleSR.color = new Color(1, 1, 1, 0.1f);
			circleSR.sortingLayerID = 0;
			expl.transform.DOScale(new Vector3(0.97f, 0.97f, 1f), 0.46f);
			HelperFunctions.BlinkSpriteRenderer(expl.GetComponent<SpriteRenderer>(), 4, 0.06f, 0.06f);
			HelperFunctions.BlinkSpriteRenderer(circleSR, 4, 0.06f, 0.06f);
			Destroy(expl, 0.48f);
		}, ignoreTimeScale: false);

	}
}
