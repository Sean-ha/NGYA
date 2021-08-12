using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GeneralTests
{
	// UNIT TESTS NOT FUNCTIONAL ATM. MORE RESEARCH REQUIRED.
	// Ensures the sound manager dictionary contains all sound enums, none of them have 0 instances, and there are no repeats.
	[Test]
	public void TestSoundManagerList()
	{
		GameObject newObj = new GameObject();

		SoundManager soundManager = newObj.AddComponent<SoundManager>();
		soundManager.Call("Awake");

		HashSet<SoundManager.Sound> soundSet = new HashSet<SoundManager.Sound>();
		foreach (SoundManager.SoundAudioClip clip in SoundManager.instance.sounds)
		{
			Assert.IsFalse(soundSet.Contains(clip.sound));

			soundSet.Add(clip.sound);

			Assert.AreNotEqual(0, clip.quantity);
		}

		foreach (SoundManager.Sound sound in Enum.GetValues(typeof(SoundManager.Sound)))
		{
			Assert.IsTrue(soundSet.Contains(sound));
		}
	}
}
