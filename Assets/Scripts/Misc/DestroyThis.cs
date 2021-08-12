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
}
