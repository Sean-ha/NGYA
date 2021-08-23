using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScaleByTween : MonoBehaviour
{
	public Vector2 initialScale;
	public Vector2 destinationScale;
	public float timeToScale;

	private Tween currentTween;

	public void ActivateScale()
	{
		currentTween.Kill();
		transform.localScale = initialScale;
		currentTween = transform.DOScale(destinationScale, timeToScale).SetEase(Ease.InQuad);
	}
}
