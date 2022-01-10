using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Explosion : MonoBehaviour
{
	public void ActivateExplosion(float damage, bool canCrit)
	{
		float randomDangle = Random.Range(0, 359);
		transform.localRotation = Quaternion.Euler(0, 0, randomDangle);

		Transform circle = transform.GetChild(0);
		SpriteRenderer circleSR = circle.GetComponent<SpriteRenderer>();

		CollideWithEnemy damager = GetComponent<CollideWithEnemy>();
		damager.damage = damage;
		damager.canCrit = canCrit;

		Vector3 newScale = transform.localScale - new Vector3(0.03f, 0.03f, 0f);

		// expl.transform.DORotate(new Vector3(0, 0, -300f), 1f, RotateMode.FastBeyond360).SetEase(Ease.OutSine).OnComplete(() => Destroy(expl));
		DOVirtual.DelayedCall(0.1f, () => {
			GetComponent<Collider2D>().enabled = false;
			circleSR.color = new Color(1, 1, 1, 0.1f);
			circleSR.sortingLayerID = 0;
			transform.DOScale(newScale, 0.42f);
			HelperFunctions.BlinkSpriteRenderer(GetComponent<SpriteRenderer>(), 3, 0.07f, 0.07f);
			HelperFunctions.BlinkSpriteRenderer(circleSR, 3, 0.07f, 0.07f);
			Destroy(gameObject, 0.42f);
		}, ignoreTimeScale: false);
	}
}
