using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyRandom : MonoBehaviour
{
	// Given a probability p, there is a p chance of returning true, and 1-p chance of returning false
	public static bool RollProbability(float probability)
	{
		float roll = Random.Range(0f, 1f);

		if (roll >= probability)
		{
			return false;
		}

		return true;
	}
}
