using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinNoRigidBody : MonoBehaviour
{
	public float spinsPerSecond;

	private float angle;
	private float spinsPerFrame;

	private void Awake()
	{
		spinsPerFrame = spinsPerSecond * 360f / Application.targetFrameRate;
	}

	private void Update()
	{
		if (angle > 360 || angle < -360)
			angle %= 360;

		angle += spinsPerFrame;
		transform.localRotation = Quaternion.Euler(0, 0, angle);
	}
}
