using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RotateBackAndForth : MonoBehaviour
{
	[Tooltip("Time it takes to rotate towards one side")]
	public float time;
	[Tooltip("Angle in degrees which is the max in either direction to rotate")]
	public float angle;

	private void Start()
	{
		RotateThis();
	}

	public void RotateThis()
	{
		transform.DORotate(new Vector3(0, 0, angle), 2 * time).SetEase(Ease.InOutQuad).SetUpdate(true).onComplete += () =>
			transform.DORotate(new Vector3(0, 0, -angle), 2 * time).SetUpdate(true).SetEase(Ease.InOutQuad).onComplete += () => RotateThis();
	}
}
