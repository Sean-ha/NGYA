using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ParticleSystemStopCallback : MonoBehaviour
{
	public UnityEvent onStop;

	public void OnParticleSystemStop()
	{
		onStop.Invoke();
	}
}
