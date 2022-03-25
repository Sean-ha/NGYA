using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new 3d object", menuName = "StackObject")]
public class StackObject : ScriptableObject
{
   public List<Sprite> stack;
}