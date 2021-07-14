using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// AI for basic moving towards player behavior
public class MoveTowardsPlayer : MonoBehaviour
{
	public float speed;
	public TowardsPlayerTargetType targetType;

	private PlayerController player;
	private Rigidbody2D rb;

	public enum TowardsPlayerTargetType
	{
		Direct,		// Chase player's current position
		InFront,		// Chase position in front of player (where they are currently moving towards)
		Behind		// Chase position behind player (where they are moving away from)
	}

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		player = FindObjectOfType<PlayerController>();
	}

	private void FixedUpdate()
	{
		Vector2 playerPos = Vector2.zero;
		if (targetType == TowardsPlayerTargetType.Direct)
			playerPos = player.transform.position;
		else if (targetType == TowardsPlayerTargetType.InFront)
			playerPos = player.GetForwardPosition(2);
		else if (targetType == TowardsPlayerTargetType.Behind)
			playerPos = player.GetBackwardPosition(.5f);
		Vector2 diff = playerPos - (Vector2)transform.position;
		rb.AddForce(diff.normalized * speed);
	}
}
