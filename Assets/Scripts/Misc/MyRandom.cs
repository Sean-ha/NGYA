using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MyRandom : MonoBehaviour
{
	// Given a probability p, there is a p chance of returning true, and 1-p chance of returning false
	public static bool RollProbability(float probability)
	{
		if (probability == 0)
			return false;

		float roll = Random.Range(0f, 1f);

		if (roll >= probability)
		{
			return false;
		}

		return true;
	}

   public static List<int> GenerateRandomValues(int count, int min, int max)
   {
      if (max <= min || count < 0 ||
              (count > max - min && max - min > 0))
      {
         Debug.LogError("Range " + min + " to " + max +
                 " (" + (max - min) + " values), or count " + count + " is illegal");
      }

      HashSet<int> candidates = new HashSet<int>();

      for (int top = max - count; top < max; top++)
      {
         if (!candidates.Add(Random.Range(min, top + 1)))
         {
            candidates.Add(top);
         }
      }

      List<int> result = candidates.ToList();

      for (int i = result.Count - 1; i > 0; i--)
      {
         int k = Random.Range(0, i + 1);
         int tmp = result[k];
         result[k] = result[i];
         result[i] = tmp;
      }
      return result;
   }
}
