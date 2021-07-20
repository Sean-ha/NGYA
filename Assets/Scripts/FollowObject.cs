using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
	public Transform toFollow;
	public bool followRotation;

	private void Update()
	{
		transform.position = toFollow.position;

		if (followRotation)
			transform.rotation = toFollow.rotation;
	}
}
