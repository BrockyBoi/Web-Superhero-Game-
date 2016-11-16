using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundController : MonoBehaviour {
	public static SoundController controller;

	public static Dictionary<int, AudioClip> soundClips = new Dictionary<int, AudioClip>();
	AudioSource fxAudio;
	AudioSource musicAudio;

	public AudioClip music;

	public AudioClip punch;
	public AudioClip capeWhoosh;
	public AudioClip whoosh;

	//Tank audio clips
	public AudioClip sonicClap;
	public AudioClip shoulderCharge;
	public AudioClip groundSmash;

	//Elementalist
	public AudioClip flamethrower;
	public AudioClip wave;
	public AudioClip rockSummon;
	public AudioClip rockThrow;
	public AudioClip lightningStrike;

	//Paragon 
	public AudioClip freezeBreath;
	public AudioClip heatVision;

	//Speedster
	public AudioClip whirlwind;

	//Vigilante
	public AudioClip pistolShot;
	public AudioClip shotgunShot;
	public AudioClip sniperShot;
	public AudioClip grenades;



	void Awake()
	{
		if (controller == null) {
			controller = this;
			DontDestroyOnLoad (this);
		} else
			Destroy (gameObject);

		fxAudio = gameObject.AddComponent<AudioSource> ();
		musicAudio = gameObject.AddComponent<AudioSource> ();

		//musicAudio.clip = music;
		musicAudio.Play ();
		musicAudio.volume = .275f;
		musicAudio.loop = true;

		InitializeSoundList ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void AddSound(SuperPowerController.PowerNames power, AudioClip sound)
	{
		soundClips.Add ((int)power, sound);
	}

	void InitializeSoundList()
	{
		//Tank
		AddSound (SuperPowerController.PowerNames.SonicClap, sonicClap);
		AddSound (SuperPowerController.PowerNames.ShoulderCharge, shoulderCharge);
		AddSound (SuperPowerController.PowerNames.GroundSmash, groundSmash);
		//Elementalist
		AddSound (SuperPowerController.PowerNames.Fire, flamethrower);
		AddSound (SuperPowerController.PowerNames.Wave, wave);
		AddSound (SuperPowerController.PowerNames.RockThrow, rockThrow);
		AddSound (SuperPowerController.PowerNames.Lightning, lightningStrike);

		AddSound (SuperPowerController.PowerNames.FreezeBreath, freezeBreath);
		AddSound (SuperPowerController.PowerNames.HeatVision, heatVision);

		AddSound (SuperPowerController.PowerNames.WindGust, whirlwind);
		//Vigilante
		AddSound (SuperPowerController.PowerNames.Pistol, pistolShot);
		AddSound (SuperPowerController.PowerNames.Shotgun, shotgunShot);
		AddSound (SuperPowerController.PowerNames.Sniper, sniperShot);
		AddSound (SuperPowerController.PowerNames.Grenades, grenades);
	}

	public void PlaySoundInList(AudioSource audioSource, SuperPowerController.PowerNames power)
	{
		audioSource.PlayOneShot (soundClips [(int)power]);
	}

	public void PlaySoundInList(AudioSource audioSource, int i)
	{
		audioSource.PlayOneShot (soundClips [i]);
	}

	public void PlaySoundInList(int i)
	{
		fxAudio.PlayOneShot (soundClips [i]);
	}

	public void PlaySound(AudioClip clip)
	{
		fxAudio.PlayOneShot (clip);
	}

	public void PlaySound(AudioSource source, AudioClip clip)
	{
		source.PlayOneShot (clip);
	}
}
