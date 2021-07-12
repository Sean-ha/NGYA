using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float moveSpeed;

	private HealthSystem healthSystem;
	private Rigidbody2D rb;
	private SpriteRenderer sr;

	private float horizontal;
	private float vertical;

	private float knockBackForce = 1f;

	// Duration of invincibility in seconds
	private float invincibilityDuration = 1f;
	// Duration of current invincibility. <=0 means player can be hit.
	private float currentInvincibility;

	private void Awake()
	{
		// TODO: Move this somewhere else probably
		Application.targetFrameRate = 60;

		rb = GetComponent<Rigidbody2D>();
		sr = GetComponent<SpriteRenderer>();
	}

	private void Start()
	{
		healthSystem = HealthSystem.instance;
	}

	private void Update()
	{
		horizontal = Input.GetAxis("Horizontal");
		vertical = Input.GetAxis("Vertical");

		if (currentInvincibility > 0)
		{
			currentInvincibility -= Time.deltaTime;
		}
	}

	private void FixedUpdate()
	{
		rb.velocity = new Vector2(horizontal, vertical) * moveSpeed;
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		OnTriggerStay2D(collision.collider);
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.gameObject.layer == 7)
		{
			if (currentInvincibility <= 0)
			{
				healthSystem.TakeDamage(10);
				TakeDamageEffects();
				CameraShake.instance.ShakeCamera(0.2f, 0.3f);

				// Get knocked back
				Vector2 diff = (transform.position - collision.transform.position).normalized;
				Vector2 destination = (Vector2)transform.position + (diff * knockBackForce);

				LeanTween.value(gameObject, (Vector2)transform.position, destination, 0.3f).setEaseOutQuad().setOnUpdateVector2((Vector2 val) =>
				{
					rb.MovePosition(val);
				});
				currentInvincibility = invincibilityDuration;
			}
		}
	}

	// Sprite blinking effect after taking damage
	public void TakeDamageEffects()
	{
		StartCoroutine(BlinkSprite());
		// StartCoroutine(ChromaticAberration());
	}

	private IEnumerator BlinkSprite()
	{
		for (int i = 0; i < 5; i++)
		{
			sr.enabled = false;
			yield return new WaitForSeconds(0.1f);
			sr.enabled = true;
			yield return new WaitForSeconds(0.1f);
		}
	}

	private IEnumerator ChromaticAberration()
	{
		sr.sharedMaterial.SetFloat("_OffsetBlueX", -.1f);
		sr.sharedMaterial.SetFloat("_OffsetRedX", .1f);
		yield return new WaitForSeconds(0.2f);

		LeanTween.value(gameObject, -.1f, 0, 0.2f).setOnUpdate((float val) =>
		{
			sr.sharedMaterial.SetFloat("_OffsetBlueX", val);
		});
		LeanTween.value(gameObject, .1f, 0, 0.2f).setOnUpdate((float val) =>
		{
			sr.sharedMaterial.SetFloat("_OffsetRedX", val);
		});
	}
}
