using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
   public static SoundManager instance;

   [System.Serializable]
   public enum Sound
   {
      PlayerShoot = 0,
      EnemyHit = 1,
      PlayerHit = 2,
      EnemyDeath = 3,
      PickupExp = 4,
      ClickySound = 5,
      LevelUp = 6,
      LaserCharge = 7,
      LaserShoot = 8,
      ShieldHit = 9,
      ShieldReady = 10,
      EnterField = 11,
      ExitField = 12,
      BalloonPop = 13,
      BasicEnemyShoot = 14,
      Explosion1 = 15,
      Whoosh = 16,
      Blip = 17,
      Select = 18,
      // EnemySpawn = 19,
      AlertLow = 20,
      Tick = 21,
      FailShoot = 22,
      // Thump = 23,
      // InspireBannerExit = 24,
      // Thump2 = 25,
      UpgradeChoose1 = 26,
      UpgradeChoose2 = 27,
      EnemyHitWall = 28,
      BigShoot = 29,
      PauseMenuClick = 30,
      WaterSplash1 = 31,
      WaterSplash2 = 32,
      FleshyHit = 33,
      Zap = 34,
      PaperRip = 35,
      Explosion2 = 36,
      Thunder = 37,
      Explosion3 = 38,
      CannonFire = 39,
      TestSound = 40,
      PageTurn = 41,
      PianoNote = 42,
      EnemyExplosion1 = 43,
      Fuse = 44,
      Squish = 45,
      Maximize = 46,
      Minimize = 47,
      MultipleClicks = 48,
      SwitchClick = 49,
      SingleClick = 50,
   }

   [System.Serializable]
   public class SoundAudioClip
   {
      public Sound sound;
      public AudioClip audioClip;
      public int quantity = 5;
   }

   public List<SoundAudioClip> sounds;

   private Dictionary<Sound, Queue<AudioSource>> dict;
   private Dictionary<Sound, bool> canPlayDict;

   private float expPickupPitch = 0.9f; // Unused, i think
   private Coroutine currentPickupExpCR; // Unused, i think

   private float sfxVolume = 0.20f;

   private void Awake()
   {
      if (instance == null)
		{
         instance = this;
         InitializeDictionary();
         DontDestroyOnLoad(gameObject);
      }
      else
		{
         Destroy(gameObject);
		}
   }

   // Each sound gets one object with multiple AudioSource components
   private void InitializeDictionary()
   {
      dict = new Dictionary<Sound, Queue<AudioSource>>();
      canPlayDict = new Dictionary<Sound, bool>();

      // Creates a dictionary for each audio source containing each different sound that can be played
      foreach (SoundAudioClip clip in sounds)
      {
         Queue<AudioSource> audioPool = new Queue<AudioSource>();
         GameObject soundObject = new GameObject(clip.sound.ToString());
         // All audio sources are persistent through scenes
         DontDestroyOnLoad(soundObject);

         for (int i = 0; i < clip.quantity; i++)
			{
            AudioSource source = soundObject.AddComponent<AudioSource>();
            source.tag = "SoundEffect";
            source.clip = clip.audioClip;
				source.volume = sfxVolume;
            source.playOnAwake = false;
            audioPool.Enqueue(source);
			}

         dict.Add(clip.sound, audioPool);

         canPlayDict.Add(clip.sound, true);
      }
   }

   public void PlaySound(Sound sound, bool randomizePitch = true, float volumeDelta = 0)
   {
      if (!canPlayDict[sound])
         return;

      AudioSource toPlay = dict[sound].Dequeue();

      if (randomizePitch)
         toPlay.pitch = Random.Range(0.95f, 1.05f);
      else
         toPlay.pitch = 1;
      toPlay.volume = sfxVolume + volumeDelta;

      toPlay.Play();
      dict[sound].Enqueue(toPlay);
      StartCoroutine(CannotPlaySound(sound));
   }

   public void PlaySoundPitch(Sound sound, float pitch, float volumeDelta = 0)
	{
      if (!canPlayDict[sound])
         return;

      AudioSource toPlay = dict[sound].Dequeue();
		toPlay.pitch = pitch;
      toPlay.volume = sfxVolume + volumeDelta;

      toPlay.Play();
      dict[sound].Enqueue(toPlay);
      StartCoroutine(CannotPlaySound(sound));
   }

   public void PlayExpPickupSound()
	{
      if (currentPickupExpCR != null)
         StopCoroutine(currentPickupExpCR);
      currentPickupExpCR = StartCoroutine(ResetExpPickupPitch());
      AudioSource toPlay = dict[Sound.PickupExp].Dequeue();
      toPlay.pitch = expPickupPitch;
      toPlay.Play();
      dict[Sound.PickupExp].Enqueue(toPlay);
      expPickupPitch = Mathf.Min(expPickupPitch + 0.025f, 1.3f);
   }

   private IEnumerator ResetExpPickupPitch()
	{
      yield return new WaitForSeconds(0.3f);
      expPickupPitch = 0.9f;
	}

   private IEnumerator CannotPlaySound(Sound sound)
	{
      canPlayDict[sound] = false;

      yield return null;
      yield return null;

      canPlayDict[sound] = true;
	}

   /// <summary>
   /// Don't use this
   /// </summary>
   private AudioClip GetAudioClip(Sound sound)
   {
      foreach (SoundAudioClip soundClip in sounds)
      {
         if (soundClip.sound == sound)
         {
            return soundClip.audioClip;
         }
      }
      return null;
   }
}
