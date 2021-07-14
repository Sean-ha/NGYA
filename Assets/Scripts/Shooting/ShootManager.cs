using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShootManager : MonoBehaviour
{
	public Transform dotHolder;

	public float damage;
	public float bulletDistance;
	public int pierceCount;

	private float shootCooldown = 0.25f;
	private float currShootCooldown;

	// Parameters: damage, distance, pierce
	public UnityEvent<float, float, int> onShoot { get; set; } = new UnityEvent<float, float, int>();

	private void Awake()
	{
		foreach (Transform child in dotHolder)
		{
			Shooter currShooter = child.GetComponent<Shooter>();
			if (currShooter != null)
				onShoot.AddListener(currShooter.Shoot);
		}
	}

	private void Update()
	{
		currShootCooldown -= Time.deltaTime;

		if (Input.GetMouseButton(0))
		{
			CameraShake.instance.ShakeCamera(0.06f, 0.06f);

			if (currShootCooldown <= 0)
			{
				// Shoot
				onShoot.Invoke(damage, bulletDistance, pierceCount);
				currShootCooldown = shootCooldown;
			}
		}
	}
}
