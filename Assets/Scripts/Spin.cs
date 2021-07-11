using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
	float angle = 0;

	private void Update()
	{
		angle += 2;
		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}
}
