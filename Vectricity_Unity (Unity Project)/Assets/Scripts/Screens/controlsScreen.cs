using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using GoogleMobileAds.Api;

public class controlsScreen : MonoBehaviour
{
	public GameObject Background;
	public Sprite AndroidBackground;
	void Start ()
	{
		Cursor.visible = false;
		GM.InGame = false;
		Time.timeScale = 0;
	
		#if UNITY_ANDROID
		Background.GetComponent<Image>().sprite = AndroidBackground;
#endif

		GM.ShowBanner();
	}


	public void back ()
	{
		if (GM.backToMain)
			ScreenManager.AddScreen (GameObject.Instantiate (Art.StartScreen));
		else 
			ScreenManager.AddScreen (GameObject.Instantiate (Art.PauseScreen));
	}
	#if !UNITY_ANDROID
	void OnGUI ()
	{
		int cursorSizeX = 32;  // cursor size x
		int cursorSizeY = 32;  // cursor size y
		GUI.DrawTexture (new Rect (Event.current.mousePosition.x, Event.current.mousePosition.y, cursorSizeX, cursorSizeY), Art.Cursor);
	}
#endif
}