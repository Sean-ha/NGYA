using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

// Shooting bursts based on enemy's rotation
public class EnemyDotShooterStraight : EnemyShooter
{
	[Tooltip("Type of shot to be produced (from ObjectPooler)")]
	public Tag bulletType = Tag.EnemyProjectile;

	[Tooltip("Cooldown between burst shots")]
	public float longCooldown;
	[Tooltip("Cooldown between each shot during the burst")]
	public float shortCooldown;
	[Tooltip("Number of shots in each burst")]
	public int shotCount;

	[Tooltip("Angle bullet will be fired at (degrees)")]
	public float straightAngle;

	[Header("Randomization")]

	[Tooltip("Whether or not to randomize bullet stats. If false, leave fields below blank.")]
	public bool randomizeShots;
	
	[ShowIf("randomizeShots")]
	public float bulletSpeedVariance;
	[ShowIf("randomizeShots")]
	public float bulletDistanceVariance;
	[ShowIf("randomizeShots")]
	public float shortCooldownVariance;
	[ShowIf("randomizeShots")]
	public int shotCountVariance;
	[ShowIf("randomizeShots")]
	public float angleVariance;

	private float currBulletSpeed;
	private float currBulletDistance;
	private float currAngle;

	private Coroutine shootingCR;

	protected override void OnAwake()
	{
		currBulletSpeed = shotSpeed;
		currBulletDistance = bulletDistance;

		if (longCooldown >= 0)
		{
			shootingCR = StartCoroutine(ShootingBehavior());
		}
	}

	public void InitiateSingleShootingBehavior()
	{
		shootingCR = StartCoroutine(SingleInstanceShootingBehavior());
	}

	private IEnumerator SingleInstanceShootingBehavior()
	{
		yield return new WaitForSeconds(longCooldown);
		for (int i = 0; i < shotCount; i++)
		{
			Shoot();
			yield return new WaitForSeconds(shortCooldown);
		}
	}

	private IEnumerator ShootingBehavior()
	{
		while (true)
		{
			yield return SingleInstanceShootingBehavior();
		}
	}

	// In degrees
	protected new float GetAngle()
	{
		return straightAngle;
	}

	private void RandomizeShot()
	{
		currBulletSpeed = shotSpeed + Random.Range(-bulletSpeedVariance, bulletSpeedVariance);
		currBulletDistance = bulletDistance + Random.Range(-bulletDistanceVariance, bulletDistanceVariance);

		currAngle += Random.Range(-angleVariance, angleVariance);
	}

	public override void Shoot()
	{
		onShoot.Invoke();

		// Get angle between player and mouse location
		currAngle = GetAngle();

		if (randomizeShots)
		{
			RandomizeShot();
		}

		Scale();

		GameObject proj = ObjectPooler.instance.Create(bulletType, transform.position, Quaternion.AngleAxis(currAngle, Vector3.forward));
		proj.GetComponent<EnemyProjectile>().SetProjectile(currBulletSpeed, currAngle, damage, currBulletDistance);
	}

	public void CancelAndRestartShooting()
	{
		StopCoroutine(shootingCR);
		shootingCR = StartCoroutine(ShootingBehavior());
	}
}
