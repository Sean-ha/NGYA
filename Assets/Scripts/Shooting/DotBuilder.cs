using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class DotBuilder : MonoBehaviour
{
	public static DotBuilder instance;

	public Transform dotTemplate;
	public int dotCount;

	// These two fields are mutually exclusive. Only have 1 enabled, but must have 1 enabled!!
	public bool angledDots;
	[EnableIf("angledDots")]
	public float dotAngle = 20f;

	public bool straightDots;
	[EnableIf("straightDots")]
	public float dotDistance = 0.1f;

	private const float distance = 0.6f;

	private void Awake()
	{
		instance = this;
	}

	private void BuildDots()
	{
		if (angledDots)
			BuildAngledDots();
		else if (straightDots)
			BuildStraightDots();
	}

	private void BuildStraightDots()
	{
		foreach (Transform child in transform)
			Destroy(child.gameObject);

		Vector2 startingDotPos;
		float distanceFromCenter = 0.6f;
		if (dotCount % 2 == 0)
		{
			startingDotPos = new Vector2(distanceFromCenter, -(((dotCount / 2) - 1) * dotDistance + (dotDistance / 2f)));
		}
		else
		{
			startingDotPos = new Vector2(distanceFromCenter, -(dotCount / 2) * dotDistance);
		}

		for (int i = 0; i < dotCount; i++)
		{
			Transform newDot = Instantiate(dotTemplate, transform, false);
			newDot.GetComponent<DotShooter>().angledBullets = angledDots;
			newDot.localPosition = startingDotPos;
			startingDotPos = new Vector2(startingDotPos.x, startingDotPos.y + dotDistance);
			ShootManager.instance.onShoot.AddListener(newDot.GetComponent<Shooter>().Shoot);
		}
	}

	private void BuildAngledDots()
	{
		foreach (Transform child in transform)
			Destroy(child.gameObject);

		float currAngle;
		if (dotCount % 2 == 0)
		{
			currAngle = -(((dotCount / 2) - 1) * dotAngle + (dotAngle / 2));
		}
		else
		{
			currAngle = -(dotCount / 2) * dotAngle;
		}

		for (int i = 0; i < dotCount; i++)
		{
			Transform newDot = Instantiate(dotTemplate, transform, false);
			newDot.localPosition = new Vector3(Mathf.Cos(currAngle * Mathf.Deg2Rad), Mathf.Sin(currAngle * Mathf.Deg2Rad)) * distance;
			currAngle += dotAngle;
			ShootManager.instance.onShoot.AddListener(newDot.GetComponent<Shooter>().Shoot);
		}
	}

	private void Update()
	{
#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.R))
		{
			BuildDots();
		}
#endif
	}
}
