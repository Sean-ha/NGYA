using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LaserProjectile : EnemyProjectile
{
	public Transform beamRectangle;
	public Transform circleEffect;
	public ParticleSystem shootParticles;
	public ParticleSystem laserParticles;
	public BoxCollider2D coll;

	private SpriteRenderer beamRectangleSR;
	private SpriteRenderer circleEffectSR;

	private Coroutine currCR;

	private void Awake()
	{
		beamRectangleSR = beamRectangle.GetComponent<SpriteRenderer>();
		circleEffectSR = circleEffect.GetComponent<SpriteRenderer>();
	}

	// USE THIS METHOD WHEN INVOKING ENEMYLASER
	public override void SetProjectile(float speed, float angle, float damage, float distance)
	{
		ActivateLaser(damage, enemyLaser: true);
	}

	// This method is used for player lasers
	public void ActivateLaser(float damage, bool canCrit = false, bool onHitEffects = false, float circleEffectScale = 1.75f, float beamSize = 0.5f, bool enemyLaser = false)
	{
		if (currCR != null)
			StopCoroutine(currCR);

		Sequence seq = DOTween.Sequence();

		coll.GetComponent<Damager>().damage = damage;

		circleEffectSR.color = Color.white;
		if (enemyLaser)
		{
			seq.AppendInterval(0.09f);
			seq.AppendCallback(() => circleEffectSR.color = Color.red);
			seq.AppendCallback(() => beamRectangleSR.color = Color.red);
			coll.size = new Vector2(coll.size.x, beamSize);
		}
		else
		{
			// Player laser has increased hitbox size than it looks
			coll.size = new Vector2(coll.size.x, beamSize + 0.3f);
		}

		circleEffect.localScale = new Vector3(circleEffectScale, circleEffectScale, 1);
		circleEffect.DOScale(new Vector3(0, 0, 1), 0.2f);

		
		
		beamRectangleSR.color = Color.white;
		
		beamRectangle.localScale = new Vector3(beamRectangle.localScale.x, beamSize, 1);
		beamRectangle.DOScale(new Vector3(beamRectangle.localScale.x, 0), 0.2f);

		shootParticles.Play();

		laserParticles.Stop();
		laserParticles.transform.localPosition = new Vector2(0, 0);
		laserParticles.Play();
		laserParticles.transform.DOLocalMoveX(50, 0.16f);

		currCR = StartCoroutine(ColliderOnOff());
	}

	private IEnumerator ColliderOnOff()
	{
		coll.gameObject.SetActive(true);
		yield return new WaitForSeconds(0.1f);
		coll.gameObject.SetActive(false);
	}
}
