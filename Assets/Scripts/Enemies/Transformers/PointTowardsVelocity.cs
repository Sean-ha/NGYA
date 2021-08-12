using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sprite faces in the direction that it is moving
public class PointTowardsVelocity : MonoBehaviour
{
	private Rigidbody2D rb;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	private void FixedUpdate()
	{
		float angleRad = Mathf.Atan2(rb.velocity.y, rb.velocity.x);
		// float angleDegrees = Vector2.SignedAngle(Vector2.right, rb.velocity);
		// transform.rotation = Quaternion.AngleAxis(angleRad * Mathf.Rad2Deg, Vector3.forward);

		rb.rotation = Mathf.Rad2Deg * angleRad;
	}
}
