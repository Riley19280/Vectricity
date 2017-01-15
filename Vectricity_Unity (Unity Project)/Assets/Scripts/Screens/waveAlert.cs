using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class waveAlret : MonoBehaviour
{
	//simple overlay on the screen used to update the player on what wave it is

	int alpha = 255;
	string alertText = "";

	void Update ()
	{
		alpha -= 2;
		if (alpha < 0) {
			alpha = 0;
			ScreenManager.DelOverlay ();
		}
	}
}