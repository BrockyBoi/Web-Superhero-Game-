using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundController : MonoBehaviour {
	public static SoundController controller;

	public static Dictionary<int, AudioClip> soundClips = new Dictionary<int, AudioClip>();
	AudioSource audio;

	//Tank audio clips
	public AudioClip upperCut;
	public AudioClip sonicClap;
	public AudioClip shoulderCharge;
	public AudioClip groundSmash;

	//Elementalist
	public AudioClip flamethrower;
	public AudioClip wave;
	public AudioClip rockThrow;
	public AudioClip lightningStrike;

	//Paragon 
	public AudioClip paragonPunch;
	public AudioClip freezeBreath;
	public AudioClip heatVision;
	public AudioClip superJump;
	public AudioClip superJumpLanding;

	//Speedster
	public AudioClip speedsterPunch;
	public AudioClip whirlwind;
	public AudioClip dash;
	public AudioClip mapDash;

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

		audio = gameObject.AddComponent<AudioSource> ();

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
		AddSound (SuperPowerController.PowerNames.TankMelee, upperCut);
		AddSound (SuperPowerController.PowerNames.SonicClap, sonicClap);
		AddSound (SuperPowerController.PowerNames.ShoulderCharge, shoulderCharge);
		AddSound (SuperPowerController.PowerNames.GroundSmash, groundSmash);
		//Elementalist
		AddSound (SuperPowerController.PowerNames.Fire, flamethrower);
		AddSound (SuperPowerController.PowerNames.Wave, wave);
		AddSound (SuperPowerController.PowerNames.RockThrow, rockThrow);
		AddSound (SuperPowerController.PowerNames.Lightning, lightningStrike);
		//Paraon
		AddSound (SuperPowerController.PowerNames.ParagonMelee, paragonPunch);
		AddSound (SuperPowerController.PowerNames.FreezeBreath, freezeBreath);
		AddSound (SuperPowerController.PowerNames.HeatVision, heatVision);
		AddSound (SuperPowerController.PowerNames.Jump, superJump);
		//Speedster
		AddSound (SuperPowerController.PowerNames.SpeedMelee, speedsterPunch);
		AddSound (SuperPowerController.PowerNames.WindGust, whirlwind);
		AddSound (SuperPowerController.PowerNames.DashAttack, dash);
		AddSound (SuperPowerController.PowerNames.MapDash, mapDash);
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
		audio.PlayOneShot (soundClips [i]);
	}

	public void PlaySound(AudioClip clip)
	{
		audio.PlayOneShot (clip);
	}

	public void PlaySound(AudioSource source, AudioClip clip)
	{
		source.PlayOneShot (clip);
	}
}
