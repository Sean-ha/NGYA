using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
   public static GameAssets instance;

   private void Awake()
   {
      instance = this;

      InitializeDictionaries();
   }

   private void InitializeDictionaries()
	{
      enemyDict = new Dictionary<EnemyType, GameObject>();
      foreach (EnemyPair pair in enemyList)
		{
         enemyDict[pair.enemyType] = pair.enemy;
		}
	}

   
   [System.Serializable]
   public class EnemyPair
	{
      public EnemyType enemyType;
      public GameObject enemy;
	}

   public List<EnemyPair> enemyList;

   private Dictionary<EnemyType, GameObject> enemyDict;
}