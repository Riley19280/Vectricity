using UnityEngine;
using System.Collections;

public class InputMethod
{

	//all the vars that the controller can have so that they can be accessed easily
	//this was used when controllers were supported, now they are just translated into keypresses in the unityinput class

	protected PlayerIndex player;
	protected bool btnA, btnB, btnBack, btnBigButton, btnDPadDown, btnDPadLeft, btnDPadRight, btnDPadUp, btnLeftShoulder, btnLeftStick, btnLeftThumbDown, btnLeftThumbLeft, btnLeftThumbRight, btnLeftThumbUp, btnLeftTrigger, btnRightShoulder, btnRightStick, btnRightThumbDown, btnRightThumbLeft, btnRightThumbRight, btnRightThumbUp, btnRightTrigger, btnStart, btnX, btnY;
	protected float thumbStickLeftX, thumbStickLeftY, thumbStickRightX, thumbStickRightY, thumbStickLeftDeg, thumbStickRightDeg;

    #region Properties

	public PlayerIndex SetPlayer {
		get { return player; }
		set { player = value; }
	}

    #region Triggers
	public float LeftTrigger { get; set; }

	public float RightTrigger { get; set; }
    #endregion

    #region Thumb stick value properties
	public float ThumbStickLeftX { get { return thumbStickLeftX; } }

	public float ThumbStickLeftY { get { return thumbStickLeftY; } }

	public float ThumbStickRightX { get { return thumbStickRightX; } }

	public float ThumbStickRightY { get { return thumbStickRightY; } }

	public float ThumbStickLeftDeg { get { return thumbStickLeftDeg; } }

	public float ThumbStickRightDeg { get { return thumbStickRightDeg; } }
    #endregion

    #region button properties
	public bool BtnA { get { return btnA; } }

	public bool BtnB { get { return btnB; } }

	public bool BtnX { get { return btnX; } }

	public bool BtnY { get { return btnY; } }

	public bool BtnBack { get { return btnBack; } }

	public bool BtnStart { get { return btnStart; } }

	public bool BtnBigButton { get { return btnBigButton; } }

	//dpad
	public bool BtnDpadUp { get { return btnDPadUp; } }

	public bool BtnDpadDown { get { return btnDPadDown; } }

	public bool BtnDpadLeft { get { return btnDPadLeft; } }

	public bool BtnDpadRight { get { return btnDPadRight; } }

	//left stick
	public bool BtnLeftThumbUp { get { return btnLeftThumbUp; } }

	public bool BtnLeftThumbDown { get { return btnLeftThumbDown; } }

	public bool BtnLeftThumbLeft { get { return btnLeftThumbLeft; } }

	public bool BtnLeftThumbRight { get { return btnLeftThumbRight; } }

	public bool BtnLeftShoulder { get { return btnLeftShoulder; } }

	public bool BtnLeftStick { get { return btnLeftStick; } }

	public bool BtnLeftTrigger { get { return btnLeftTrigger; } }

	//Right stick
	public bool BtnRightThumbUp { get { return btnRightThumbUp; } }

	public bool BtnRightThumbDown { get { return btnRightThumbDown; } }

	public bool BtnRightThumbLeft { get { return btnRightThumbLeft; } }

	public bool BtnRightThumbRight { get { return btnRightThumbRight; } }

	public bool BtnRightShoulder { get { return btnRightShoulder; } }

	public bool BtnRightStick { get { return btnRightStick; } }

	public bool BtnRightTrigger { get { return btnRightTrigger; } }

    #endregion

    #endregion
	public virtual void Update ()
	{
	}

	public virtual void Start ()
	{
	}

}
