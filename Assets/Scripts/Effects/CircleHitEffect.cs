using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CircleHitEffect : MonoBehaviour
{
	public float startSize;
	public float timeToScale = 0.2f;

	private SpriteRenderer sr;

	private void Awake()
	{
		sr = GetComponent<SpriteRenderer>();
	}

	private void StartTween()
	{
		transform.DOKill();
		transform.localScale = new Vector3(startSize, startSize, 1);
		transform.DOScale(new Vector3(0, 0, 1), timeToScale).onComplete += () => gameObject.SetActive(false);
	}

	public void SetHitEffect(Color color, float startSize, float timeToShrink)
	{
		sr.color = color;
		this.startSize = startSize;
		timeToScale = timeToShrink;

		StartTween();
	}
}
