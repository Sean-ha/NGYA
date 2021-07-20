using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyDotShooterStraight : EnemyShooter
{
	[Tooltip("Cooldown between burst shots")]
	public float longCooldown;
	[Tooltip("Cooldown between each shot during the burst")]
	public float shortCooldown;
	[Tooltip("Number of shots in each burst")]
	public int shotCount;

	private void Start()
	{
		StartCoroutine(ShootingBehavior());
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
		return transform.rotation.eulerAngles.z;
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
