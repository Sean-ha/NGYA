using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DotShooter : Shooter
{
	[Tooltip("If true, bullets will shoot away from player center. If false, bullets will shoot straight ahead.")]
	public bool angledBullets;

	public ParticleSystem[] onShootParticles;

	private Tween scaleDownTween;

	public override void Shoot(float damage, float speed, float bulletDistance, int numberOfTargets)
	{
		foreach (ParticleSystem ps in onShootParticles)
		{
			ps.Play();
		}

		SoundManager.instance.PlaySound(SoundManager.Sound.PlayerShoot);

		float dangle;
		if (angledBullets)
		{
			// Get angle (degrees) of dot relative to player
			dangle = GetAngle();
		}
		else
		{
			// Get angle (degrees) of parent object relative to player
			dangle = transform.parent.rotation.eulerAngles.z;
		}

		ShootManager.instance.OnShoot(dangle, transform.position);

		// Tweening dot
		scaleDownTween.Complete();
		float currentSize = transform.localScale.x;
		transform.localScale = new Vector3(currentSize + 0.4f, currentSize + 0.4f, 1);
		scaleDownTween = transform.DOScale(new Vector3(currentSize, currentSize, 1), 0.1f);

		ObjectPooler.instance.CreatePlayerProjectile(transform.position, dangle, speed, damage, 
			numberOfTargets, bulletDistance, true, true);
	}

	private void CreateAmmoShell()
	{
		// Ammo shell
		/*
		GameObject shell = ObjectPooler.instance.Create(Tag.AmmoShell, transform.position, Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward));
		float shellAngle = dangle + Random.Range(80, 130f);
		float shellForce = Random.Range(1.4f, 2f);
		float shellTime = Random.Range(0.4f, 0.6f);
		float shellSpinTime = shellTime + Random.Range(0.15f, 0.3f);
		float shellSpinAmount = Random.Range(480, 1080);
		Vector2 shellDestination = new Vector2(Mathf.Cos(shellAngle * Mathf.Deg2Rad), Mathf.Sin(shellAngle * Mathf.Deg2Rad)) * shellForce;
		shellDestination += (Vector2)shell.transform.position;

		shell.transform.DOMove(shellDestination, shellTime).SetEase(Ease.OutQuad);
		shell.transform.DORotate(new Vector3(0, 0, shellSpinAmount), shellSpinTime, RotateMode.FastBeyond360).SetEase(Ease.OutQuad).onComplete += () => shell.SetActive(false);
	*/
	}
}
