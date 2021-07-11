using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotBuilder : MonoBehaviour
{
	public Transform dotTemplate;
	public int dotCount;
	private ShootManager sm;

	private float dotAngle = 20f;

	private void BuildDots()
	{
		sm = FindObjectOfType<ShootManager>();
		foreach (Transform child in transform)
			Destroy(child.gameObject);

		float distance = 0.6f;
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
			sm.onShoot.AddListener(newDot.GetComponent<Shooter>().Shoot);
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			BuildDots();
		}
	}
}
