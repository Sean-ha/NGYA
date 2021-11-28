using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Uses a LineRenderer to draw an electricity effect
[RequireComponent(typeof(LineRenderer))]
public class ElectricityEffect : MonoBehaviour
{
	private LineRenderer lr;

	private void Awake()
	{
		lr = GetComponent<LineRenderer>();
	}

	// "startWidth" is the starting width of the LineRenderer
	// "time" is the time it takes for the electricity to fade away. If time = 0, then the electricity does not fade
	public void CreateElectricEffect(Vector2 from, Vector2 to, float time, float startWidth = 0.5f)
	{
		lr.widthMultiplier = startWidth;
		if (time > 0)
		{
			DOTween.To(() => lr.widthMultiplier, (float val) => lr.widthMultiplier = val, 0, time).OnComplete(() => gameObject.SetActive(false));
		}

		float distance = Vector2.Distance(from, to);

		Vector2 direction = to - from;
		Vector2 perp = Vector3.Cross(direction, Vector3.back).normalized;

		int minVertices = 1 + Mathf.FloorToInt(distance / 3);
		int maxVertices = 2 + Mathf.FloorToInt(distance / 2);

		int numVertices = Random.Range(minVertices, maxVertices + 1);

		// +2 to account for start and end positions
		lr.positionCount = numVertices + 2;

		lr.SetPosition(0, from);

		// To contain randomized distances between from and to (in order)
		float[] distances = new float[numVertices + 1];
		distances[0] = 0f;
		for (int i = 1; i < distances.Length; i++)
		{
			distances[i] = distances[i - 1] + Random.Range(1f / (numVertices + 4), 1f / (numVertices + 1));
		}

		// Generate random points between from and to
		for (int i = 1; i < lr.positionCount - 1; i++)
		{
			Vector2 randomLoc = from + distances[i] * direction;
			// Offset that point randomly in a direction perpendicular to the angle between from and to
			randomLoc += perp * Random.Range(-1f, 1f);

			lr.SetPosition(i, randomLoc);
		}

		lr.SetPosition(lr.positionCount - 1, to);
	}
}
