using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CircleProgressBar : MonoBehaviour
{
	// Adjust fillValule from other scripts to change progress amount
	[Range(0, 100)]
	public float fillValue = 0;
	public Image radialImage;
	public RectTransform roundEndHolder;

	public Image roundStart;
	public Image roundEnd;

	private ParticleSystem filledParticles;

	private void Awake()
	{
		filledParticles = transform.GetComponentInChildren<ParticleSystem>();
	}

	private void Update()
	{
		FillCircleValue(fillValue);
	}

	/// <summary>
	/// Fills the progress bar by the given amount (between 0 and 100).
	/// </summary>
	/// <param name="value">Between 0 and 100</param>
	private void FillCircleValue(float value)
	{
		float fillAmount = (value / 100.0f);
		radialImage.fillAmount = fillAmount;
		float angle = fillAmount * 360;
		roundEndHolder.localEulerAngles = new Vector3(0, 0, -angle);
	}

	public void SetColor(Color color)
	{
		radialImage.color = color;
		roundStart.color = color;
		roundEnd.color = color;
	}

	// Updates the loading bar's size
	private void UpdateSize(float newSize)
	{
		radialImage.rectTransform.sizeDelta = new Vector2(newSize, newSize);
		roundEndHolder.sizeDelta = new Vector2(newSize, newSize);
	}

	public void BeginBar(float timeToFull, TweenCallback onComplete)
	{
		fillValue = 0;

		Tween fill = DOTween.To(() => fillValue, (float val) => fillValue = val, 100, timeToFull);
		// scaling
		fill.onComplete += () =>
		{
			DOTween.To(() => roundEndHolder.sizeDelta.x, (float val) => UpdateSize(val), 350f, 0.05f).onComplete += () =>
			{
				DOTween.To(() => roundEndHolder.sizeDelta.x, (float val) => UpdateSize(val), 300f, 0.05f);
			};
		};

		fill.onComplete += () => SetColor(Color.white);
		fill.onComplete += () =>
		{
			filledParticles.transform.parent = transform.parent;
			filledParticles.Play();
		};

		fill.onComplete += onComplete;

		fill.onComplete += () => Destroy(gameObject, 0.15f);
	}
}
