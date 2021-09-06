using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinNoRigidBody : MonoBehaviour
{
	public float spinsPerSecond;

	private float angle;
	private float degreesPerSecond;

	private void Awake()
	{
		degreesPerSecond = spinsPerSecond * 360f;
	}

	private void Update()
	{
		if (angle > 360 || angle < -360)
			angle %= 360;

		angle += degreesPerSecond * Time.deltaTime;

		transform.localRotation = Quaternion.Euler(0, 0, angle);
	}
}
