using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// Basic script for an enemy that just shoots a projectile in regular intervals. Angle is relative to body position
public class EnemyDotShooter : EnemyShooter
{
	[Tooltip("Time between shots in seconds")]
	public float shotCooldown;

	private void Start()
	{
		StartCoroutine(ShootingBehavior());
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

		transform.DOScale(new Vector3(0.6f, 0.6f, 1), 0.05f).onComplete += () =>
		{
			transform.DOScale(new Vector3(0.2f, 0.2f, 1), 0.05f);
		};

		GameObject proj = ObjectPooler.instance.Create(Tag.EnemyProjectile, transform.position, Quaternion.AngleAxis(angle, Vector3.forward));
		proj.GetComponent<BasicEnemyProjectile>().SetProjectile(shotSpeed, angle, damage, bulletDistance);
	}
}
