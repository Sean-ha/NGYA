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

	[Tooltip("Text description of item when it's first picked up")]
	[TextArea(1, 5)]
	public string upgradeDescriptionFirst;

	[Tooltip("Text description of item after it's already picked up")]
	[TextArea(1, 5)]
	public string upgradeDescriptionSecond;

	[Tooltip("Total number of this upgrade that the user can possibly acquire")]
	public int maxNumber;
}
