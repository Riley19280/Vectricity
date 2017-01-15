using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using GoogleMobileAds.Api;

public class GameGUI : MonoBehaviour
{
	public GameObject ScoreText;
	public GameObject Healthbar;
	public GameObject timer;
	public GameObject timer_overlay;
	public GameObject overlay;
	public GameObject pnlItems;
	public GameObject btnPause;
	GameObject playerGO;
	int overlaytime = 180;
	float startHBWidth;
	bool timeBombActive = false;
	int timeBombtime;
	float timePreTick;
	bool slowOut = false;
	float slowOutPerTick = .005f;
	bool movePanelUp = false;
	bool movePanelDown = false;
	bool panelIsShowing = false;
	float panelTopPos = 75;
	float panelBottomPos = -80;
	float panelUpTimeMAX = 240;
	float panelUpTime = 0;

	void Awake ()
	{
		Time.timeScale = 1;
	}

	void Start ()
	{
		GM.InGame = true;

		startHBWidth = Healthbar.GetComponent<RectTransform> ().sizeDelta.x;

		//for timer for timebomb
		timer.SetActive (false);
		timer_overlay.SetActive (false);
		overlay.SetActive (false);

	
		#if UNITY_ANDROID
		GM.DeleteBanner ();
		GM.RequestInterstitial ();

		pnlItems.SetActive(true);
		btnPause.SetActive(true);
		playerGO = GameObject.Find("player(Clone)");
		#endif

	}

	#if !UNITY_ANDROID
	//draw mouse
	void OnGUI ()
	{
		int cursorSizeX = 32;  
		int cursorSizeY = 32; 
		GUI.DrawTexture (new Rect (Event.current.mousePosition.x, Event.current.mousePosition.y, cursorSizeX, cursorSizeY), Art.Cursor);
	}
	#endif
	public void DoTimeBomb ()
	{
		//slowing down time
		timeBombActive = true;
		timeBombtime = GM.timeBombTime;
		Time.timeScale = .25f;
		timer_overlay.GetComponent<Image> ().fillAmount = 0;
		timePreTick = (1 - Time.timeScale) / timeBombtime;
	
		timer.SetActive (true);
		timer_overlay.SetActive (true);
	}

	void Update ()
	{

		//checkling for input
		#if UNITY_ANDROID
		if (Input.touchCount==3)
			pause();
		
		#else
		if (InputManager.PlayerInput (1).BtnStart)
			pause ();
		#endif	

		if (slowOut) {
			Time.timeScale += slowOutPerTick;
			
			if (Time.timeScale >= 1) {
				Time.timeScale = 1;
				slowOut = false;
			}
		}

		
		
		//updating the GUI when the time bomb is active
		if (timeBombActive) {
			timeBombtime--;
			
			Time.timeScale += timePreTick;
			timer_overlay.GetComponent<Image> ().fillAmount += timePreTick + (.25f / GM.timeBombTime);
			if (timeBombtime <= 0) {
				timeBombActive = false;
				Time.timeScale = 1;
				timer.SetActive (false);
				timer_overlay.SetActive (false);
			}
		}
	}
  
	void FixedUpdate ()
	{

		//setting score display and checking for input
		ScoreText.GetComponent<Text> ().text = "SCORE: " + GM.Points;

		if (PlayerManager.player (1) != null)
			Healthbar.GetComponent<RectTransform> ().sizeDelta = new Vector2 (startHBWidth * ((PlayerManager.player (1).GetComponent<PlayerShip> ().Health / GM.maxHealth)), Healthbar.GetComponent<RectTransform> ().sizeDelta.y);



		if (overlay.active) {
			if (overlaytime <= 0) {
				overlay.SetActive (false);
			}
			if (overlaytime > 0)
				overlaytime--;

         
			overlay.GetComponent<Text> ().color = new Color (overlaytime, overlaytime, overlaytime, overlaytime);
		}

		PanelMovement ();
	}

	public  void DoOverlay (string s)
	{
		overlaytime = 360;
		overlay.SetActive (true);
		overlay.GetComponent<Text> ().text = s;

	}
			
	public void DoSlowOut ()
	{
		Time.timeScale = .1f;
		slowOut = true;			
	}

	void PanelMovement ()
	{
		//ScoreText.GetComponent<Text> ().text = pnlItems.transform.position.y +" < " + panelBottomPos +"\n"+ movePanelUp + movePanelDown+panelIsShowing;

		if (movePanelUp) {
			if (pnlItems.transform.position.y < panelTopPos) {
				pnlItems.transform.position = new Vector3 (pnlItems.transform.position.x, pnlItems.transform.position.y + 2, pnlItems.transform.position.z);
			} else {
				movePanelUp = false;
				panelIsShowing = true;
			}
		}

		if (movePanelDown) {
			if (pnlItems.transform.position.y > panelBottomPos) {
				pnlItems.transform.position = new Vector3 (pnlItems.transform.position.x, pnlItems.transform.position.y - 2, pnlItems.transform.position.z);
			} else {
				movePanelDown = false;
				panelIsShowing = false;
				panelUpTime = 0;
			}
		}

		if (panelIsShowing) {
			panelUpTime++;
		}

		if (panelUpTime >= panelUpTimeMAX) {
			if (panelIsShowing)
				movePanelDown = true;
		}

		foreach (Touch t in Input.touches) {
			if (t.tapCount == 2) {
				movePanelDown = false;
				movePanelUp = true;
				panelUpTime = 0;
			}
		}
	}

	public void buyItem (int i)
	{
		panelUpTime = 0;

		switch (i) {
		case 0: 
			if (GM.Points >= 1000) {
				DoTimeBomb ();
				GM.Points -= 1000;
			}
			break;
		case 1: 
			if (GM.Points >= 50) {
				GameObject.Instantiate (Art.Firewall, playerGO.transform.position, Quaternion.identity);			
				GM.Points -= 50;
			}
			break;
		case 2: 
			if (GM.Points >= 25) {
				GameObject.Instantiate (Art.Freeze, playerGO.transform.position, Quaternion.identity);
				GM.Points -= 25;
			}
			break;
		case 3: 
			if (GM.Points >= 75) {
				GameObject.Instantiate (Art.Tower, playerGO.transform.position, Quaternion.identity);
				GM.Points -= 75;
			}
			break;


		}


	}

	public void pause ()
	{
		ScreenManager.AddScreen (GameObject.Instantiate (Art.PauseScreen));
	}

	void OnDestroy() {
		GM.RequestBanner();
	
	}


}