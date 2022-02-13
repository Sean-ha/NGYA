using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Shooter : MonoBehaviour
{
	/// <summary>
	/// Returns the angle the current dot is at relative to the current body, in degrees
	/// </summary>
	public float GetAngle()
	{
		Vector2 diff = (Vector2)transform.position - (Vector2)transform.parent.position;

		return Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
	}

	public abstract void Shoot(float damage, float bulletDistance, int numberOfTargets);
}
