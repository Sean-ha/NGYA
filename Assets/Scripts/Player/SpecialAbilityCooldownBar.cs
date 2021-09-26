using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAbilityCooldownBar : MonoBehaviour
{
	private Transform myTransform;
	private SpecialAbilityManager sam;

	private float maxScaleX;
	private float yScale;

	private void Awake()
	{
		myTransform = transform;
		maxScaleX = myTransform.localScale.x;
		yScale = myTransform.localScale.y;
	}

	private void Start()
	{
		sam = SpecialAbilityManager.instance;
	}

	private void Update()
	{
		float ratio = 1 - sam.GetCooldownFraction();

		myTransform.localScale = new Vector3(ratio * maxScaleX, yScale, 1);
	}
}