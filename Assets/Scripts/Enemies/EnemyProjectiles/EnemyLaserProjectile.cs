using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyLaserProjectile : EnemyProjectile
{
	public Transform beamRectangle;
	public Transform circleEffect;
	public ParticleSystem shootParticles;
	public ParticleSystem laserParticles;
	public Collider2D coll;

	private SpriteRenderer beamRectangleSR;
	private SpriteRenderer circleEffectSR;

	private Coroutine currCR;

	private void Awake()
	{
		beamRectangleSR = beamRectangle.GetComponent<SpriteRenderer>();
		circleEffectSR = circleEffect.GetComponent<SpriteRenderer>();
	}

	public override void SetProjectile(float speed, float angle, float damage, float distance)
	{
		ActivateLaser(damage);
	}

	public void ActivateLaser(float damage)
	{
		if (currCR != null)
			StopCoroutine(currCR);

		Sequence seq = DOTween.Sequence();

		coll.GetComponent<Damager>().damage = damage;

		circleEffectSR.color = Color.white;
		seq.AppendInterval(0.09f);
		seq.AppendCallback(() => circleEffectSR.color = Color.red);

		circleEffect.localScale = new Vector3(1.75f, 1.75f, 1);
		circleEffect.DOScale(new Vector3(0, 0, 1), 0.2f);

		
		beamRectangleSR.color = Color.white;
		seq.AppendCallback(() => beamRectangleSR.color = Color.red);
		beamRectangle.localScale = new Vector3(beamRectangle.localScale.x, 0.5f, 1);
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
