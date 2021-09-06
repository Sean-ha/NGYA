using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAbilityCooldownBar : MonoBehaviour
{
	private Transform myTransform;
	private SpecialAbilityManager sam;

	private void Awake()
	{
		myTransform = transform;
	}

	private void Start()
	{
		sam = SpecialAbilityManager.instance;
	}

	private void Update()
	{
		float yScale = sam.GetCooldownFraction();

		myTransform.localScale = new Vector3(1, yScale, 1);
	}
}
