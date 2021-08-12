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

	public override void SetProjectile(float speed, float angle, float damage, float distance)
	{
		ActivateLaser(damage);
	}

	public void ActivateLaser(float damage)
	{
		coll.GetComponent<Damager>().damage = damage;

		circleEffect.localScale = new Vector3(1, 1, 1);
		circleEffect.DOScale(new Vector3(0, 0, 1), 0.2f);

		beamRectangle.localScale = new Vector3(beamRectangle.localScale.x, 0.3f, 1);
		beamRectangle.DOScale(new Vector3(beamRectangle.localScale.x, 0), 0.2f);

		shootParticles.Play();

		laserParticles.Stop();
		laserParticles.transform.localPosition = new Vector2(0, 0);
		laserParticles.Play();
		laserParticles.transform.DOLocalMoveX(30, 0.1f);

		StartCoroutine(ColliderOnOff());
	}

	private IEnumerator ColliderOnOff()
	{
		coll.gameObject.SetActive(true);
		yield return new WaitForSeconds(0.1f);
		coll.gameObject.SetActive(false);
	}
}
