using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameAssets : MonoBehaviour
{
   public static GameAssets instance;

   public static bool LoadingDone = false;

   private void Awake()
   {
      instance = this;

      InitializeDictionaries();
      blueColorHex = ColorUtility.ToHtmlStringRGB(blueColor);
   }

	private void InitializeDictionaries()
	{
      LoadingDone = false;
      enemyDict = new Dictionary<EnemyType, GameObject>();
      upgradeDict = new Dictionary<Upgrade_OLD2, UpgradeObject>();

      foreach (EnemyPair pair in enemyList)
		{
         enemyDict[pair.enemyType] = pair.enemy;
		}

      LoadAssets();
	}

   public async void LoadAssets()
	{
      print("loadingStart");
      // Load UpgradeObjects ScriptableObjects from Assets and use them to construct a dictionary
      await Addressables.LoadAssetsAsync("UpgradeObjects", (UpgradeObject loaded) =>
      {
         upgradeDict.Add(loaded.upgradeType, loaded);
      }).Task;
      LoadingDone = true;
   }

   public async void LoadAssetsWebGL()
	{
      print("webgl");
      var handle = Addressables.LoadAssetsAsync("UpgradeObjects", (UpgradeObject loaded) =>
      {
         print(loaded.upgradeName);
         upgradeDict.Add(loaded.upgradeType, loaded);
      });
      var go = await handle;
      foreach (UpgradeObject o in go)
		{
         print(o);
         if (!upgradeDict.ContainsKey(o.upgradeType))
            upgradeDict.Add(o.upgradeType, o);
		}
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

   public Color critColor;

   public List<EnemyPair> enemyList;

   private Dictionary<EnemyType, GameObject> enemyDict;

   public HashSet<Upgrade_OLD2> currentUpgrades { get; set; }

   public Dictionary<Upgrade_OLD2, UpgradeObject> upgradeDict { get; set; }

   public GameObject blowbackExplosion;

   public GameObject textMeshShadow;
}