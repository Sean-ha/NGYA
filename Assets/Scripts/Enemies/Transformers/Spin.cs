using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
	public float spinsPerSecond;

	private Rigidbody2D rb;
	private float spinsPerFrame;
	private float angle = 0;

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		spinsPerFrame = spinsPerSecond * 360f / 50f;
	}

	private void FixedUpdate()
	{
		if (angle > 360)
			angle %= 360;

		angle += spinsPerFrame;
		rb.MoveRotation(Quaternion.Euler(0, 0, angle));
	}
}
