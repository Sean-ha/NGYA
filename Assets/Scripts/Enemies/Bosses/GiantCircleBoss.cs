using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiantCircleBoss : MonoBehaviour
{
	public Animator myAnimator;

	private float currDangle;

	private int animIdle, animCharge, animShoot, animOpenMouth;

	// Radius of the circle boss guy. This is how far from the center the bullets will be created from.
	private const float distanceFromCenter = 1.2f;
	// Number of lasers to be shot
	private const int laserCount = 12;

	private void Awake()
	{
		animIdle = Animator.StringToHash("GiantCircleIdle");
		animCharge = Animator.StringToHash("GiantCircleCharge");
		animShoot = Animator.StringToHash("GiantCircleShoot");
		animOpenMouth = Animator.StringToHash("GiantCircleOpenMouth");
	}

	private void Start()
	{
		StartCoroutine(AttackBehavior());
	}

	private IEnumerator AttackBehavior()
	{
		while (true)
		{
			yield return new WaitForSeconds(1f);

			// Shoot 8 shots outwards in rapid succession
			for (int i = 0; i < 10; i++)
			{
				myAnimator.Play(animOpenMouth);
				ShootOutwards();
				yield return new WaitForSeconds(0.1f);
				myAnimator.Play(animIdle);
				yield return new WaitForSeconds(0.1f);
			}
			yield return new WaitForSeconds(1.5f);


			currDangle = 0;
			// Shoot 8 shots outwards in rapid succession
			for (int i = 0; i < 8; i++)
			{
				ShowLaserSight();
				yield return new WaitForSeconds(0.3f);
				myAnimator.Play(animCharge);
				yield return new WaitForSeconds(0.6f);
				myAnimator.Play(animShoot);
				ShootLasersOutwards();
				currDangle += 13f;
				yield return new WaitForSeconds(0.3f);
				myAnimator.Play(animIdle);
			}
		}
	}

	private void ShootOutwards()
	{
		int bulletCount = 8;

		float addValue = 360f / bulletCount;

		for (int i = 0; i < laserCount; i++)
		{
			float thisDangle = currDangle + addValue * i;
			Vector2 pos = GetBulletSourcePosition(thisDangle * Mathf.Deg2Rad);
			GameObject proj = ObjectPooler.instance.Create(Tag.CircularEnemyProjectile, pos, Quaternion.AngleAxis(thisDangle, Vector3.forward));
			EnemyProjectile projComp = proj.GetComponent<EnemyProjectile>();
			projComp.SetProjectile(3f, thisDangle, 15f, 30f);
		}

		currDangle += 11f;
		currDangle %= 360;
	}

	private void ShowLaserSight()
	{
		float addValue = 360f / laserCount;

		SoundManager.instance.PlaySound(SoundManager.Sound.LaserCharge, randomizePitch: false);
		for (int i = 0; i < laserCount; i++)
		{
			float thisDangle = currDangle + addValue * i;
			Vector2 pos = GetBulletSourcePosition(thisDangle * Mathf.Deg2Rad);
			GameObject proj = ObjectPooler.instance.CreateLaserSight(pos, Quaternion.AngleAxis(thisDangle, Vector3.forward), Color.red, 0.5f);
		}
	}

	private void ShootLasersOutwards()
	{
		float addValue = 360f / laserCount;

		SoundManager.instance.PlaySound(SoundManager.Sound.LaserShoot);
		CameraShake.instance.ShakeCamera(0.25f, 0.25f);
		for (int i = 0; i < laserCount; i++)
		{
			float thisDangle = currDangle + addValue * i;
			Vector2 pos = GetBulletSourcePosition(thisDangle * Mathf.Deg2Rad);
			GameObject proj = ObjectPooler.instance.Create(Tag.LaserProjectile, pos, Quaternion.AngleAxis(thisDangle, Vector3.forward));
			EnemyProjectile projComp = proj.GetComponent<EnemyLaserProjectile>();
			projComp.SetProjectile(0, thisDangle, 15f, 30f);
		}
	}

	private Vector2 GetBulletSourcePosition(float rangle)
	{
		return new Vector2(Mathf.Cos(rangle), Mathf.Sin(rangle)) * distanceFromCenter + (Vector2)transform.position;
	}
}
