using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyProjectile : MonoBehaviour
{
	public float damage { get; set; }

	// Distance the projectile will travel
	public float distance { get; set; }

	// Angle is in degrees
	public abstract void SetProjectile(float speed, float dangle, float damage, float distance);
}
