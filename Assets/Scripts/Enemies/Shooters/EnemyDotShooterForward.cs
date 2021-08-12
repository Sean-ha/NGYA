using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// Shooting bursts based on enemy's rotation
public class EnemyDotShooterForward : EnemyShooter
{
	[Tooltip("Type of shot to be produced (from ObjectPooler)")]
	public Tag bulletType = Tag.EnemyProjectile;

	[Tooltip("Cooldown between burst shots. Set to -1 if not shooting via this script")]
	public float longCooldown;
	[Tooltip("Cooldown between each shot during the burst")]
	public float shortCooldown;
	[Tooltip("Number of shots in each burst")]
	public int shotCount;

	[Tooltip("Whether or not to use parent's rotation for direction. Use if the dot is contained in a Holder")]
	public bool useParentRotation;

	private Coroutine shootingCR;

	private void Start()
	{
		if (longCooldown >= 0)
		{
			shootingCR = StartCoroutine(ShootingBehavior());
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

	protected new float GetAngle()
	{
		if (useParentRotation)
			return transform.parent.rotation.eulerAngles.z;
		else
			return transform.rotation.eulerAngles.z;
	}

	public override void Shoot()
	{
		// Get rotation angle
		float angle = GetAngle();

		Scale();

		GameObject proj = ObjectPooler.instance.Create(bulletType, transform.position, Quaternion.AngleAxis(angle, Vector3.forward));
		proj.GetComponent<EnemyProjectile>().SetProjectile(shotSpeed, angle, damage, bulletDistance);
	}

	public void CancelAndRestartShooting()
	{
		StopCoroutine(shootingCR);
		shootingCR = StartCoroutine(ShootingBehavior());
	}
}
