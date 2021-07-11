using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
	public Transform toFollow;

	private void Update()
	{
		transform.position = toFollow.position;
	}
}
