using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using GoogleMobileAds.Api;

public class Pause : MonoBehaviour
{
	public GameObject upgradeButton;

	void Start ()
	{
		GM.InGame = false;
		Time.timeScale = 0;
		GM.backToMain = false;

#if UNITY_ANDROID
		upgradeButton.SetActive(true);
#endif
		GM.ShowBanner();
	}

	public void BuyScreen(){
		ScreenManager.AddScreen (GameObject.Instantiate (Art.BuyScreen));
	}

	//handles GUI back button
	public void Back ()
	{
		GM.InGame = true;

		ScreenManager.AddScreen (GameObject.Instantiate (Art.GameGUIScreen));
		GameObject.Find ("GameGUIScreen(Clone)").GetComponent<GameGUI> ().DoSlowOut();

	}
	//handles GUI control button
	public void Controls ()
	{
		ScreenManager.AddScreen (GameObject.Instantiate (Art.ControlScreen));
	}

	//handles GUI options button
	public void Options ()
	{
		ScreenManager.AddScreen (GameObject.Instantiate (Art.OptionsScreen));
	}

	//handles GUI main menu button
	public void MainMenu ()
	{
		GM.InGame = false;
		Time.timeScale = 0;
		ScreenManager.AddScreen (GameObject.Instantiate (Art.StartScreen));
	}
	#if !UNITY_ANDROID
	//handles mouse draw
	void OnGUI ()
	{
		int cursorSizeX = 32;  
		int cursorSizeY = 32;  
		GUI.DrawTexture (new Rect (Event.current.mousePosition.x, Event.current.mousePosition.y, cursorSizeX, cursorSizeY), Art.Cursor);
	}
#endif
}