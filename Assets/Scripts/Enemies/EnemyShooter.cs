using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyShooter : MonoBehaviour
{
	[Tooltip("Speed of shots")]
	public float shotSpeed;
	[Tooltip("Maximum distance shot can travel")]
	public float bulletDistance;
	public float damage;

	/// <summary>
	/// Get angle in degrees that the current dot is at relative to its body
	/// </summary>
	protected float GetAngle()
	{
		Vector2 diff = (Vector2)transform.position - (Vector2)transform.parent.position;

		return Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
	}
	

	public abstract void Shoot();
}
