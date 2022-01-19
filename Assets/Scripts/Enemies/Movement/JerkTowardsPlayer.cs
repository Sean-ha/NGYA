using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class JerkTowardsPlayer : MonoBehaviour
{
	public float forcePerJerk;
	public float timePerJerk;
	[Tooltip("Enemy will move towards player if distance between them is less than this value")]
	public float maxDistance;
	public UnityEvent onJerk;

	private PlayerController player;
	private Rigidbody2D rb;

	private float currTime;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		player = PlayerController.instance;
	}

	private void FixedUpdate()
	{
		currTime += Time.fixedDeltaTime;

		if (currTime >= timePerJerk)
		{
			currTime = 0;
			onJerk.Invoke();
			Vector2 playerPos = player.transform.position;

			if (Vector2.Distance(playerPos, transform.position) > maxDistance)
			{
				Vector2 diff = playerPos - (Vector2)transform.position;
				rb.AddForce(diff.normalized * forcePerJerk);
			}
		}
	}

	public void DoPhysicsSpin(float angle)
	{
		rb.freezeRotation = false;
		float finalAngle = rb.rotation + angle;
		rb.DORotate(finalAngle, 0.25f).SetEase(Ease.OutQuad).OnComplete(() =>
		{
			rb.SetRotation(finalAngle);
			rb.freezeRotation = true;
		});
	}
}
