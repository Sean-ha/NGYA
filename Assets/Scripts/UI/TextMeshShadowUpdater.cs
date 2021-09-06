using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Added to dynamic text mesh shadows.
public class TextMeshShadowUpdater : MonoBehaviour
{
	private TextMeshPro parentText;
	private TextMeshPro myText;

	private void Awake()
	{
		parentText = transform.parent.GetComponent<TextMeshPro>();
		myText = GetComponent<TextMeshPro>();
	}

	private void LateUpdate()
	{
		// Currently only dynamically updates text field and alpha
		myText.text = parentText.text;
		myText.color = new Color(0, 0, 0, parentText.color.a);

		myText.ForceMeshUpdate();
	}
}
