using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLaserProjectile : MonoBehaviour
{
	public Transform beamRectangle;
	public Transform circleEffect;
	public List<ParticleSystem> shootParticles;
	public ParticleSystem laserParticles;
	public Collider2D coll;

	private SpriteRenderer beamRectangleSR;
	private SpriteRenderer circleEffectSR;

	private Vector3 circleEffectDefaultSize;
	private float beamRectangleDefaultSizeY;

	private Coroutine currCR;

	private void Awake()
	{
		circleEffectDefaultSize = circleEffect.localScale;
		beamRectangleDefaultSizeY = beamRectangle.localScale.y;
		beamRectangleSR = beamRectangle.GetComponent<SpriteRenderer>();
		circleEffectSR = circleEffect.GetComponent<SpriteRenderer>();
	}

	public void ActivateLaser(float damage)
	{
		if (currCR != null)
			StopCoroutine(currCR);

		coll.GetComponent<CollideWithEnemy>().damage = damage;

		circleEffect.localScale = circleEffectDefaultSize;
		circleEffect.DOScale(new Vector3(0, 0, 1), 0.2f);


		beamRectangle.localScale = new Vector3(beamRectangle.localScale.x, beamRectangleDefaultSizeY, 1);
		beamRectangle.DOScale(new Vector3(beamRectangle.localScale.x, 0), 0.2f);

		foreach (ParticleSystem ps in shootParticles)
			ps.Play();

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
