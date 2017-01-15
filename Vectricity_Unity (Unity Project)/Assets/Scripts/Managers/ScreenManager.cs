using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public static class ScreenManager
{
	//class that manages the current screen and overlay that is active

	static GameObject currScreen;
	static GameObject overlay;

	public static void Initialize ()
	{
		Cursor.visible = false;
		AddScreen (GameObject.Instantiate (Art.StartScreen));
	}

	public static void AddScreen (GameObject screen)
	{
		DelScreen ();
		currScreen = screen;
	}

	public static void AddOverlay (GameObject screen)
	{
		overlay = screen;
	}

	public static void DelOverlay ()
	{
		GameObject.Destroy (overlay);
	}

	public static void DelScreen ()
	{
		if (currScreen != null)
			GameObject.Destroy (currScreen);
	}
}