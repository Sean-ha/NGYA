using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointToPlayer : MonoBehaviour
{
	private Transform player;

	private void Start()
	{
		player = GameAssets.instance.player.transform;
	}

	private void Update()
	{
		// Get angle between enemy and player location
		Vector2 diff = (Vector2)player.position - (Vector2)transform.position;

		float angle = Mathf.Rad2Deg * Mathf.Atan2(diff.y, diff.x);

		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	public void ForceUpdate()
	{
		Update();
	}
}
