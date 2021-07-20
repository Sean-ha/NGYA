using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
		// TODO: Move this stuff somewhere else probably
		Application.targetFrameRate = 60;
		DOTween.Init(useSafeMode:false).SetCapacity(500, 50);
		DOTween.defaultEaseType = Ease.Linear;
		DOTween.useSafeMode = true;

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

		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			Time.timeScale = 5;
		}
		else if (Input.GetKeyUp(KeyCode.LeftShift))
		{
			Time.timeScale = 1;
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
				float toTake = collision.GetComponent<Damager>().damage;
				healthSystem.TakeDamage(toTake);
				TakeDamageEffects();
				CameraShake.instance.ShakeCamera(0.2f, 0.3f);

				// Get knocked back
				Vector2 diff = (transform.position - collision.transform.position).normalized;
				Vector2 destination = (Vector2)transform.position + (diff * knockBackForce);

				rb.DOMove(destination, 0.3f).SetEase(Ease.OutQuad).SetUpdate(UpdateType.Fixed);

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

	/// <summary>
	/// Get the position the player will be in 'time' seconds given their current movement
	/// </summary>
	/// <param name="time">Number of seconds</param>
	/// <returns></returns>
	public Vector2 GetForwardPosition(float time)
	{
		Vector2 vel = rb.velocity * time;
		return (Vector2)transform.position + vel;
	}

	public Vector2 GetBackwardPosition(float time)
	{
		Vector2 vel = rb.velocity * time;
		return (Vector2)transform.position - vel;
	}
}
