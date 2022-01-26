using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
	private SoundManager sm;

	private void Start()
	{
		sm = SoundManager.instance;
	}

	public void PlaySound(SoundManager.Sound sound)
	{
		sm.PlaySound(sound);
	}

	public void PlaySound(int sound)
	{
		sm.PlaySound((SoundManager.Sound)sound);
	}

	public void PlaySoundNoPitchAlteration(int sound)
	{
		sm.PlaySound((SoundManager.Sound)sound, randomizePitch: false);
	}
}
