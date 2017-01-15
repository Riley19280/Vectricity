using UnityEngine;
using System.Collections;

public class MobileInput : InputMethod
{

	Vector2 leftStartPos, rightStartPos;
	Vector2 leftNorm, rightNorm;
	int leftIndex = -1, rightIndex = -1;

	public override void Start ()
	{
	
	}
	
	public override void  Update ()
	{
		UpdateTouches ();
		CalcVectors ();

	}

	GameObject leftGhost;
	GameObject rightGhost;

	void UpdateTouches ()
	{
		int ghostJoy = PlayerPrefs.GetInt (GM.PP_joysticksVisible);
		foreach (Touch t in Input.touches) {
			if (t.phase == TouchPhase.Began && t.position.x < GM.ScreenDims.x / 2 && t.fingerId < 2) {
				leftStartPos = t.position; 
				leftIndex = t.fingerId;
				if (ghostJoy == 1&& GM.InGame) {
					leftGhost = (GameObject)GameObject.Instantiate (Art.GhostJoy, leftStartPos, Quaternion.identity);
					leftGhost.transform.SetParent (GameObject.Find ("GameGUIScreen(Clone)").transform);
				}
			} else if (t.phase == TouchPhase.Began && t.position.x > GM.ScreenDims.x / 2 && t.fingerId < 2) {
				rightStartPos = t.position; 
				rightIndex = t.fingerId;
				if (ghostJoy == 1 && GM.InGame) {
					rightGhost = (GameObject)GameObject.Instantiate (Art.GhostJoy, rightStartPos, Quaternion.identity);
					rightGhost.transform.SetParent (GameObject.Find ("GameGUIScreen(Clone)").transform);
				}
			}

			if (t.phase == TouchPhase.Ended) {
				if (t.fingerId == leftIndex) {
					if (leftGhost != null)
						GameObject.Destroy (leftGhost);
				}
				if (t.fingerId == rightIndex) {
					if (rightGhost != null)
						GameObject.Destroy (rightGhost);
				}
			}

		}

		if(Input.touchCount==0){
			foreach(GameObject g in GameObject.FindGameObjectsWithTag("joystick")){
				GameObject.Destroy(g);
			}

		}
			

	}
		
	void CalcVectors ()
	{
		if (Input.touchCount > 0) {
			try {
				leftNorm = (Input.touches [leftIndex].position - leftStartPos).normalized;
				rightNorm = (Input.touches [rightIndex].position - rightStartPos).normalized;
			} catch (System.Exception ex) {
				
			}
				
			thumbStickLeftX = leftNorm.x;
			thumbStickLeftY = leftNorm.y;
			thumbStickRightX = rightNorm.x;
			thumbStickRightY = rightNorm.y;
				
				
		} else {
			leftNorm = Vector2.zero;
			rightNorm = Vector2.zero;
				
		}

		thumbStickLeftDeg = (float)Mathf.Atan2 (thumbStickLeftX, thumbStickLeftY) * Mathf.Rad2Deg;
		thumbStickRightDeg = (float)Mathf.Atan2 (-thumbStickRightX, thumbStickRightY) * Mathf.Rad2Deg;



	}
}