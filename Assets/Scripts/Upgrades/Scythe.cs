using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scythe : MonoBehaviour
{
	public ParticleSystem createParticles;


	private Animator animator;

	private void Awake()
	{
		animator = GetComponent<Animator>();
	}

	public void CreateScythe()
	{

	}
}
