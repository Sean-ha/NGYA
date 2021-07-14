using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
	public float damage { get; set; }

	// Number of enemies that can be hit by this projectile before it deactivates
	public int numberOfTargets { get; set; }

	// Distance the projectile will travel
	public float distance { get; set; }
}
