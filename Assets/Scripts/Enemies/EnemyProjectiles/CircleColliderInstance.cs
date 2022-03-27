using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CircleColliderInstance : MonoBehaviour
{
	private CircleCollider2D coll;
	private Damager damager;

	private void Awake()
	{
		coll = GetComponent<CircleCollider2D>();
		damager = GetComponent<Damager>();
	}

	public void Setup(float radius, float damage, float duration, bool destroyAfter=true)
	{
		coll.radius = radius;
		damager.damage = damage;

		DOVirtual.DelayedCall(duration, () =>
		{
			if (destroyAfter)
				Destroy(gameObject);
			else
				gameObject.SetActive(false);
		}, ignoreTimeScale: false);
	}
}
