using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class highscoreScreen : MonoBehaviour
{

	public GameObject scoreText;
	public GameObject killText;

	public GameObject avergeScoreText;
	public GameObject avergeKillsText;
	// Use this for initialization
	void Start ()
	{
		#if UNITY_ANDROID || UNITY_IPHONE
		GM.ShowInterstitial();
		GM.ShowBanner();
		#endif

		GM.InGame = false;
		Time.timeScale = 0;

		if(GM.maxPoints > PlayerPrefs.GetInt(GM.PP_highscore)){
			PlayerPrefs.SetInt(GM.PP_highscore,GM.maxPoints);
			scoreText.GetComponent<Text> ().text = "YOUR NEW HIGHSCORE IS <b><color=magenta>" + GM.maxPoints + "</color></b> POINTS!";
		} 
		else{
			scoreText.GetComponent<Text> ().text = "YOUR TOTAL SCORE WAS <b><color=green>" + GM.maxPoints + "</color></b>";
		}

		PlayerPrefs.SetInt(GM.PP_totalPoints,PlayerPrefs.GetInt(GM.PP_totalPoints)+GM.maxPoints);
		PlayerPrefs.SetInt(GM.PP_totalKills,PlayerPrefs.GetInt(GM.PP_totalKills)+GM.killCount);
		PlayerPrefs.SetInt(GM.PP_gameCount,PlayerPrefs.GetInt(GM.PP_gameCount)+1);

		PlayerPrefs.Save();


		killText.GetComponent<Text> ().text = "<b><color=red>" + GM.killCount + "</color></b> ENEMIES DIED";
		avergeScoreText.GetComponent<Text> ().text = "Averge Score: <b><color=blue>" + (PlayerPrefs.GetInt(GM.PP_totalPoints)/PlayerPrefs.GetInt(GM.PP_gameCount)) + "</color></b>";
		avergeKillsText.GetComponent<Text> ().text = "Averge Kills: <b><color=blue>" + (PlayerPrefs.GetInt(GM.PP_totalKills)/PlayerPrefs.GetInt(GM.PP_gameCount)) + "</color></b>";




	}
	
	//handles GUI main menu button
	public void MainMenu ()
	{
		GM.InGame = false;
		Time.timeScale = 0;
		ScreenManager.AddScreen (GameObject.Instantiate (Art.StartScreen));
	}

	//handles GUI play again button
	public void PlayAgain ()
	{
		GM.NEWGAME ();
	}

	//handles mouse draw
	#if !UNITY_ANDROID || UNITY_IPHONE
	void OnGUI ()
	{
		int cursorSizeX = 32;  
		int cursorSizeY = 32;  
		GUI.DrawTexture (new Rect (Event.current.mousePosition.x, Event.current.mousePosition.y, cursorSizeX, cursorSizeY), Art.Cursor);
	}
#endif
}
