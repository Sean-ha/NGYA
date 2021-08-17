using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserProjectileWallCollision : MonoBehaviour
{
	private BoxCollider2D col;
	private int wallLayer;

	private void Awake()
	{
		col = GetComponent<BoxCollider2D>();
		wallLayer = 1 << 9;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// Wall layer
		if (collision.gameObject.layer == 9)
		{
			float leftPos = col.offset.x - (col.size.x / 2f);
			float mid = col.offset.y;
			Vector2 leftMostPos = transform.TransformPoint(new Vector2(leftPos, mid));
			float rangle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
			Vector2 dir = new Vector3(Mathf.Cos(rangle), Mathf.Sin(rangle), 0);

			RaycastHit2D hit = Physics2D.Raycast(leftMostPos, dir, 30f, wallLayer);
			if (hit)
			{
				ObjectPooler.instance.CreateHitParticles(Color.white, hit.point);
				ObjectPooler.instance.CreateCircleHitEffect(Color.white, hit.point, 1.5f);
			}
		}
	}
}
