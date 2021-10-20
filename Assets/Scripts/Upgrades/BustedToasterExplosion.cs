using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BustedToasterExplosion : MonoBehaviour
{
	public float timeToScale = 0.2f;

	private CollideWithEnemy damager;
	private DestroyThis destroyer;

	private void Awake()
	{
		damager = GetComponent<CollideWithEnemy>();
		destroyer = GetComponent<DestroyThis>();
	}

	private void StartTween()
	{
		transform.DOScale(new Vector3(0, 0, 1), timeToScale).onComplete += () => 
		{
			damager.enabled = false;
			destroyer.DestroyThisObject(0.2f);
		};
	}

	public void ActivateExplosion(float damage)
	{
		damager.damage = damage;
		StartTween();
	}
}
