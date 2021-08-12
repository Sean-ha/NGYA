using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LevelUpText : MonoBehaviour
{
	private void Start()
	{
		transform.DOLocalMoveY(transform.localPosition.y + 0.5f, 1.25f).SetEase(Ease.OutQuad).onComplete += () =>
		{
			transform.DOScaleY(0, 0.5f).SetEase(Ease.OutQuad).onComplete += () => Destroy(gameObject);
		};
	}
}
