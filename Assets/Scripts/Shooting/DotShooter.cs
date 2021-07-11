using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotShooter : Shooter
{
	public GameObject projectile;
	public float shotSpeed;

	public override void Shoot()
	{
		// Get angle between player and mouse location
		float angle = GetAngle();

		LeanTween.scale(gameObject, new Vector3(0.6f, 0.6f, 1), 0.05f).setOnComplete(() =>
		{
			LeanTween.scale(gameObject, new Vector3(0.2f, 0.2f, 1), 0.05f);
		});
		// float distanceOffset = 0.8f;
		GameObject proj = Instantiate(projectile, transform.position, Quaternion.AngleAxis(angle, Vector3.forward));
		// proj.transform.position += new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * distanceOffset;

		proj.transform.GetComponentInChildren<BasicProjectile>().SetProjectile(shotSpeed, angle);
	}
}
