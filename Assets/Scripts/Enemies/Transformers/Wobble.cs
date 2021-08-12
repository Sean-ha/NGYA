using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wobble : MonoBehaviour
{
	[Tooltip("Time it takes to rotate from middle position to one side")]
	public float timeToRotate;
	[Tooltip("Maximum angle to rotate on either side")]
	public float maxAngle;
}
