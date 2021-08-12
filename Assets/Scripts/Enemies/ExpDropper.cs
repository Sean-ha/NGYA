using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpDropper : MonoBehaviour
{
	public int expToDrop;

	public void DropExp()
	{
		for (int i = 0; i < expToDrop; i++)
		{
			ObjectPooler.instance.Create(Tag.SmallExpShell, transform.position, Quaternion.identity);
		}
	}
}
