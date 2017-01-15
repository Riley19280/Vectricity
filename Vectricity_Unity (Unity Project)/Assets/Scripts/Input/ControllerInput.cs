using UnityEngine;
using System.Collections;

public class ControllerInput : InputMethod {
	/*
	GamePadState pad;
	GamePadState oldPad;
	
	//polls the controller for its values and sets them in the main class
	public override void Update(GameTime gameTime)
	{
		pad = GamePad.GetState(player);
		
		#region Button Presses
		///for anyone reading this, this is the worst way to do this but the only way i could find after 7 hours
		
		//other
		if (pad.IsButtonDown(Buttons.A) && oldPad.IsButtonUp(Buttons.A)) { btnA = true; } else { btnA = false; }
		
		if (pad.IsButtonDown(Buttons.B) && !oldPad.IsButtonDown(Buttons.B)) { btnB = true; } else { btnB = false; }
		
		if (pad.IsButtonDown(Buttons.X) && !oldPad.IsButtonDown(Buttons.X)) { btnX = true; } else { btnX = false; }
		
		if (pad.IsButtonDown(Buttons.Y) && !oldPad.IsButtonDown(Buttons.Y)) { btnY = true; } else { btnY = false; }
		
		if (pad.IsButtonDown(Buttons.Back) && !oldPad.IsButtonDown(Buttons.Back)) { btnBack = true; } else { btnBack = false; }
		
		if (pad.IsButtonDown(Buttons.Start) && !oldPad.IsButtonDown(Buttons.Start)) { btnStart = true; } else { btnStart = false; }
		
		if (pad.IsButtonDown(Buttons.BigButton) && !oldPad.IsButtonDown(Buttons.BigButton)) { btnBigButton = true; } else { btnBigButton = false; }
		
		if (pad.IsButtonDown(Buttons.DPadDown) && !oldPad.IsButtonDown(Buttons.DPadDown)) { btnDPadDown = true; } else { btnDPadDown = false; }
		
		if (pad.IsButtonDown(Buttons.DPadLeft) && !oldPad.IsButtonDown(Buttons.DPadLeft)) { btnDPadLeft = true; } else { btnDPadLeft = false; }
		
		if (pad.IsButtonDown(Buttons.DPadRight) && !oldPad.IsButtonDown(Buttons.DPadRight)) { btnDPadRight = true; } else { btnDPadRight = false; }
		
		if (pad.IsButtonDown(Buttons.DPadUp) && !oldPad.IsButtonDown(Buttons.DPadUp)) { btnDPadUp = true; } else { btnDPadUp = false; }
		
		
		
		
		//left
		if (pad.IsButtonDown(Buttons.LeftShoulder) && !oldPad.IsButtonDown(Buttons.LeftShoulder)) { btnLeftShoulder = true; } else { btnLeftShoulder = false; }
		
		if (pad.IsButtonDown(Buttons.LeftStick) && !oldPad.IsButtonDown(Buttons.LeftStick)) { btnLeftStick = true; } else { btnLeftStick = false; }
		
		if (pad.IsButtonDown(Buttons.LeftThumbstickDown) && !oldPad.IsButtonDown(Buttons.LeftThumbstickDown)) { btnLeftThumbDown = true; } else { btnLeftThumbDown = false; }
		
		if (pad.IsButtonDown(Buttons.LeftThumbstickLeft) && !oldPad.IsButtonDown(Buttons.LeftThumbstickLeft)) { btnLeftThumbLeft = true; } else { btnLeftThumbLeft = false; }
		
		if (pad.IsButtonDown(Buttons.LeftThumbstickRight) && !oldPad.IsButtonDown(Buttons.LeftThumbstickRight)) { btnLeftThumbRight = true; } else { btnLeftThumbRight = false; }
		
		if (pad.IsButtonDown(Buttons.LeftThumbstickUp) && !oldPad.IsButtonDown(Buttons.LeftThumbstickUp)) { btnLeftThumbUp = true; } else { btnLeftThumbUp = false; }
		
		if (pad.IsButtonDown(Buttons.LeftTrigger) && !oldPad.IsButtonDown(Buttons.LeftTrigger)) { btnLeftTrigger = true; } else { btnLeftTrigger = false; }
		
		
		
		//right
		if (pad.IsButtonDown(Buttons.RightShoulder) && !oldPad.IsButtonDown(Buttons.RightShoulder)) { btnRightShoulder = true; } else { btnRightShoulder = false; }
		
		if (pad.IsButtonDown(Buttons.RightStick) && !oldPad.IsButtonDown(Buttons.RightStick)) { btnRightStick = true; } else { btnRightStick = false; }
		
		if (pad.IsButtonDown(Buttons.RightThumbstickDown) && !oldPad.IsButtonDown(Buttons.RightThumbstickDown)) { btnRightThumbDown = true; } else { btnRightThumbDown = false; }
		
		if (pad.IsButtonDown(Buttons.RightThumbstickLeft) && !oldPad.IsButtonDown(Buttons.RightThumbstickLeft)) { btnRightThumbLeft = true; } else { btnRightThumbLeft = false; }
		
		if (pad.IsButtonDown(Buttons.RightThumbstickRight) && !oldPad.IsButtonDown(Buttons.RightThumbstickRight)) { btnRightThumbRight = true; } else { btnRightThumbRight = false; }
		
		if (pad.IsButtonDown(Buttons.RightThumbstickUp) && !oldPad.IsButtonDown(Buttons.RightThumbstickUp)) { btnRightThumbUp = true; } else { btnRightThumbUp = false; }
		
		if (pad.IsButtonDown(Buttons.RightTrigger) && !oldPad.IsButtonDown(Buttons.RightTrigger)) { btnRightTrigger = true; } else { btnRightTrigger = false; }
		
		
		#endregion
		thumbStickLeftX = GamePad.GetState(player).ThumbSticks.Left.X;
		thumbStickLeftY = GamePad.GetState(player).ThumbSticks.Left.Y;
		thumbStickRightX = GamePad.GetState(player).ThumbSticks.Right.X;
		thumbStickRightY = GamePad.GetState(player).ThumbSticks.Right.Y;
		thumbStickLeftDeg = (float)Math.Atan2(thumbStickLeftX, thumbStickLeftY);
		thumbStickRightDeg = (float)Math.Atan2(thumbStickRightX, thumbStickRightY);
		LeftTrigger = GamePad.GetState(player).Triggers.Left;
		RightTrigger = GamePad.GetState(player).Triggers.Right;
		
		oldPad = pad;*/
}
