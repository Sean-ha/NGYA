using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	public int perfectTime;

	private float cooldown = 0.5f;
	private float currCooldown = 0f;

	private bool isHit = false;
	// Number of frames since you were hit
	private int frameCount = 0;

	private GameObject proj;

	private void Awake()
	{
		Application.targetFrameRate = 60;
	}

	private void Update()
	{
		currCooldown -= Time.deltaTime;

		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (currCooldown <= 0)
			{
				currCooldown = cooldown;
				if (isHit)
				{
					if (frameCount < perfectTime)
					{
						// successful perfect block
						print("success");
						Destroy(proj);
						currCooldown = 0;
					}
				}
			}
		}

		if (isHit)
		{
			if (frameCount >= perfectTime)
			{
				print("fail");
			}
		}
		frameCount++;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		proj = collision.gameObject;
		isHit = true;
		frameCount = 0;
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		isHit = false;
	}
}
