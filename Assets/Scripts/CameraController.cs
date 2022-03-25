using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class CameraController : MonoBehaviour
{
	public Transform toFollow;

	public PixelPerfectCamera ppc;

	private void Update()
	{
		Vector3 newPos = toFollow.position;

		newPos.z = transform.position.z;

		transform.position = newPos;
	}

	private void LateUpdate()
	{
		transform.position = ppc.RoundToPixel(transform.position);
	}
}
