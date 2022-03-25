using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;

public class CircleHitEffect : MonoBehaviour
{
	private SpriteRenderer sr;

	private void Awake()
	{
		sr = GetComponent<SpriteRenderer>();
	}

	// startWhite: if true, this hit effect will start white and turn into the specified color after a little bit
	public void SetHitEffect(Color color, float startSize, float timeToShrink, bool startWhite = false)
	{
		if (startWhite)
			sr.color = Color.white;
		else
			sr.color = color;

		// Start the tween stuff
		transform.DOKill();

		DOVirtual.DelayedCall(Mathf.Min(timeToShrink, 0.06f), () => sr.color = color, ignoreTimeScale: false).SetId(transform);
		transform.localScale = new Vector3(startSize, startSize, 1);
		transform.DOScale(new Vector3(0, 0, 1), timeToShrink).onComplete += () => gameObject.SetActive(false);
	}
}
