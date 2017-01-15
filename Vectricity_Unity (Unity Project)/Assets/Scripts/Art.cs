using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Xml;
using MyDataTypes;
using System;

public static class Art
{
	//all the assets used for the game
	public static XmlDocument WavesXML;
	public static GUIStyle FontStyle;
	public static Texture2D Cursor;
	public static GameObject StartScreen;
	public static GameObject GameGUIScreen;
	public static GameObject OptionsScreen;
	public static GameObject ControlScreen;
	public static GameObject PauseScreen;
	public static GameObject HighscoreScreen;
	public static GameObject BuyScreen;
	public static GameObject Triangle;
	public static GameObject Splitter;
	public static GameObject Splitter2;
	public static GameObject Shooter;
	public static GameObject CloudBoss;
	public static GameObject PlayerGO;
	public static GameObject Tower;
	public static GameObject Freeze;
	public static GameObject Firewall;
	public static GameObject LaserT1;
	public static GameObject LaserT2;
	public static GameObject LaserT3;
	public static GameObject LaserT4;
	public static GameObject LaserT5;
	public static GameObject BulletT1;
	public static GameObject BulletT2;
	public static GameObject BulletT3;
	public static GameObject BulletT4;
	public static GameObject BulletT5;
	public static GameObject MissileT1;
	public static GameObject MissileT2;
	public static GameObject MissileT3;
	public static GameObject MissileT4;
	public static GameObject MissileT5;
	public static GameObject Bolt;
	public static GameObject GhostJoy;

	public static AudioClip Cannon;
	public static AudioClip Click;
	public static AudioClip Pew;
	public static AudioClip Error;
	public static AudioClip Thunder;
	public static AudioClip Silencer;
	public static AudioClip Woosh;


