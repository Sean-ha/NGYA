using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float moveSpeed;

	private HealthSystem healthSystem;
	private Rigidbody2D rb;

	private float horizontal;
	private float vertical;

	private void Awake()
	{
		// TODO: Move this somewhere else probably
		Application.targetFrameRate = 60;

		rb = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		healthSystem = HealthSystem.instance;
	}

	private void Update()
	{
		horizontal = Input.GetAxis("Horizontal");
		vertical = Input.GetAxis("Vertical");
	}

	private void FixedUpdate()
	{
		rb.velocity = new Vector2(horizontal, vertical) * moveSpeed;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.layer == 7)
		{
			healthSystem.TakeDamage(10);
		}
	}
}
