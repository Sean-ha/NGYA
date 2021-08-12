using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

// AI for charging enemies
public class ChargeMovement : MonoBehaviour
{
	[Tooltip("How fast the enemy will charge")]
	public float chargeSpeed;
	[Tooltip("How long the enemy will be stunned upon hitting a wall")]
	public float wallStunDuration;
	[Tooltip("How long the enemy will be looking at the player, preparing to charge")]
	public float waitTime;

	public UnityEvent onWallBump;

	private Rigidbody2D rb;

	private bool preparingToCharge = true;
	private bool isCharging;
	private bool needsToResetRotation;
	// In radians
	private float angle;

	private float currentTime;

	private Transform player;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		player = GameAssets.instance.player.transform;
	}

	private void Update()
	{
		if (preparingToCharge)
		{
			Vector2 diff = (Vector2)player.position - (Vector2)transform.position;

			angle = Mathf.Atan2(diff.y, diff.x);
			currentTime += Time.deltaTime;

			if (currentTime >= waitTime)
			{
				currentTime = 0;
				preparingToCharge = false;
				InitiateCharge();
			}
		}
	}

	private void FixedUpdate()
	{
		if (preparingToCharge)
		{
			rb.MoveRotation(Quaternion.Euler(0, 0, Mathf.Rad2Deg * angle));
		}
		if (isCharging)
		{
			Vector2 force = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * chargeSpeed;
			rb.AddForce(force);
		}
		if (needsToResetRotation)
		{
			rb.rotation %= 360;
			needsToResetRotation = false;
		}
	}

	private void InitiateCharge()
	{
		rb.freezeRotation = true;
		isCharging = true;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (isCharging)
		{
			// Wall layer
			if (collision.gameObject.layer == 9)
			{
				// End charge
				isCharging = false;
				rb.freezeRotation = false;

				// Bounce against wall
				Vector2 bounceForce = collision.GetContact(0).normal * 100;
				rb.AddForce(bounceForce);

				rb.DORotate(rb.rotation + Random.Range(180, 540f), wallStunDuration).SetUpdate(UpdateType.Fixed).SetEase(Ease.OutCubic).onComplete += () =>
				{
					// Finish spinning, getting ready to charge player
					StartCoroutine(StartNewCharge());
				};

				onWallBump.Invoke();
			}
		}
	}

	private IEnumerator StartNewCharge()
	{
		needsToResetRotation = true;

		yield return new WaitForSeconds(0.5f);

		preparingToCharge = true;
	}
}
