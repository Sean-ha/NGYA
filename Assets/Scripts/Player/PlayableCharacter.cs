using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PlayableCharacter", order = 2)]
public class PlayableCharacter : ScriptableObject
{
	public enum Character
	{
		Jimmy = 0,
		Timmy = 1,
	}

	public Character character;
	public string displayName;
	public string title;

	[Tooltip("This character's specific upgrades")]
	public List<Upgrade> specificUpgrades;

	// This is subject to change, might not be the best way to handle super upgrades
	public List<Upgrade> superUpgrades;
}
