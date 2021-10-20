using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class CritText : MonoBehaviour
{
	public TextMeshPro tmp;

	private Color defaultColor;
	private Color transparentColor;

	private Tween moveUpTween, fadeTween, rotateTween;

	private void Awake()
	{
		defaultColor = tmp.color;
		transparentColor = defaultColor;
		transparentColor.a = 0;
	}

	private void OnEnable()
	{
		ResetToDefault();

		moveUpTween = transform.DOMoveY(transform.position.y + 0.5f, 0.4f).SetEase(Ease.OutQuad);
		// shrinkTween = transform.DOScaleY(0, 0.5f).SetEase(Ease.OutQuad).OnComplete(() => gameObject.SetActive(false));
		DOVirtual.DelayedCall(0.25f, () =>
		{
			fadeTween = DOTween.To(() => tmp.color, (Color value) => tmp.color = value, transparentColor, 0.4f).SetEase(Ease.InQuad)
			.OnComplete(() => gameObject.SetActive(false));
		});

		float rotation = Random.Range(-15f, 15f);
		rotateTween = transform.DOLocalRotate(new Vector3(0, 0, rotation), 0.4f);
	}

	// Resets object to default state (include rotation, color, anything else)
	private void ResetToDefault()
	{
		moveUpTween.Kill();
		fadeTween.Kill();
		rotateTween.Kill();

		tmp.color = defaultColor;
		transform.rotation = Quaternion.identity;
	}
}
