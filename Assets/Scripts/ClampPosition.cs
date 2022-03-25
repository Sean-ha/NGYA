using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClampPosition : MonoBehaviour
{
	private void LateUpdate()
	{
		Vector2 clampedPos = transform.position;

		clampedPos.x = Mathf.RoundToInt(transform.position.x * 16) / 16f;
		clampedPos.y = Mathf.RoundToInt(transform.position.y * 16) / 16f;

		transform.position = clampedPos;
	}
}
