using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SetInactive : MonoBehaviour
{
	private Sequence currentSequence;

	private void Awake()
	{
		currentSequence = DOTween.Sequence();
	}

	public void DisableSelf()
	{
		gameObject.SetActive(false);
	}

	public void DisableSelfAfter(float time)
	{
		currentSequence.Kill();

		currentSequence = DOTween.Sequence();
		currentSequence.AppendInterval(time);
		currentSequence.AppendCallback(DisableSelf);
	}
}
