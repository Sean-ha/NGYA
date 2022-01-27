using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SplitterEnemy : MonoBehaviour
{
	public Transform smallSplitterHolder;
	public GameObject splitParticles;
	[Tooltip("Ratio of parent's max health that children should have")]
	public float maxHealthMutliplier;
	[Tooltip("Ratio of parent's damage that children should deal")]
	public float damageMultiplier;

	private EnemyHealth myHealth;
	private Damager myDamager;

	private void Awake()
	{
		myHealth = GetComponent<EnemyHealth>();
		myDamager = GetComponent<Damager>();
	}

	// Releases the children objects and then destroys this parent object
	public void SplitThenDie()
	{
		ObjectPooler.instance.CreateExpandingExplosion(transform.position, Color.red, 2f);
		splitParticles.transform.parent = null;
		splitParticles.SetActive(true);
		gameObject.SetActive(false);

		// Set all children active, unparent them, and set their initial stats properly
		while (smallSplitterHolder.childCount != 0)
		{
			Transform child = smallSplitterHolder.GetChild(0);

			child.gameObject.SetActive(true);
			child.transform.parent = null;
			EnemyHealth childHealth = child.GetComponentInChildren<EnemyHealth>();
			childHealth.maxHealth = myHealth.maxHealth * maxHealthMutliplier;
			childHealth.SetCurrHealth();

			child.GetComponentInChildren<Damager>().damage = myDamager.damage * damageMultiplier;
		}

		// Destroy this parent object now
		Destroy(transform.parent.gameObject);
	}
}
