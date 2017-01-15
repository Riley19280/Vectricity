using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{

	public AudioSource sourceS;
	public	AudioSource sourceM;
	public AudioClip[] songs;

	void Start ()
	{
		GM.AudioMgr = gameObject;
		changeSong ();
	}

	public void playSoundEffect (AudioClip clip)
	{
		sourceS.PlayOneShot (clip, PlayerPrefs.GetFloat(GM.PP_sound));
	}

	public void playMusic ()
	{
		//changeSong ();

		sourceM.Play ();
		updateMusicVol ();
	}

	public void updateMusicVol ()
	{
		sourceM.volume = PlayerPrefs.GetFloat(GM.PP_music);
	}

	System.Random rand = new System.Random ();

	public void changeSong ()
	{
		sourceM.clip = songs [rand.Next (songs.Length)];
		playMusic ();
	}

}