	//this loads them into memory
	public static void LoadContent ()
	{
		FontStyle = new GUIStyle ();

		//wave list
		#region XML Wave Loading

		TextAsset textAsset = (TextAsset)Resources.Load ("Waves");
		WavesXML = new XmlDocument ();
		WavesXML.LoadXml (textAsset.text);

        
		XmlNodeList waveList = WavesXML.GetElementsByTagName ("Wave");

		WaveManager.Waves = new Wave[waveList.Count];

		for (int i = 0; i < waveList.Count; i++) {//for each wave
        
			XmlNodeList waveData = waveList [i].ChildNodes;

			WaveManager.Waves [i] = new Wave ();

			foreach (XmlNode n in waveData) {
				if (n.Name == "id") {

					WaveManager.Waves [i].id = Convert.ToInt16 (n.InnerText);
				}

				if (n.Name == "waitTime") {

					WaveManager.Waves [i].waitTime = Convert.ToInt16 (n.InnerText);
				}
				//loading the spawns
				if (n.Name == "Spawns") {
					WaveManager.Waves [i].Spawns = new Wave.Spawn[n.ChildNodes.Count];//number of spawns in the wave

					XmlNodeList spawns = n.ChildNodes;

					for (int j = 0; j < spawns.Count; j++) {// the data in the spawn aka the items
						WaveManager.Waves [i].Spawns [j] = new Wave.Spawn ();

						WaveManager.Waves [i].Spawns [j].time = Convert.ToInt16 (spawns [j].ChildNodes.Item (0).InnerText);

						WaveManager.Waves [i].Spawns [j].a = Convert.ToInt16 (spawns [j].ChildNodes.Item (1).InnerText);
						WaveManager.Waves [i].Spawns [j].b = Convert.ToInt16 (spawns [j].ChildNodes.Item (2).InnerText);
						WaveManager.Waves [i].Spawns [j].c = Convert.ToInt16 (spawns [j].ChildNodes.Item (3).InnerText);
						WaveManager.Waves [i].Spawns [j].d = Convert.ToInt16 (spawns [j].ChildNodes.Item (4).InnerText);

					}


				}


			}
		}


		Debug.Log ("Waves Loaded: " + waveList.Count);
		WaveManager.maxWaves = waveList.Count;

		#endregion


		Cursor = (Texture2D)Resources.Load ("cursor");

		//Screens
		StartScreen = (GameObject)Resources.Load ("Prefabs/StartScreen");
		GameGUIScreen = (GameObject)Resources.Load ("Prefabs/GameGUIScreen");
		OptionsScreen = (GameObject)Resources.Load ("Prefabs/OptionsScreen");
		ControlScreen = (GameObject)Resources.Load ("Prefabs/ControlScreen");
		PauseScreen = (GameObject)Resources.Load ("Prefabs/PauseScreen");
		HighscoreScreen = (GameObject)Resources.Load ("Prefabs/HighscoreScreen");
		BuyScreen = (GameObject)Resources.Load ("Prefabs/BuyScreen");

		PlayerGO = (GameObject)Resources.Load ("Prefabs/player");

		Triangle = (GameObject)Resources.Load ("Prefabs/triangle");
		Splitter = (GameObject)Resources.Load ("Prefabs/splitter");
		Splitter2 = (GameObject)Resources.Load ("Prefabs/splitter2");
		Shooter = (GameObject)Resources.Load ("Prefabs/shooter");
		CloudBoss = (GameObject)Resources.Load ("Prefabs/cloudboss");
		PlayerGO = (GameObject)Resources.Load ("Prefabs/player");

		Tower = (GameObject)Resources.Load ("Prefabs/turret");
		Freeze = (GameObject)Resources.Load ("Prefabs/freeze");
		Firewall = (GameObject)Resources.Load ("Prefabs/firewall");

		BulletT1 = (GameObject)Resources.Load ("Prefabs/Bullets/bulletT1");
		BulletT2 = (GameObject)Resources.Load ("Prefabs/Bullets/bulletT2");
		BulletT3 = (GameObject)Resources.Load ("Prefabs/Bullets/bulletT3");
		BulletT4 = (GameObject)Resources.Load ("Prefabs/Bullets/bulletT4");
		BulletT5 = (GameObject)Resources.Load ("Prefabs/Bullets/bulletT5");
		LaserT1 = (GameObject)Resources.Load ("Prefabs/Bullets/laserT1");
		LaserT2 = (GameObject)Resources.Load ("Prefabs/Bullets/laserT2");
		LaserT3 = (GameObject)Resources.Load ("Prefabs/Bullets/laserT3");
		LaserT4 = (GameObject)Resources.Load ("Prefabs/Bullets/laserT4");
		LaserT5 = (GameObject)Resources.Load ("Prefabs/Bullets/laserT5");
		MissileT1 = (GameObject)Resources.Load ("Prefabs/Bullets/missileT1");
		MissileT2 = (GameObject)Resources.Load ("Prefabs/Bullets/missileT2");
		MissileT3 = (GameObject)Resources.Load ("Prefabs/Bullets/missileT3");
		MissileT4 = (GameObject)Resources.Load ("Prefabs/Bullets/missileT4");
		MissileT5 = (GameObject)Resources.Load ("Prefabs/Bullets/missileT5");
		Bolt = (GameObject)Resources.Load ("Prefabs/Bullets/bolt");
		GhostJoy = (GameObject)Resources.Load ("Prefabs/ghostJoy");


		//sound effects
		Cannon = (AudioClip)Resources.Load ("Sound/Grenade Launcher");
		Click = (AudioClip)Resources.Load ("Sound/click");
		Pew = (AudioClip)Resources.Load ("Sound/pew");
		Error = (AudioClip)Resources.Load ("Sound/error");
		Thunder = (AudioClip)Resources.Load ("Sound/thunder_strike");
		Silencer = (AudioClip)Resources.Load ("Sound/silencer");
		Woosh = (AudioClip)Resources.Load ("Sound/woosh");

	}



}