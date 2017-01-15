using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using GoogleMobileAds.Api;

public static class GM
{
	//main class that all others access game variables and important data/functions

	//global vars


	public static Vector2 ScreenDims;
	public static Rect ScreenRect;
	public static int maxPoints = 0;
	public static int Points = 0;
	public static bool InGame = false;
	public static Vector2 basePos = ScreenDims / 2;
	public static bool ENABLEADS = true;
	public static int firewallTime = 1800;
	public static int freezeTime = 1800;
	public static int towerTime = 1800;
	public static int timeBombTime = 180;
	public static int Difficulty = 1;
	public static int regenerationAmt = 5;
	public static int maxHealth = 100;
	static int chkTmr = 120;
	public static bool keyboardEnabled;
	public static bool backToMain = true;
	public static GameObject AudioMgr;
	public static string PP_FIRSTTIME = "FIRSTTIME";
	public static string PP_sound = "sound";
	public static string PP_music = "music";
	public static string PP_highscore = "highscore";
	public static string PP_totalKills = "totalkills";
	public static string PP_totalPoints = "totalpoints";
	public static string PP_gameCount = "gamecount";
	public static string PP_joysticksVisible = "joysticksVisibility";
	public static Texture2D testTexture;
	public static int killCount;

	//called for a new game, resets everything
	public static void NEWGAME ()
	{
		//reseting all values necessary for a new game
		foreach (GameObject g in GameObject.FindGameObjectsWithTag("enemy"))
			GameObject.Destroy (g);
		foreach (GameObject g in GameObject.FindGameObjectsWithTag("bullet"))
			GameObject.Destroy (g);
		foreach (GameObject g in GameObject.FindGameObjectsWithTag("item"))
			GameObject.Destroy (g);

		

		ScreenManager.AddScreen (GameObject.Instantiate (Art.GameGUIScreen));

	

		PlayerManager.Initialize ();
		EnemyManager.CLEARALL ();

		testTexture = (Texture2D)Resources.Load ("Enemies/square");
	
	
		Points = 0;
		WaveManager.waveNum = 1;
		WaveManager.Spawning = true;
		WaveManager.gotInfo = false;
		WaveManager.sentMessage = false;
		WaveManager.spawnLoadedWaves = true;
		Difficulty = 1;
		chkTmr = 120;
		regenerationAmt = 5;
		maxHealth = 100;

		firewallTime = 1800;
		freezeTime = 1800;
		towerTime = 1800;
		maxPoints = 0;
		killCount = 0;

		Time.timeScale = 1;

	}

	//helper method to provide clamp functionality for vectors
	public static Vector2 VectorClamp (Vector2 start, Vector2 lower, Vector2 upper)
	{
		float lowerX = lower.x;
		float lowerY = lower.y;

		float upperX = upper.x;
		float upperY = upper.y;

		float adjX = Mathf.Clamp (start.x, lowerX, upperX);
		float adjY = Mathf.Clamp (start.y, lowerY, upperY);

		return new Vector2 (adjX, adjY);
	}

	//slowly increases the 

	static	BannerView bannerView;

	public static void RequestBanner ()
	{
		if (ENABLEADS) {
			#if UNITY_ANDROID
		string adUnitId = "ca-app-pub-2351910116411316/7184168386";
			#elif UNITY_IPHONE
		string adUnitId = "ca-app-pub-3940256099942544/6300978111";
			#else
			string adUnitId = "ca-app-pub-3940256099942544/6300978111";
			#endif
		
			// Create a 320x50 banner at the top of the screen.
			bannerView = new BannerView (adUnitId, AdSize.Banner, AdPosition.Bottom);
			// Create an empty ad request.
			AdRequest request = new AdRequest.Builder ().Build ();
			// Load the banner with the request.
			bannerView.LoadAd (request);
		}
	}

	public static void HideBanner ()
	{
		bannerView.Hide ();
	}

	public static void ShowBanner ()
	{
		bannerView.Show ();
	}

	public static void DeleteBanner ()
	{
		bannerView.Destroy ();
	}

	static	InterstitialAd interstitial;

	public static void RequestInterstitial ()
	{
		if (ENABLEADS) {
			#if UNITY_ANDROID
		string adUnitId = "ca-app-pub-2351910116411316/8660901586";
			#elif UNITY_IPHONE
		string adUnitId = "ca-app-pub-3940256099942544/1033173712";
			#else
			string adUnitId = "ca-app-pub-3940256099942544/1033173712";
			#endif
		
			// Initialize an InterstitialAd.
			interstitial = new InterstitialAd (adUnitId);
			// Create an empty ad request.
			AdRequest request = new AdRequest.Builder ().Build ();
			// Load the interstitial with the request.
			interstitial.OnAdLoaded += HandleOnAdLoaded;
			interstitial.LoadAd (request);
		}
	}

	public static void ShowInterstitial ()
	{
		if (interstitial.IsLoaded ()) {
			interstitial.Show ();
		}
	}

	public static void DeleteInterstitial ()
	{
		interstitial.Destroy ();
	}

	public static void HandleOnAdLoaded (object sender, EventArgs args)
	{
		Debug.Log ("interstitial loaded");
	}
}

