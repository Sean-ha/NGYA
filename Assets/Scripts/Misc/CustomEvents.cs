using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomEvents : MonoBehaviour
{
	public List<UnityEvent> eventList;
	
	public void InvokeEvent(int which)
	{
#if UNITY_EDITOR
		if (which >= eventList.Count)
			Debug.LogError("Invalid number");
#endif

		eventList[which].Invoke();
	}
}
