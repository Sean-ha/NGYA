using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
	public float spinsPerSecond;

	private float spinsPerFrame;
	private float angle = 0;

	private void Awake()
	{
		spinsPerFrame = spinsPerSecond * 360 / Application.targetFrameRate;
	}

	private void Update()
	{
		if (angle > 360)
			angle %= 360;

		angle += spinsPerFrame;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}
}
