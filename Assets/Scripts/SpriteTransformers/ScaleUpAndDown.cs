using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScaleUpAndDown : MonoBehaviour
{
	// Assume the axes scale at the same rate
	public float maxScale;
	public float minScale;

	[Tooltip("Amount of time it takes to scale either from min to max or vice versa")]
	public float scaleRate;

	private float currentScale;

	private void Awake()
	{
		currentScale = transform.localScale.x;	
	}

	private void OnEnable()
	{
		ScalingBehavior();
	}

	private void ScalingBehavior()
	{
		transform.DOScale(maxScale, scaleRate).SetEase(Ease.InOutQuad).OnComplete(() =>
		{
			transform.DOScale(minScale, scaleRate).SetEase(Ease.InOutQuad).OnComplete(ScalingBehavior);
		});
	}
}
