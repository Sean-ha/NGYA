using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperEnemy : MonoBehaviour
{
	public Transform laserSight;
	public ParticleSystem shootParticles;
	public EnemyDotShooterForward dotShooter;

	private PointToPlayer point;

	private void Start()
	{
		StartCoroutine(Behavior());
		point = GetComponent<PointToPlayer>();
	}

	private IEnumerator Behavior()
	{
		while(true)
		{
			yield return new WaitForSeconds(2f);

			// Lock rotation and disable laser sight
			laserSight.gameObject.SetActive(false);
			point.enabled = false;

			yield return new WaitForSeconds(0.9f);

			// Shoot
			CameraShake.instance.ShakeCamera(0.15f, 0.2f);
			dotShooter.Shoot();
			float rangle = transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
			Vector2 force = new Vector2(Mathf.Cos(rangle), Mathf.Sin(rangle)) * -4000f;
			GetComponent<Rigidbody2D>().AddForce(force);
			
			yield return new WaitForSeconds(1.5f);

			// Enable laser sight and unlock rotation
			point.enabled = true;
			yield return null;
			yield return null;
			laserSight.gameObject.SetActive(true);
			point.ForceUpdate();
			
		}
	}
}
