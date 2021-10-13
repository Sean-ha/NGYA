using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameAssets : MonoBehaviour
{
   public static GameAssets instance;

   public static bool LoadingDone;

   private void Awake()
   {
      instance = this;

      InitializeDictionaries();
      blueColorHex = ColorUtility.ToHtmlStringRGB(blueColor);
   }

	private void InitializeDictionaries()
	{
      enemyDict = new Dictionary<EnemyType, GameObject>();
      upgradeDict = new Dictionary<Upgrade, UpgradeObject>();

      foreach (EnemyPair pair in enemyList)
		{
         enemyDict[pair.enemyType] = pair.enemy;
		}

      LoadAssets();
	}

   public async void LoadAssets()
	{
      // Load UpgradeObjects ScriptableObjects from Assets and use them to construct a dictionary
      await Addressables.LoadAssetsAsync("UpgradeObjects", (UpgradeObject loaded) =>
      {
         upgradeDict.Add(loaded.upgradeType, loaded);
      }).Task;
      LoadingDone = true;
   }

	[System.Serializable]
   public class EnemyPair
	{
      public EnemyType enemyType;
      public GameObject enemy;
	}

   public Color blueColor { get; set; } = new Color(0, 200f / 255f, 1);
   public string blueColorHex { get; set; }

   public List<EnemyPair> enemyList;

   private Dictionary<EnemyType, GameObject> enemyDict;

   public HashSet<Upgrade> currentUpgrades { get; set; }

   public Dictionary<Upgrade, UpgradeObject> upgradeDict { get; set; }

   public GameObject blowbackExplosion;

   public GameObject textMeshShadow;
}