using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Wave", order = 0)]
public class Wave : ScriptableObject
{
	public List<GameObject> spawns;
}
