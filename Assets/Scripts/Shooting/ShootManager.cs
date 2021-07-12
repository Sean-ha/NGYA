using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShootManager : MonoBehaviour
{
	public Transform dotHolder;

	private float shootCooldown = 0.25f;
	private float currShootCooldown;

	public UnityEvent onShoot { get; set; } = new UnityEvent();

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
			CameraShake.instance.ShakeCamera(0.1f, 0.05f);
			if (currShootCooldown <= 0)
			{
				// Shoot
				onShoot.Invoke();
				currShootCooldown = shootCooldown;
			}
		}
	}
}
