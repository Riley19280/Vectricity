using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class resumeCountdown
{

	int alpha = 255;
	string alertText = "";
	int secs = 0;
	int ticks = 0;

	public resumeCountdown (int num)
	{
		Time.timeScale = 0;
		GM.InGame = false;
		alertText = num.ToString ();
		ticks = 60 * num;

	}

	void Update ()
	{
		//fades countdown out
		alertText = ((ticks / 60) + 1).ToString ();

		if (alpha < 0) {
			alpha = 0;
			alpha = 255;
		}
		if (ticks <= 0) {

			Time.timeScale = 1;
			ScreenManager.DelOverlay ();
		}
		ticks--;
		//alpha -= 3;
	}



}
