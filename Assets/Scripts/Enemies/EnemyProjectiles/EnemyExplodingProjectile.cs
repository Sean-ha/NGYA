using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyExplodingProjectile : MonoBehaviour
{
	[SerializeField] private GameObject explosion;

	private Rigidbody2D rb;

	private float dangle;
	private float damage;
	private float distance;
	private Vector2 destination;

	private bool canHit = true;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	public void SetProjectile(float speed, float dangle, float damage, float distance)
	{
		canHit = true;
		this.dangle = Mathf.Deg2Rad * dangle;
		this.damage = damage;
		this.distance = distance;

		destination = (Vector2)transform.position + (new Vector2(Mathf.Cos(this.dangle), Mathf.Sin(this.dangle)) * distance);
		float timeToReachFinal = distance / speed;

		rb.DOMove(destination, timeToReachFinal).SetUpdate(UpdateType.Fixed).onComplete += () =>
		{
			CreateExplosion();
			Destroy(gameObject);
		};
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// Player layer or wall layer (same behavior either way)
		if (canHit && collision.gameObject.layer == 8 || collision.gameObject.layer == 9)
		{
			CreateExplosion();
			canHit = false;
			Destroy(gameObject);
		}
	}

	private void CreateExplosion()
	{
		CameraShake.instance.ShakeCamera(0.3f, 0.4f);
		SoundManager.instance.PlaySound(SoundManager.Sound.Explosion3);
		// GameObject expl = Instantiate(explosion, transform.position, Quaternion.identity);
		// expl.GetComponent<Explosion>().ActivateExplosion(damage);
	}
}
