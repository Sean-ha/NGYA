using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyProjectile : MonoBehaviour
{
	public float damage { get; set; }

	// Distance the projectile will travel
	public float distance { get; set; }

	public abstract void SetProjectile(float speed, float angle, float damage, float distance);
}
