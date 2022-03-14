using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

public class GiantCircleBoss : MonoBehaviour
{
	public Animator myAnimator;
	public EnemyHealth myHealth;
	public Collider2D myCollider;
	public GameObject bombObject;

	[BoxGroup("Stats")]
	public float phase2HealthThreshold;
	[BoxGroup("Stats")]
	public float phase3HealthThreshold;

	[BoxGroup("Stats")]
	public float phase1BulletDamage;
	[BoxGroup("Stats")]
	public float phase1LaserDamage;

	[BoxGroup("Stats")]
	public float phase2BombDamage;
	[BoxGroup("Stats")]
	public float phase3BombDamage;

	private Transform playerTransform;

	private float currDangle;
	private int currentPhase;
	private Coroutine currentCR;

	private int animIdle, animCharge, animShoot, animOpenMouth, animHurt;

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
		animHurt = Animator.StringToHash("GiantCircleHurt");
	}

	private void Start()
	{
		playerTransform = PlayerController.instance.transform;
		currentCR = StartCoroutine(Phase1AttackBehavior());
	}

	private IEnumerator Phase1AttackBehavior()
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

		for (int i = 0; i < bulletCount; i++)
		{
			float thisDangle = currDangle + addValue * i;
			Vector2 pos = GetBulletSourcePosition(thisDangle * Mathf.Deg2Rad);
			GameObject proj = ObjectPooler.instance.Create(Tag.CircularEnemyProjectile, pos, Quaternion.AngleAxis(thisDangle, Vector3.forward));
			EnemyProjectile projComp = proj.GetComponent<EnemyProjectile>();
			projComp.SetProjectile(4f, thisDangle, phase1BulletDamage, 40f);
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
			GameObject proj = ObjectPooler.instance.Create(Tag.EnemyLaserProjectile, pos, Quaternion.AngleAxis(thisDangle, Vector3.forward));
			EnemyProjectile projComp = proj.GetComponent<LaserProjectile>();
			projComp.SetProjectile(0, thisDangle, phase1LaserDamage, 30f);
		}
	}

	private Vector2 GetBulletSourcePosition(float rangle)
	{
		return new Vector2(Mathf.Cos(rangle), Mathf.Sin(rangle)) * distanceFromCenter + (Vector2)transform.position;
	}

	public void Die()
	{
		StopCoroutine(currentCR);

		myAnimator.Play(animHurt);
		CameraShake.instance.ShakeCameraLong(5f, 0.1f);
		myCollider.enabled = false;
		transform.DOShakePosition(4.75f, 0.2f).OnComplete(() =>
		{
			currentCR = StartCoroutine(DeathBehavior());
		});
	}

	private IEnumerator DeathBehavior()
	{
		yield return new WaitForSeconds(0.25f);
		ObjectPooler.instance.CreateCircleHitEffect(Color.white, transform.position, 1, 0.3f, large: true);
		ObjectPooler.instance.CreateHitParticles(Color.white, transform.position);
		ObjectPooler.instance.CreateHitParticles(Color.white, transform.position);
		ObjectPooler.instance.CreateHitParticles(Color.red, transform.position);
		ObjectPooler.instance.CreateHitParticles(Color.red, transform.position);

		GetComponent<ExpDropper>().DropExp();

		Destroy(transform.parent.gameObject);
	}


	// Called whenever boss takes damage. Determine whether or not to move to next phase based on current hp.
	public void CheckPhaseProgress()
	{
		if (myHealth.currentHealth <= phase2HealthThreshold && currentPhase == 0)
		{
			currentPhase++;
			StopCoroutine(currentCR);

			// Transition to phase 2:
			myAnimator.Play(animHurt);
			CameraShake.instance.ShakeCameraLong(1f, 0.15f);
			myCollider.enabled = false;
			transform.DOShakePosition(.75f, 0.3f).OnComplete(() =>
			{
				currentCR = StartCoroutine(Phase2AttackBehavior());
			});
		}
		else if (myHealth.currentHealth <= phase3HealthThreshold && currentPhase == 1)
		{
			currentPhase++;
			StopCoroutine(currentCR);

			// Transition to phase 3:
			myAnimator.Play(animHurt);
			CameraShake.instance.ShakeCameraLong(1f, 0.15f);
			myCollider.enabled = false;
			transform.DOShakePosition(.75f, 0.3f).OnComplete(() =>
			{
				currentCR = StartCoroutine(Phase3AttackBehavior());
			});
		}
	}

	private IEnumerator Phase2AttackBehavior()
	{
		yield return new WaitForSeconds(0.3f);
		myCollider.enabled = true;
		yield return new WaitForSeconds(0.4f);
		myAnimator.Play(animIdle);
		yield return new WaitForSeconds(0.4f);

		while (true)
		{
			for (int i = 0; i < 8; i++)
			{
				myAnimator.Play(animOpenMouth);
				GameObject bomb = Instantiate(bombObject, transform.position, Quaternion.identity);
				bomb.GetComponent<BombEnemyProjectile>().SetProjectile(playerTransform.position, 1.75f, phase2BombDamage, cameraShakeMagnitude: 0.15f);
				yield return new WaitForSeconds(0.15f);
				myAnimator.Play(animIdle);
				yield return new WaitForSeconds(0.15f);
			}

			yield return new WaitForSeconds(0.4f);

			List<Vector2> positions = PoissonDiscSampling.GeneratePoints(2.5f, 10, numSamplesBeforeRejection:10);
			myAnimator.Play(animOpenMouth);
			foreach (Vector2 pos in positions)
			{
				GameObject bomb = Instantiate(bombObject, transform.position, Quaternion.identity);
				bomb.GetComponent<BombEnemyProjectile>().SetProjectile(pos, 1.75f, phase2BombDamage);
			}
			yield return new WaitForSeconds(0.15f);
			myAnimator.Play(animIdle);

			yield return new WaitForSeconds(0.85f);
		}
	}

	private IEnumerator Phase3AttackBehavior()
	{
		yield return new WaitForSeconds(0.3f);
		myCollider.enabled = true;
		yield return new WaitForSeconds(0.4f);
		myAnimator.Play(animIdle);
		yield return new WaitForSeconds(0.4f);

		while (true)
		{
			List<Vector2> positions = PoissonDiscSampling.GeneratePoints(2.5f, 12, numSamplesBeforeRejection: 10);
			positions.Add(playerTransform.position);
			myAnimator.Play(animOpenMouth);
			foreach (Vector2 pos in positions)
			{
				GameObject bomb = Instantiate(bombObject, transform.position, Quaternion.identity);
				bomb.GetComponent<BombEnemyProjectile>().SetProjectile(pos, 1.75f, phase3BombDamage);
			}
			yield return new WaitForSeconds(0.15f);
			myAnimator.Play(animIdle);

			yield return new WaitForSeconds(1f);
		}
	}
}
