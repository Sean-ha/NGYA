using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BomberEnemy : MonoBehaviour
{
	public GameObject bombEnemy;
	[Tooltip("Time it takes for the bomb to reach its destination")]
	public float timeToReach;
	public float bombDamage;

	// Position the bomb should spawn relative to the big bomber body
	private Vector2 localSpawnPos = new Vector2(-0.035f, 0.53f);

	public void CreateBomb()
	{
		GameObject bomb = Instantiate(bombEnemy, transform, false);
		bomb.transform.localPosition = localSpawnPos;
		bomb.transform.parent = null;

		Vector2 destination = PlayerController.instance.transform.position;
		bomb.GetComponent<BombEnemyProjectile>().SetProjectile(destination, timeToReach, bombDamage);
	}
}
