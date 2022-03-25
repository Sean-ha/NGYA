using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointToCursorStack : MonoBehaviour
{
	[SerializeField] private DisplayObject stackedSprite;

	private void Update()
	{
		if (TimeManager.IsPaused) return;

		Vector2 pos = Input.mousePosition;
		Vector2 worldPos = Camera.main.ScreenToWorldPoint(pos);

		// Get angle between player and mouse location
		Vector2 diff = worldPos - (Vector2)transform.position;

		float angle = Mathf.Rad2Deg * Mathf.Atan2(diff.y, diff.x);

		stackedSprite.rotation = new Vector3(0f, 0f, angle);
	}
}
