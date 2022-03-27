using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Behavior: Spawn two orbs, after a short delay, throw them at player. They explode after reaching player position
public class GhastEnemy : MonoBehaviour
{
	[SerializeField] private GameObject ghastProjectile;
	[SerializeField] private CircleColliderInstance explosionCollider;
	[SerializeField] private Transform orbHolder;

	private Damager damager;
	private Transform player;

	private void Awake()
	{
		damager = GetComponent<Damager>();
	}

	private void Start()
	{
		player = PlayerController.instance.transform;
		StartCoroutine(GhastBehavior());
	}

	private IEnumerator GhastBehavior()
	{
		while (true)
		{
			yield return new WaitForSeconds(1f);

			// Spawn orbs
			GameObject orb1 = Instantiate(ghastProjectile, orbHolder, false);
			orb1.transform.localPosition = new Vector3(0, 3f, 0);

			yield return new WaitForSeconds(0.5f);

			GameObject orb2 = Instantiate(ghastProjectile, orbHolder, false);
			orb2.transform.localPosition = new Vector3(0, -3f, 0);

			yield return new WaitForSeconds(1.6f);

			// Shoot orbs

			// Minimum distance the orbs will travel before exploding
			float minDistance = 4f;


		}
	}
}
