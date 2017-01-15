using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GoogleMobileAds.Api;

public class optionsScreen : MonoBehaviour
{


	void Start ()
	{
		Cursor.visible = false;
		GM.InGame = false;
		Time.timeScale = 0;
		soundSlider.GetComponentInChildren<Slider> ().value = PlayerPrefs.GetFloat (GM.PP_sound);
		musicSlider.GetComponentInChildren<Slider> ().value = PlayerPrefs.GetFloat (GM.PP_music);

		#if UNITY_ANDROID
		visibleJoy.SetActive(true);
		if(PlayerPrefs.GetInt(GM.PP_joysticksVisible)==1){
			visibleJoy.GetComponent<Toggle>().isOn = true;
		}
#endif

		GM.ShowBanner();
	}

	//handles GUI back button
	public void back ()
	{
		if (GM.backToMain)
			ScreenManager.AddScreen (GameObject.Instantiate (Art.StartScreen));
		else 
			ScreenManager.AddScreen (GameObject.Instantiate (Art.PauseScreen));				
	}
	#if !UNITY_ANDROID
	//draws mouse
	void OnGUI ()
	{
		int cursorSizeX = 32;  // cursor size x
		int cursorSizeY = 32;  // cursor size y
		GUI.DrawTexture (new Rect (Event.current.mousePosition.x, Event.current.mousePosition.y, cursorSizeX, cursorSizeY), Art.Cursor);

	}
#endif
	public GameObject soundSlider;
	//handles GUI sound slider
	public void soundChange ()
	{	
		PlayerPrefs.SetFloat (GM.PP_sound, soundSlider.GetComponent<Slider> ().value);
		PlayerPrefs.Save ();
	}

	public GameObject musicSlider;
	//handles GUI music slider
	public void musicChange ()
	{
		PlayerPrefs.SetFloat (GM.PP_music, musicSlider.GetComponent<Slider> ().value);	
		GM.AudioMgr.GetComponent<AudioManager> ().updateMusicVol ();
		PlayerPrefs.Save ();
	}

	public GameObject visibleJoy;

	public void ChangeJoy ()
	{
	bool b =visibleJoy.GetComponent<Toggle>().isOn;
		if (b)
			PlayerPrefs.SetInt (GM.PP_joysticksVisible, 1);
		else
			PlayerPrefs.SetInt (GM.PP_joysticksVisible, 0);
		PlayerPrefs.Save ();


	}

}

