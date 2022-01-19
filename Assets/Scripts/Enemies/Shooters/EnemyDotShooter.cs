using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// Basic script for an enemy that just shoots a projectile in regular intervals. Angle is relative to body position
public class EnemyDotShooter : EnemyShooter
{
	[Tooltip("Cooldown between burst shots. Set to -1 if not shooting via this script")]
	public float longCooldown;
	[Tooltip("Cooldown between each shot during the burst")]
	public float shortCooldown;
	[Tooltip("Number of shots in each burst")]
	public int shotCount;

	public Tag shot = Tag.EnemyProjectile;

	private void Start()
	{
		if (longCooldown >= 0)
		{
			StartCoroutine(ShootingBehavior());
		}
	}

	private IEnumerator ShootingBehavior()
	{
		while (true)
		{
			yield return new WaitForSeconds(longCooldown);
			for (int i = 0; i < shotCount; i++)
			{
				Shoot();
				yield return new WaitForSeconds(shortCooldown);
			}
		}
	}

	public override void Shoot()
	{
		onShoot.Invoke();

		// Get angle between player and mouse location
		float angle = GetAngle();

		Scale();

		GameObject proj = ObjectPooler.instance.Create(shot, transform.position, Quaternion.AngleAxis(angle, Vector3.forward));
		proj.GetComponent<EnemyProjectile>().SetProjectile(shotSpeed, angle, damage, bulletDistance);
	}
}
