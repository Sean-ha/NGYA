using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class UpgradeWindow : MonoBehaviour
{
	public Transform topLeft;
	public Transform topRight;
	public Transform bottomLeft;
	public Transform bottomRight;
	public Transform center;

	private SpriteRenderer centerSR;
	private Vector2 defaultCenterSize;

	// X and Y positions for the corners (so top left would be (-cornerX, cornerY))
	private float cornerX = 16f;
	private float cornerY = 8f;
	private Vector2 initialPos = new Vector2(0f, -14f);

	private void Awake()
	{
		centerSR = center.GetComponent<SpriteRenderer>();
		defaultCenterSize = centerSR.size;
	}

	// Animate the showing of the upgrade window
	public void DisplayUpgradeWindow(Action onComplete = null)
	{
		StartCoroutine(AnimateDisplayUpgradeWindow(onComplete));
	}

	private IEnumerator AnimateDisplayUpgradeWindow(Action onComplete)
	{
		transform.position = initialPos;

		transform.DOMoveY(0, 0.4f).SetEase(Ease.InOutQuad).SetUpdate(true);

		yield return new WaitForSecondsRealtime(0.45f);

		// Randomize window's rotation to add visual interest
		float randomRotation = UnityEngine.Random.Range(1.3f, 2.6f) * MyRandom.FlipCoin();
		transform.DORotate(new Vector3(0, 0, randomRotation), 0.2f).SetEase(Ease.InOutQuad).SetUpdate(true);

		yield return new WaitForSecondsRealtime(0.26f);

		// Expand window: move the 4 corners while scaling up the fill
		float expandDur = 0.35f;
		// Tween x vals
		DOTween.To(() => 0f, (float val) =>
		{
			topLeft.localPosition = new Vector3(-val, topLeft.localPosition.y, topLeft.localPosition.z);
			topRight.localPosition = new Vector3(val, topRight.localPosition.y, topRight.localPosition.z);
			bottomLeft.localPosition = new Vector3(-val, bottomLeft.localPosition.y, bottomLeft.localPosition.z);
			bottomRight.localPosition = new Vector3(val, bottomRight.localPosition.y, bottomRight.localPosition.z);
			center.localScale = new Vector3(((val * 2 + 2) / 1.8125f) - 0.1f, center.localScale.y, center.localScale.z);
		}, cornerX, expandDur).SetEase(Ease.InOutQuad).SetUpdate(true);
		// Tween y vals
		DOTween.To(() => 0f, (float val) =>
		{
			topLeft.localPosition = new Vector3(topLeft.localPosition.x, val, topLeft.localPosition.z);
			topRight.localPosition = new Vector3(topRight.localPosition.x, val, topRight.localPosition.z);
			bottomLeft.localPosition = new Vector3(bottomLeft.localPosition.x, -val, bottomLeft.localPosition.z);
			bottomRight.localPosition = new Vector3(bottomRight.localPosition.x, -val, bottomRight.localPosition.z);
			center.localScale = new Vector3(center.localScale.x, ((val * 2 + 2) / 1.8125f) - 0.1f, center.localScale.z);
		}, cornerY, expandDur).SetEase(Ease.InOutQuad).SetUpdate(true).OnComplete(() =>
		{
			if (onComplete != null)
				onComplete.Invoke();
		});

		yield return new WaitForSecondsRealtime(0.06f);
		center.gameObject.SetActive(false);
		yield return new WaitForSecondsRealtime(0.06f);
		center.gameObject.SetActive(true);
		yield return new WaitForSecondsRealtime(0.06f);
		center.gameObject.SetActive(false);
		yield return new WaitForSecondsRealtime(0.06f);
		center.gameObject.SetActive(true);
	}

	public void HideUpgradeWindow(Action onComplete = null)
	{
		StartCoroutine(AnimateHideUpgradeWindow(onComplete));
	}

	// Animate the hiding of the upgrade window
	private IEnumerator AnimateHideUpgradeWindow(Action onComplete)
	{
		// Shrink window: move the 4 corners while scaling down the fill
		float expandDur = 0.35f;
		// Tween x vals
		DOTween.To(() => cornerX, (float val) =>
		{
			topLeft.localPosition = new Vector3(-val, topLeft.localPosition.y, topLeft.localPosition.z);
			topRight.localPosition = new Vector3(val, topRight.localPosition.y, topRight.localPosition.z);
			bottomLeft.localPosition = new Vector3(-val, bottomLeft.localPosition.y, bottomLeft.localPosition.z);
			bottomRight.localPosition = new Vector3(val, bottomRight.localPosition.y, bottomRight.localPosition.z);
			center.localScale = new Vector3(((val * 2 + 2) / 1.8125f) - 0.1f, center.localScale.y, center.localScale.z);
		}, 0, expandDur).SetEase(Ease.InOutQuad).SetUpdate(true);
		// Tween y vals
		DOTween.To(() => cornerY, (float val) =>
		{
			topLeft.localPosition = new Vector3(topLeft.localPosition.x, val, topLeft.localPosition.z);
			topRight.localPosition = new Vector3(topRight.localPosition.x, val, topRight.localPosition.z);
			bottomLeft.localPosition = new Vector3(bottomLeft.localPosition.x, -val, bottomLeft.localPosition.z);
			bottomRight.localPosition = new Vector3(bottomRight.localPosition.x, -val, bottomRight.localPosition.z);
			center.localScale = new Vector3(center.localScale.x, ((val * 2 + 2) / 1.8125f) - 0.1f, center.localScale.z);
		}, 0, expandDur).SetEase(Ease.InOutQuad).SetUpdate(true);

		yield return new WaitForSecondsRealtime(0.4f);

		transform.DORotate(new Vector3(0, 0, 45), 0.2f).SetEase(Ease.InOutQuad).SetUpdate(true);

		yield return new WaitForSecondsRealtime(0.25f);

		transform.DOMoveY(initialPos.y, 0.4f).SetEase(Ease.InOutQuad).SetUpdate(true).OnComplete(() =>
		{
			if (onComplete != null)
				onComplete.Invoke();
		});
	}
}
