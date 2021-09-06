using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Place this component on a TextMesh object to give it a shadow
public class TextMeshShadowCreator : MonoBehaviour
{
	[Tooltip("Set this to true if this text object can and will change at some point (this includes changing text, changing font size, etc.)")]
	public bool dynamicText;

	private void Awake()
	{
		TextMeshPro myTextMesh = GetComponent<TextMeshPro>();

		GameObject shadow = Instantiate(GameAssets.instance.textMeshShadow, transform, false);
		// Set shadow properties to be identical to this text
		TextMeshPro textComponent = shadow.GetComponent<TextMeshPro>();
		textComponent.text = myTextMesh.text;
		textComponent.fontSize = myTextMesh.fontSize;
		textComponent.fontStyle = myTextMesh.fontStyle;
		textComponent.alignment = myTextMesh.alignment;
		textComponent.color = new Color(0, 0, 0, myTextMesh.color.a);
		textComponent.rectTransform.sizeDelta = myTextMesh.rectTransform.sizeDelta;

		if (dynamicText)
			shadow.AddComponent<TextMeshShadowUpdater>();

		textComponent.ForceMeshUpdate();
	}
}
