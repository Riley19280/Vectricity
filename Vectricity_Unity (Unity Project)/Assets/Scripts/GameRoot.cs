using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameRoot : MonoBehaviour
{
	//this is the main class where everything else originates form
	void Awake ()
	{
		GM.ScreenDims = new Vector2 (Screen.width, Screen.height);
		GM.ScreenRect = new Rect (0, 0, Screen.width, Screen.height);

	}

	void Start ()
	{
		Time.timeScale = 0;
		//setting up crucial parts
		InputManager.addPlayer1 ();

		GM.ScreenDims = new Vector2 (Screen.width, Screen.height);
		GM.ScreenRect = new Rect (0, 0, Screen.width, Screen.height);

		Art.LoadContent ();
		WaveManager.Initialize ();

		ScreenManager.Initialize ();
		#if UNITY_ANDROID
		//Input.simulateMouseWithTouches = false;
		#endif
	
		
		if (PlayerPrefs.GetInt (GM.PP_FIRSTTIME, 1) == 1) {
			PlayerPrefs.SetInt (GM.PP_FIRSTTIME, 0);
			PlayerPrefs.SetFloat (GM.PP_music, .1f );
			PlayerPrefs.SetFloat (GM.PP_sound, 1f );
			PlayerPrefs.SetInt (GM.PP_highscore, 0 );
			PlayerPrefs.SetInt (GM.PP_totalPoints, 0 );
			PlayerPrefs.SetInt (GM.PP_totalKills, 0 );
			PlayerPrefs.SetInt (GM.PP_gameCount, 0 );
			PlayerPrefs.SetInt(GM.PP_joysticksVisible, 1 );
			PlayerPrefs.Save();
		}

		GM.RequestBanner();
	}

	void FixedUpdate ()
	{

		WaveManager.Update ();

	}

	void Update ()
	{
		//update screen dims
		GM.ScreenDims = new Vector2 (Screen.width, Screen.height);
		GM.ScreenRect = new Rect (0, 0, Screen.width, Screen.height);

		InputManager.Update ();


	}

}