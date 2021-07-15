using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotShooter : Shooter
{
	public float shotSpeed;

	public override void Shoot(float damage, float bulletDistance, int numberOfTargets)
	{
		if (!AmmoSystem.instance.RemoveAmmo())
			return;

		// Get angle between player and mouse location
		float angle = GetAngle();

		LeanTween.scale(gameObject, new Vector3(0.6f, 0.6f, 1), 0.05f).setOnComplete(() =>
		{
			LeanTween.scale(gameObject, new Vector3(0.2f, 0.2f, 1), 0.05f);
		});
		GameObject proj = ObjectPooler.instance.Create(Tag.PlayerProjectile, transform.position, Quaternion.AngleAxis(angle, Vector3.forward));
		proj.GetComponent<BasicProjectile>().SetProjectile(shotSpeed, angle, damage, numberOfTargets, bulletDistance);

		// Ammo shell
		GameObject shell = ObjectPooler.instance.Create(Tag.AmmoShell, transform.position, Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward));
		float shellAngle = angle + Random.Range(80, 130f);
		float shellForce = Random.Range(1.4f, 2f);
		float shellTime = Random.Range(0.4f, 0.6f);
		float shellSpinAmount = Random.Range(1080, 1800);
		Vector2 shellDestination = new Vector2(Mathf.Cos(shellAngle * Mathf.Deg2Rad), Mathf.Sin(shellAngle * Mathf.Deg2Rad)) * shellForce;
		shellDestination += (Vector2)shell.transform.position;
		LeanTween.move(shell, shellDestination, shellTime).setEaseOutCubic();
		LeanTween.rotateZ(shell, shellSpinAmount, shellTime).setEaseOutCubic().setOnComplete(() => shell.SetActive(false));
	}
}
