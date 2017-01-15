using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using MyDataTypes;

public static class WaveManager
{
	//repsonsivle for spawning the enemies and loading the waves from a file

	public static int maxWaves;
	public static Wave[] Waves;
	public static int waveNum = 0;
	public static bool Spawning = false;
	static int countdown;
	static Wave currWave;
	static int numOfSpawns;
	static int currSpawn;
	static Vector3 topLeft, topRight, btmLeft, btmRight;
	public static bool spawnLoadedWaves = true;

	//for random waves
	static int waveDelay = 0;
	static int minSpawnDelay = 60;
	static int maxSpawnDelay = 240;
	static int minSpawnCount = 30;
	static int maxSpawnCount = 50;

	public static void Initialize ()
	{

		topLeft = new Vector3 (-Camera.main.orthographicSize, Camera.main.orthographicSize / 2);
		topRight = new Vector3 (Camera.main.orthographicSize, Camera.main.orthographicSize / 2);
		btmLeft = new Vector3 (-Camera.main.orthographicSize, -Camera.main.orthographicSize / 2);
		btmRight = new Vector3 (Camera.main.orthographicSize, -Camera.main.orthographicSize / 2);

	}

	public static bool gotInfo = false;
	public static bool sentMessage = false;

	public static void Update ()
	{
		LoadedWaveSpawn ();
	
	}

	static void LoadedWaveSpawn ()
	{
		if (Time.timeScale != 0) {
			countdown--;
			if (countdown <= 0)
				countdown = 0;
			if (gotInfo == false) {
				GetSpawnInfo (waveNum);
				gotInfo = true;
			}

			// while wave is Not over
			if (currSpawn == numOfSpawns) {
				Spawning = false;
				gotInfo = false;
				sentMessage = false;
				waveNum++;
				GameObject.Find ("GameGUIScreen(Clone)").GetComponent<GameGUI> ().DoOverlay (waveNum.ToString ());
				GM.AudioMgr.GetComponent<AudioManager> ().changeSong ();
			}

			if (Spawning == true) {
				if (currSpawn == 0 && sentMessage == false && countdown <= 0) {
					sentMessage = true;
					GM.Difficulty += (int)(waveNum * .1f);
				}
				//get spawn num
				//wait countdown 
				if (countdown > 0) {
					return;
				} else {
					//spawn things

					if (spawnLoadedWaves) {
						Spawn (currWave.Spawns [currSpawn].a);
						Spawn (currWave.Spawns [currSpawn].b);
						Spawn (currWave.Spawns [currSpawn].c);
						Spawn (currWave.Spawns [currSpawn].d);
						countdown = currWave.Spawns [currSpawn].time;
					} else {
						Spawn (Random.Range (1, 5));
						Spawn (Random.Range (1, 5));
						Spawn (Random.Range (1, 5));
						Spawn (Random.Range (1, 5));
						countdown = Random.Range (minSpawnDelay, maxSpawnDelay);
					}
				}
				//move to next spawn
				currSpawn += 1;
			}
		}
	}

	//converts a number into the enemy it is associated with
	private static void Spawn (int id)
	{
		switch (id) {
		case 1:
			GameObject.Instantiate (Art.Triangle, GetPos (), Quaternion.identity);
			break;
		case 2:
			GameObject.Instantiate (Art.Splitter, GetPos (), Quaternion.identity);
			break;
		case 3:
			GameObject.Instantiate (Art.Splitter2, GetPos (), Quaternion.identity);
			break;
		case 4:
			GameObject.Instantiate (Art.Shooter, GetPos (), Quaternion.identity);
			break;

		}
	}
	
	//depending on the order loaded the enemies are put into the approiate corrners
	static int posCtr = 0;

	private static Vector2 GetPos ()
	{
		switch (posCtr) {
		case 0:
			posCtr++;
			return topLeft * Camera.main.aspect;
		case 1:
			posCtr++;
			return topRight * Camera.main.aspect;
		case 2:
			posCtr++;
			return btmLeft * Camera.main.aspect;
		case 3:
			posCtr = 0;
			return btmRight * Camera.main.aspect;
		default:
			return GM.ScreenDims / 2;
		}


	}

	//reads the spawn info from the wave
	public static void GetSpawnInfo (int num)
	{
		if (num > maxWaves) {
			//max waves exceeded

			spawnLoadedWaves = false;
			numOfSpawns = Random.Range (minSpawnCount, maxSpawnCount);
			currSpawn = 0;
			countdown = Random.Range (minSpawnDelay, maxSpawnDelay);
			Spawning = true;

		} else {

			currWave = Waves [num - 1];
			numOfSpawns = currWave.Spawns.Length;
			currSpawn = 0;
			countdown = currWave.waitTime;
			Spawning = true;

		}
	}

}