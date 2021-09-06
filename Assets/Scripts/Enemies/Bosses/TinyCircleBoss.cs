using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

public class TinyCircleBoss : MonoBehaviour
{
	public Animator myAnimator;
	public EnemyHealth myHealth;
	public Collider2D myCollider;
	public ParticleSystem chargeUpParticles;

	[BoxGroup("Stats")]
	public float phase2HealthThreshold;

	[BoxGroup("Stats")]
	public float phase1BulletDamage;

	private Transform playerTransform;

	private float currDangle;
	private int currentPhase;
	private Coroutine currentCR;

	private int animIdle, animOpenMouth, animHurt, animCharge, animShoot;

	// Radius of the circle boss guy. This is how far from the center the bullets will be created from.
	private const float distanceFromCenter = 0.1f;

	private void Awake()
	{
		animIdle = Animator.StringToHash("TinyCircleIdle");
		animOpenMouth = Animator.StringToHash("TinyCircleOpenMouth");
		animHurt = Animator.StringToHash("TinyCircleHurt");
		animCharge = Animator.StringToHash("TinyCircleCharge");
		animShoot = Animator.StringToHash("TinyCircleShoot");
	}

	private void Start()
	{
		playerTransform = PlayerController.instance.transform;
		currentCR = StartCoroutine(Phase1AttackBehavior());
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
	}

	private IEnumerator Phase1AttackBehavior()
	{
		yield return new WaitForSeconds(1f);
		while (true)
		{
			for (int i = 0; i < 4; i++)
			{
				myAnimator.Play(animOpenMouth);
				ShootOutwards();
				yield return new WaitForSeconds(0.1f);
				myAnimator.Play(animIdle);
				yield return new WaitForSeconds(0.1f);
			}

			yield return new WaitForSeconds(0.15f);
			chargeUpParticles.Play();
			myAnimator.Play(animCharge);
			yield return new WaitForSeconds(0.6f);

			myAnimator.Play(animShoot);
			ShootShotgunAtPlayer();
			yield return new WaitForSeconds(0.4f);
			myAnimator.Play(animIdle);

			yield return new WaitForSeconds(0.2f);
		}
	}

	private void ShootOutwards()
	{
		SoundManager.instance.PlaySound(SoundManager.Sound.BasicEnemyShoot);
		int bulletCount = 8;

		float addValue = 360f / bulletCount;

		for (int i = 0; i < bulletCount; i++)
		{
			float thisDangle = currDangle + addValue * i;
			Vector2 pos = GetBulletSourcePosition(thisDangle * Mathf.Deg2Rad);
			GameObject proj = ObjectPooler.instance.Create(Tag.CircularEnemyProjectile, pos, Quaternion.AngleAxis(thisDangle, Vector3.forward));
			EnemyProjectile projComp = proj.GetComponent<EnemyProjectile>();
			projComp.SetProjectile(2.15f, thisDangle, phase1BulletDamage, 30f);
		}
	}

	private void ShootShotgunAtPlayer()
	{
		SoundManager.instance.PlaySound(SoundManager.Sound.BigShoot);
		float dangle = GetDangleTowardsPlayer();

		Vector2 circleEffectPos = GetBulletSourcePosition(dangle * Mathf.Deg2Rad);
		ObjectPooler.instance.CreateCircleHitEffect(Color.red, circleEffectPos, 0.6f);
		ObjectPooler.instance.CreateHitParticles(Color.red, circleEffectPos);
		ObjectPooler.instance.CreateHitParticles(Color.red, circleEffectPos);

		int bulletCount = 15;

		for (int i = 0; i < bulletCount; i++)
		{
			float thisDangle = dangle + Random.Range(-12f, 12f);
			Vector2 pos = GetBulletSourcePosition(thisDangle * Mathf.Deg2Rad);
			GameObject proj = ObjectPooler.instance.Create(Tag.EnemyProjectile, pos, Quaternion.AngleAxis(thisDangle, Vector3.forward));
			EnemyProjectile projComp = proj.GetComponent<EnemyProjectile>();
			projComp.SetProjectile(Random.Range(4.5f, 5.5f), thisDangle, phase1BulletDamage, 30f);
		}
	}

	private Vector2 GetBulletSourcePosition(float rangle)
	{
		return new Vector2(Mathf.Cos(rangle), Mathf.Sin(rangle)) * distanceFromCenter + (Vector2)transform.position;
	}

	private float GetDangleTowardsPlayer()
	{
		// Get angle between enemy and player location
		Vector2 diff = (Vector2)playerTransform.position - (Vector2)transform.position;
		float angle = Mathf.Rad2Deg * Mathf.Atan2(diff.y, diff.x);

		return angle;
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
			yield return new WaitForSeconds(0.1f);
		}
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
}
