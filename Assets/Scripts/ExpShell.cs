using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ExpShell : MonoBehaviour
{
	// Amount of exp this exp shell will give. 1 is the default. There might be special exceptions in the future.
	public int expAmount = 1;

	// The curve on which the shell moves towards the player
	public AnimationCurve attractionCurve;

	private Transform player;
	private bool isMovingToPlayer;
	private float timer;
	private float timeToReach;

	private void Awake()
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
	}

	private void OnEnable()
	{
		float shellAngle = Random.Range(0, 360);
		float shellForce = Random.Range(1.4f, 2f);
		float shellTime = Random.Range(0.4f, 0.6f);
		float shellSpinTime = shellTime + Random.Range(0.1f, 0.3f);
		float shellSpinAmount = Random.Range(480, 1080);

		Vector2 shellDestination = new Vector2(Mathf.Cos(shellAngle * Mathf.Deg2Rad), Mathf.Sin(shellAngle * Mathf.Deg2Rad)) * shellForce;
		shellDestination += (Vector2)transform.position;

		transform.DOMove(shellDestination, shellTime).SetEase(Ease.OutQuad);
		transform.DORotate(new Vector3(0, 0, shellSpinAmount), shellSpinTime, RotateMode.FastBeyond360).SetEase(Ease.OutQuad).onComplete += () => SetMoveToPlayer();
	}

	// This ammo shell begins moving towards the player.
	private void SetMoveToPlayer()
	{
		isMovingToPlayer = true;
		timer = 0f;
		timeToReach = 0.4f;
	}

	private void Update()
	{
		if (TimeManager.IsPaused)
			return;

		if (isMovingToPlayer)
		{
			transform.position = Vector3.Lerp(transform.position, player.position, attractionCurve.Evaluate(timer / timeToReach));
			timer += Time.deltaTime;
		}
	}

	private void OnDisable()
	{
		transform.DOKill();
		isMovingToPlayer = false;
	}
}
