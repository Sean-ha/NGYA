using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecuteAction : MonoBehaviour
{
	public static ExecuteAction instance;

	private void Awake()
	{
		instance = this;
	}

	public void Execute(float time, Action function)
	{
		StartCoroutine(ExecuteInTime(time, function));
	}

	private IEnumerator ExecuteInTime(float time, Action function)
	{
		yield return new WaitForSeconds(time);

		function.Invoke();
	}
}
