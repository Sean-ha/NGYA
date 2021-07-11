using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackTest : MonoBehaviour
{
	public GameObject projectile;

	private void Awake()
	{
		InvokeRepeating("ShootProjectile", 0.25f, 1f);
	}

	private void ShootProjectile()
	{
		GameObject proj = Instantiate(projectile, transform.position, Quaternion.identity);
	}
}
