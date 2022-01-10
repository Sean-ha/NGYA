using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class ColliderOnClick : MonoBehaviour
{
	public UnityEvent onClick;

	private void OnMouseDown()
	{
		onClick.Invoke();
	}

	// TEMP!!! DELETE ME
	public void GoToPlay()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene("Battle");
	}
}
