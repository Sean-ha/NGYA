using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEditor;
using System.IO;

[CreateAssetMenu(menuName = "ScriptableObjects/Upgrade", order = 1)]
public class UpgradeObject : ScriptableObject
{
#if UNITY_EDITOR
	[OnValueChanged(nameof(ChangeUpgradeName))]
#endif
	public Upgrade upgradeType;
	public string upgradeName;
	[ShowAssetPreview]
	public Sprite upgradeSprite;

	[Tooltip("The description of the upgrade for each level. Length of list defines max # of this upgrade that can be obtained")]
	[TextArea(2, 5)]
	public string[] upgradeDescriptions;

	[ReadOnly]
	public string debug;

#if UNITY_EDITOR
	// Set name of asset to be based on the upgradeType
	private void ChangeUpgradeName()
	{
		string assetPath = AssetDatabase.GetAssetPath(GetInstanceID());
		debug = AssetDatabase.RenameAsset(assetPath, (int)upgradeType + upgradeType.ToString());
	}
#endif

	/*
	[Tooltip("Text description of item when it's first picked up")]
	[TextArea(2, 5)]
	public string upgradeDescriptionFirst;

	[Tooltip("Text description of item after it's already picked up")]
	[TextArea(2, 5)]
	public string upgradeDescriptionSecond;

	[Tooltip("Total number of this upgrade that the user can possibly acquire")]
	public int maxCount;
	*/
}