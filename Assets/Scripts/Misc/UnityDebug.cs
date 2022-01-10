using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityDebug : MonoBehaviour
{
	private void Update()
	{
#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.LeftBracket))
		{
			UnityEditor.EditorWindow.focusedWindow.maximized = !UnityEditor.EditorWindow.focusedWindow.maximized;
		}
#endif
	}
}
