using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeirdEyeball : MonoBehaviour
{
	private Transform endpoint;
	private Transform eyeballPos;

	private float damage;
	private float dangle;

	private void Awake()
	{
		endpoint = transform.GetChild(0);
		eyeballPos = transform.GetChild(1);
	}

	public void Setup(float damage)
	{
		this.damage = damage;
	}

	public void PointTowardsRandomEnemy()
	{
		Transform enemy = SpawnManager.instance.GetRandomEnemy();
		if (enemy == null)
			dangle = Random.Range(0, 360);
		else
			dangle = HelperFunctions.GetDAngleTowards(transform.position, enemy.position);

		transform.localRotation = Quaternion.Euler(0, 0, dangle);
	}

	public void Shoot()
	{
		ObjectPooler.instance.CreateElectricity(transform.position, endpoint.position, startWidth: 1.0f);
		ObjectPooler.instance.CreatePlayerLaserProjectile(eyeballPos.position, transform.localRotation, damage, true, false, circleSize: 1.0f, beamSize: 0.7f);

		float rangle = (dangle + 180) * Mathf.Deg2Rad;

		Vector2 offset = new Vector2(Mathf.Cos(rangle), Mathf.Sin(rangle)).normalized * 1.5f;
		Vector2 moveBackPos = (Vector2)transform.position + offset;

		transform.DOMove(moveBackPos, 0.4f).SetEase(Ease.OutSine);
		GetComponent<SpriteRenderer>().DOColor(new Color(1, 1, 1, 0), 0.4f).OnComplete(() => Destroy(gameObject));
	}
}
