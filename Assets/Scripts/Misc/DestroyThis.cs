using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyThis : MonoBehaviour
{
	public void DestroyThisObject()
	{
		Destroy(gameObject);
	}

	public void DestroyParentObject()
	{
		Destroy(transform.parent.gameObject);
	}

	// Waits 'timeToWait' seconds, and then destroys this gameObject
	public void DestroyThisObject(float timeToWait)
	{
		StartCoroutine(WaitThenDestroy(timeToWait));
	}

	private IEnumerator WaitThenDestroy(float timeToWait)
	{
		yield return new WaitForSeconds(timeToWait);
		Destroy(gameObject);
	}
}
