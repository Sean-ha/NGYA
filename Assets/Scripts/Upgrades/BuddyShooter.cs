using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BuddyShooter : Shooter
{
	public float shotSpeed;

	public new float GetAngle()
	{
		// Get angle towards mouse
		Vector2 pos = Input.mousePosition;
		Vector2 worldPos = Camera.main.ScreenToWorldPoint(pos);

		// Get angle between buddy and mouse location
		Vector2 diff = worldPos - (Vector2)transform.position;

		float angle = Mathf.Rad2Deg * Mathf.Atan2(diff.y, diff.x);
		return angle;
	}

	public override void Shoot(float damage, float bulletDistance, int numberOfTargets)
	{
		float roll = Random.Range(0f, 1f);
		if (roll >= ChanceToNotConsumeAmmo)
		{
			if (!AmmoSystem.instance.RemoveAmmo(ammoPerShot))
			{
				return;
			}
		}
		float angle = GetAngle();

		SoundManager.instance.PlaySound(SoundManager.Sound.BuddyShoot);
		ObjectPooler.instance.CreateCircleHitEffect(Color.white, transform.position, 0.6f);

		// Create bullet
		GameObject proj = ObjectPooler.instance.Create(Tag.PlayerProjectile, transform.position, Quaternion.AngleAxis(angle, Vector3.forward));
		proj.GetComponent<BasicProjectile>().SetProjectile(shotSpeed, angle, damage, numberOfTargets, bulletDistance);

		// Ammo shell
		GameObject shell = ObjectPooler.instance.Create(Tag.AmmoShell, transform.position, Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward));
		float shellAngle = angle + Random.Range(80, 130f);
		float shellForce = Random.Range(1.4f, 2f);
		float shellTime = Random.Range(0.4f, 0.6f);
		float shellSpinTime = shellTime + Random.Range(0.15f, 0.3f);
		float shellSpinAmount = Random.Range(480, 1080);
		Vector2 shellDestination = new Vector2(Mathf.Cos(shellAngle * Mathf.Deg2Rad), Mathf.Sin(shellAngle * Mathf.Deg2Rad)) * shellForce;
		shellDestination += (Vector2)shell.transform.position;

		shell.transform.DOMove(shellDestination, shellTime).SetEase(Ease.OutQuad);
		shell.transform.DORotate(new Vector3(0, 0, shellSpinAmount), shellSpinTime, RotateMode.FastBeyond360).SetEase(Ease.OutQuad).onComplete += () => shell.SetActive(false);
	}
}
