using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointToCursor : MonoBehaviour
{
	private void Update()
	{
		if (TimeManager.IsPaused) return;

		Vector2 pos = Input.mousePosition;
		Vector2 worldPos = Camera.main.ScreenToWorldPoint(pos);

		// Get angle between player and mouse location
		Vector2 diff = worldPos - (Vector2)transform.position;

		float angle = Mathf.Rad2Deg * Mathf.Atan2(diff.y, diff.x);

		transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}
}
