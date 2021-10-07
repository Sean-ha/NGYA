using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BezierCurve : MonoBehaviour
{
	private LineRenderer myLR;

	private void Awake()
	{
		myLR = GetComponent<LineRenderer>();
	}

	private void OnEnable()
	{
		// Reset this LineRenderer to default
		DOTween.Kill(gameObject);
		myLR.widthMultiplier = 0.7f;
	}

	private Vector2 Bezier(Vector2 a, Vector2 b, float t)
	{
		return Vector2.Lerp(a, b, t);
	}

	public Vector2 Bezier(Vector2 a, Vector2 b, Vector2 c, float t)
	{
		return Vector2.Lerp(Bezier(a, b, t), Bezier(b, c, t), t);
	}

	public void SetBezierCurve(Vector2 a, Vector2 b, Vector2 c, int pointCount)
	{
		myLR.positionCount = pointCount;
		for (int i = 0; i < pointCount; i++)
		{
			Vector2 point = Bezier(a, b, c, i / (float)(pointCount - 1));
			myLR.SetPosition(i, point);
		}

		DOTween.To(() => myLR.widthMultiplier, (float val) => myLR.widthMultiplier = val, 0, 0.15f).OnComplete(() => gameObject.SetActive(false));
	}
}
