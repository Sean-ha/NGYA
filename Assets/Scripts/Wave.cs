using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Wave", order = 0)]
public class Wave : ScriptableObject
{
	public List<GameObject> spawns;

#if UNITY_EDITOR
	public float expTotal;
	[Tooltip("Does nothing but it refreshes expTotal to be the correct value")]
	public bool refreshExpTotal;

	private void OnValidate()
	{
		expTotal = 0;
		foreach(GameObject spawn in spawns)
		{
			ExpDropper[] expDropList = spawn.GetComponentsInChildren<ExpDropper>();
			foreach (ExpDropper dropper in expDropList)
			{
				expTotal += dropper.GetComponent<ExpDropper>().expToDrop;
			}
		}
	}
#endif
}
