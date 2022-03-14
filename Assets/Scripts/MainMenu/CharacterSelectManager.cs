using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterSelectManager : MonoBehaviour
{
	public GameObject characterTemplate;
	public TextMeshPro characterName, characterDescription;

	public List<PlayableCharacter.Character> availableCharacters;

	private List<GameObject> characterList = new List<GameObject>();

	private void Start()
	{
		BuildCharacterSelect();
	}

	private void BuildCharacterSelect()
	{
		float distance = 5f;
		Vector2 currPos = Vector2.zero;

		for (int i = 0; i < availableCharacters.Count; i++)
		{
			GameObject created = Instantiate(characterTemplate, characterTemplate.transform.parent, false);
			created.transform.localPosition = currPos;
			characterList.Add(created);
			currPos.x += distance;

			created.GetComponent<CharacterData>().character = availableCharacters[i];
		}
		Destroy(characterTemplate);
	}
}
