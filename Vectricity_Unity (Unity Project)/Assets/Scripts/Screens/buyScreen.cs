using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class buyScreen : MonoBehaviour
{
	public GameObject ScoreText;
	int[] bulletCost = new int[4] { 25, 50, 100, 150 };
	int[] laserCost = new int[4] { 50, 75, 125, 200 };
	int[] missileCost = new int[4] { 50, 100, 150, 250 };
	int regenCost = 100;
	int healthCost = 100;
	int towerCost = 100;
	int firewallCost = 100;
	int freezeCost = 75;
	int timeBombCost = 100;
	public GameObject[] menuItems;
	int boxWidth, boxHeight;
	int player = 1;

	void Start ()
	{
		GM.InGame = false;
		Time.timeScale = 0;

		//updating the prices displayed
		boxWidth = (int)(GM.ScreenDims.x / 3);
		boxHeight = (int)(GM.ScreenDims.y - 100) / 3;
		if (PlayerManager.player (1).GetComponent<PlayerShip> ().BulletTier != 5)
			menuItems [0].GetComponentInChildren<Text> ().text = "Price: " + bulletCost [PlayerManager.player (1).GetComponent<PlayerShip> ().BulletTier - 1];
		if (PlayerManager.player (1).GetComponent<PlayerShip> ().LaserTier != 5)
			menuItems [1].GetComponentInChildren<Text> ().text = "Price: " + laserCost [PlayerManager.player (1).GetComponent<PlayerShip> ().LaserTier - 1];
		if (PlayerManager.player (1).GetComponent<PlayerShip> ().MissileTier != 5)
			menuItems [2].GetComponentInChildren<Text> ().text = "Price: " + missileCost [PlayerManager.player (1).GetComponent<PlayerShip> ().MissileTier - 1];
		menuItems [3].GetComponentInChildren<Text> ().text = "Price: " + regenCost;
		menuItems [4].GetComponentInChildren<Text> ().text = "Price: " + healthCost;
		menuItems [5].GetComponentInChildren<Text> ().text = "Price: " + towerCost;
		menuItems [6].GetComponentInChildren<Text> ().text = "Price: " + firewallCost;
		menuItems [7].GetComponentInChildren<Text> ().text = "Price: " + freezeCost;
		menuItems [8].GetComponentInChildren<Text> ().text = "Price: " + timeBombCost;
	
		GM.HideBanner();
	}



	void Update ()
	{
		//checking for exit
		ScoreText.GetComponent<Text> ().text = "SCORE: " + GM.Points;
		#if UNITY_ANDROID
		if(Input.touchCount==3){
			back();
		}
		#else
		if (InputManager.PlayerInput (player).BtnB || InputManager.PlayerInput (player).BtnBack) {
			back();
		}
#endif
	}
	#if !UNITY_ANDROID
	void OnGUI ()
	{
		//drawing the cursor
		int cursorSizeX = 32;  // cursor size x
		int cursorSizeY = 32;  // cursor size y
		GUI.DrawTexture (new Rect (Event.current.mousePosition.x, Event.current.mousePosition.y, cursorSizeX, cursorSizeY), Art.Cursor);
	}
#endif


	//functions for each button press
	public void DoActions (int selectedIndex)
	{
		switch (selectedIndex) {
            #region Upgrade bullets
		case 0:
			PlayerManager.player (player).GetComponent<PlayerShip> ().fireRate = 7;

			if (PlayerManager.player (player).GetComponent<PlayerShip> ().BulletTier != 5) {

				if (GM.Points >= bulletCost [PlayerManager.player (player).GetComponent<PlayerShip> ().BulletTier - 1]) {
					//play buy sound here
					clickSound ();


					GM.Points -= bulletCost [PlayerManager.player (player).GetComponent<PlayerShip> ().BulletTier - 1];
					PlayerManager.player (player).GetComponent<PlayerShip> ().BulletTier += 1;
                       
					
					if (PlayerManager.player (1).GetComponent<PlayerShip> ().BulletTier <= 4) {
						menuItems [0].GetComponentInChildren<Text> ().text = "Price: " + bulletCost [PlayerManager.player (1).GetComponent<PlayerShip> ().BulletTier-1];
					} else {
						menuItems [0].GetComponentInChildren<Text> ().text = "";
					}


				} else {
					notEnoughSound ();
				}

			} else {
				//play sound here
				notEnoughSound ();
                 
			}

			PickBullet ();
			break;


            #endregion

            #region Upgrade Lasers

		case 1:
			PlayerManager.player (player).GetComponent<PlayerShip> ().fireRate = 5;
               

                //upgrade lasers
			if (PlayerManager.player (player).GetComponent<PlayerShip> ().LaserTier != 5) {
				if (GM.Points >= laserCost [PlayerManager.player (player).GetComponent<PlayerShip> ().LaserTier - 1]) {
					//play buy sound here
					clickSound ();


					GM.Points -= laserCost [PlayerManager.player (player).GetComponent<PlayerShip> ().LaserTier - 1];
					PlayerManager.player (player).GetComponent<PlayerShip> ().LaserTier += 1;
                        
					
					if (PlayerManager.player (1).GetComponent<PlayerShip> ().LaserTier <= 4) {
						menuItems [1].GetComponentInChildren<Text> ().text = "Price: " + laserCost [PlayerManager.player (1).GetComponent<PlayerShip> ().LaserTier-1];
					} else {
						menuItems [1].GetComponentInChildren<Text> ().text = "";
					}


				} else {
					notEnoughSound ();
				}

			} else {
                
				//play sound here
				notEnoughSound ();
			}

	

			PickLaser ();
			break;

            #endregion

            #region Upgrade Missiles
		case 2:
			PlayerManager.player (player).GetComponent<PlayerShip> ().fireRate = 7;
               


                //upgrade missiles                 
			if (PlayerManager.player (player).GetComponent<PlayerShip> ().MissileTier != 5) {

				if (GM.Points >= missileCost [PlayerManager.player (player).GetComponent<PlayerShip> ().MissileTier - 1]) {
					//play buy sound here
					clickSound ();


					GM.Points -= missileCost [PlayerManager.player (player).GetComponent<PlayerShip> ().MissileTier - 1];
					PlayerManager.player (player).GetComponent<PlayerShip> ().MissileTier += 1;

					if (PlayerManager.player (1).GetComponent<PlayerShip> ().MissileTier <= 4) {
						menuItems [2].GetComponentInChildren<Text> ().text = "Price: " + missileCost [PlayerManager.player (1).GetComponent<PlayerShip> ().MissileTier-1];
					} else {
						menuItems [2].GetComponentInChildren<Text> ().text = "";
					}


                       
				} else {
					notEnoughSound ();
				}

			} else {
				notEnoughSound ();
			}
			


			PickMissile ();
			break;
            #endregion

		case 3:
                //upgrade regen
			if (GM.Points >= regenCost) {
				GM.regenerationAmt += 5;
				GM.Points -= regenCost;
				clickSound ();
			} else {
				notEnoughSound ();
			}
			break;
		case 4:
                //upgrade max health
			if (GM.Points >= healthCost) {
				GM.maxHealth += 25;
				GM.Points -= healthCost;
				clickSound ();
			} else {
				notEnoughSound ();
			}
			break;
		case 5:
                //upgrade tower
			if (GM.Points >= towerCost) {
				GM.towerTime += 900;
				GM.Points -= towerCost;
				clickSound ();
			} else {
				notEnoughSound ();
			}
			break;
		case 6:
                // upgrade firewall
			if (GM.Points >= firewallCost) {
				GM.firewallTime += 900;
				GM.Points -= firewallCost;
				clickSound ();
			} else {
				notEnoughSound ();
			}
			break;
		case 7:
                //upgrade freeze
			if (GM.Points >= freezeCost) {
				GM.freezeTime += 900;
				GM.Points -= freezeCost;
				clickSound ();
			} else {
				notEnoughSound ();
			}
			break;
		case 8:
                //upgrade nuke
			if (GM.Points >= timeBombCost) {
				GM.timeBombTime += 30;
				GM.Points -= timeBombCost;
				clickSound ();
			} else {
				notEnoughSound ();
			}
			break;
		}
	}

	public void back(){
		ScreenManager.AddScreen (GameObject.Instantiate (Art.GameGUIScreen));
		GameObject.Find ("GameGUIScreen(Clone)").GetComponent<GameGUI> ().DoSlowOut();

	}

	//sets bullet based on tier owned
	private void PickBullet ()
	{
		switch (PlayerManager.player (player).GetComponent<PlayerShip> ().BulletTier) {
		case 1:
			PlayerManager.player (player).GetComponent<PlayerShip> ().ammo = PlayerShip.Ammo.B1;
			break;
		case 2:
			PlayerManager.player (player).GetComponent<PlayerShip> ().ammo = PlayerShip.Ammo.B2;
			break;
		case 3:
			PlayerManager.player (player).GetComponent<PlayerShip> ().ammo = PlayerShip.Ammo.B3;
			break;
		case 4:
			PlayerManager.player (player).GetComponent<PlayerShip> ().ammo = PlayerShip.Ammo.B4;
			break;
		case 5:
			PlayerManager.player (player).GetComponent<PlayerShip> ().ammo = PlayerShip.Ammo.B5;
			break;
		}
	}

	//sets bullet based on tier owned
	private void PickLaser ()
	{
		switch (PlayerManager.player (player).GetComponent<PlayerShip> ().LaserTier) {
		case 1:
			PlayerManager.player (player).GetComponent<PlayerShip> ().ammo = PlayerShip.Ammo.L1;
			break;
		case 2:
			PlayerManager.player (player).GetComponent<PlayerShip> ().ammo = PlayerShip.Ammo.L2;
			break;
		case 3:
			PlayerManager.player (player).GetComponent<PlayerShip> ().ammo = PlayerShip.Ammo.L3;
			break;
		case 4:
			PlayerManager.player (player).GetComponent<PlayerShip> ().ammo = PlayerShip.Ammo.L4;
			break;
		case 5:
			PlayerManager.player (player).GetComponent<PlayerShip> ().ammo = PlayerShip.Ammo.L5;
			break;
		}
	}

	//sets bullet based on tier owned
	private void PickMissile ()
	{
		switch (PlayerManager.player (player).GetComponent<PlayerShip> ().MissileTier) {
		case 1:
			PlayerManager.player (player).GetComponent<PlayerShip> ().ammo = PlayerShip.Ammo.M1;
			break;
		case 2:
			PlayerManager.player (player).GetComponent<PlayerShip> ().ammo = PlayerShip.Ammo.M2;
			break;
		case 3:
			PlayerManager.player (player).GetComponent<PlayerShip> ().ammo = PlayerShip.Ammo.M3;
			break;
		case 4:
			PlayerManager.player (player).GetComponent<PlayerShip> ().ammo = PlayerShip.Ammo.M4;
			break;
		case 5:
			PlayerManager.player (player).GetComponent<PlayerShip> ().ammo = PlayerShip.Ammo.M5;
			break;
		}
	}


	//played if not enought $
	private void notEnoughSound ()
	{
		GM.AudioMgr.GetComponent<AudioManager> ().playSoundEffect (Art.Error);
	}

	//played if succcessfully bought
	private void clickSound ()
	{
		GM.AudioMgr.GetComponent<AudioManager> ().playSoundEffect (Art.Click);
	}


}