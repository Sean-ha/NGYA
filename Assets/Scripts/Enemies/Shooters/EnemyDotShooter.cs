using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// Basic script for an enemy that just shoots a projectile in regular intervals. Angle is relative to body position
public class EnemyDotShooter : EnemyShooter
{
	[Tooltip("Time between shots in seconds. Put -1 if shooting not done through this script")]
	public float shotCooldown;

	public Tag shot = Tag.EnemyProjectile;

	private void Start()
	{
		if (shotCooldown >= 0)
		{
			StartCoroutine(ShootingBehavior());
		}
	}

	private IEnumerator ShootingBehavior()
	{
		while (true)
		{
			yield return new WaitForSeconds(shotCooldown);
			Shoot();
		}
	}

	public override void Shoot()
	{
		// Get angle between player and mouse location
		float angle = GetAngle();

		Scale();

		GameObject proj = ObjectPooler.instance.Create(shot, transform.position, Quaternion.AngleAxis(angle, Vector3.forward));
		proj.GetComponent<EnemyProjectile>().SetProjectile(shotSpeed, angle, damage, bulletDistance);
	}
}
