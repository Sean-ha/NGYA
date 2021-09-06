using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class ShootManager : MonoBehaviour
{
	public static ShootManager instance;

	public Transform dotHolder;

	public float damage;

	private float bulletDistance = 6.5f;
	public float BulletDistance 
	{ 
		get { return bulletDistance; }
		set { bulletDistance = Mathf.Max(1, value); }
	}

	public int pierceCount;

	private float shootCooldown = 0.25f;
	public float ShootCooldown
	{
		get { return shootCooldown; }
		set { shootCooldown = Mathf.Max(0.01f, value); }
	}

	private float currShootCooldown;

	// Parameters: damage, distance, pierce
	public UnityEvent<float, float, int> onShoot { get; set; } = new UnityEvent<float, float, int>();

	private void Awake()
	{
		instance = this;
		foreach (Transform child in dotHolder)
		{
			Shooter currShooter = child.GetComponent<Shooter>();
			if (currShooter != null)
				onShoot.AddListener(currShooter.Shoot);
		}
	}

	private void Update()
	{
		if (TimeManager.IsPaused) return;

		currShootCooldown -= Time.deltaTime;

		if (Input.GetMouseButton(0))
		{
			if (currShootCooldown <= 0)
			{
				onShoot.Invoke(damage, bulletDistance, pierceCount);
				currShootCooldown = shootCooldown;
			}
		}
	}
}
