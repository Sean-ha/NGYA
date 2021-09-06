using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotBuilder : MonoBehaviour
{
	public static DotBuilder instance;

	public Transform dotTemplate;
	public int dotCount;

	private BuddyShooter buddyShooter;

	private float dotAngle = 20f;

	private const float distance = 0.6f;

	private void Awake()
	{
		instance = this;
		buddyShooter = FindObjectOfType<BuddyShooter>(true);
	}

	private void BuildDots()
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

	public void BuildBackboneDot()
	{
		Transform newDot = Instantiate(dotTemplate, transform, false);
		newDot.localPosition = new Vector3(-1, 0) * distance;
		Shooter shooter = newDot.GetComponent<Shooter>();
		shooter.ammoPerShot = 0;
		ShootManager.instance.onShoot.AddListener(shooter.Shoot);

		ObjectPooler.instance.CreateHitParticles(Color.white, newDot.position);
		ObjectPooler.instance.CreateCircleHitEffect(Color.white, newDot.position, 0.4f);
	}

	/// <summary>
	/// Change amount of bullets each shooter consumes. adjustmentValue is the amount to be added.
	/// </summary>
	/// <param name="adjustmentValue"></param>
	public void ChangeBulletAmmoCost(int adjustmentValue)
	{
		Shooter[] shooters = GetComponentsInChildren<Shooter>();
		foreach (Shooter shooter in shooters)
		{
			if (shooter.ammoPerShot != 0)
				shooter.ammoPerShot += adjustmentValue;
		}
		buddyShooter.ammoPerShot += adjustmentValue;
	}

	public void ChangeBulletAmmoConsumptionChance(float adjustmentValue)
	{
		Shooter[] shooters = GetComponentsInChildren<Shooter>();
		foreach (Shooter shooter in shooters)
		{
			shooter.ChanceToNotConsumeAmmo += adjustmentValue;
		}
		buddyShooter.ChanceToNotConsumeAmmo += adjustmentValue;
	}
}
