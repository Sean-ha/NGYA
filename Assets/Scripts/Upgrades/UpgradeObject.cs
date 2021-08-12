using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(menuName = "ScriptableObjects/Upgrade", order = 1)]
public class UpgradeObject : ScriptableObject
{
	public Upgrade upgradeType;
	public string upgradeName;
	[ShowAssetPreview]
	public Sprite upgradeSprite;
	[TextArea(1, 5)]
	public string upgradeDescription;
}
