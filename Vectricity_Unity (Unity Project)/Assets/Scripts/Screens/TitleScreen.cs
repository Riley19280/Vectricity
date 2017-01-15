using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using GoogleMobileAds.Api;

public class TitleScreen : MonoBehaviour
{


	void Start ()
	{
		Cursor.visible = false;
		Time.timeScale = 0;
		GM.backToMain = true;
#if MOBILE
		GM.ShowBanner();
#endif
	}

	//handles GUI play button
	public void Play ()
	{
		GM.NEWGAME ();

	}

	//handles GUI options button
	public void Options ()
	{
		ScreenManager.AddScreen (GameObject.Instantiate (Art.OptionsScreen));
	}

	//handles GUI cntrols button
	public void Controls ()
	{

		ScreenManager.AddScreen (GameObject.Instantiate (Art.ControlScreen));
	}

	//handles GUI exit button
	public void Exit ()
	{

		Application.Quit();
	}

	#if !UNITY_ANDROID || UNITY_IOS
	//handles GUI mouse draw
	void OnGUI ()
	{
		int cursorSizeX = 32;
		int cursorSizeY = 32; 
		GUI.DrawTexture (new Rect (Event.current.mousePosition.x, Event.current.mousePosition.y, cursorSizeX, cursorSizeY), Art.Cursor);
	}
#endif
}