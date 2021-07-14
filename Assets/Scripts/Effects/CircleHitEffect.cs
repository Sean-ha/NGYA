using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleHitEffect : MonoBehaviour
{
	public float maxScale;
	public float timeToScale = 0.1f;

	private void OnEnable()
	{
		LeanTween.cancel(gameObject);
		transform.localScale = new Vector3(maxScale, maxScale, 1);
		LeanTween.scale(gameObject, new Vector3(0, 0, 1), timeToScale).setOnComplete(() => gameObject.SetActive(false));
	}
}
