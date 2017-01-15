using UnityEngine;
using System.Collections.Generic;
using System.Collections;

class UnityInput : InputMethod
{
	//the special input management that feets the unity controls the the custom input manager

   public override void  Update()
    {


        if (Input.GetKeyDown(KeyCode.Return)) { btnA = true; } else { btnA = false; }
        if (Input.GetKeyDown(KeyCode.Escape)) { btnB = true; } else { btnB = false; }

        if (Input.GetKey(KeyCode.W) || (Input.GetKeyDown(KeyCode.UpArrow))) { btnRightThumbUp = true; } else { btnRightThumbUp = false; }
        if (Input.GetKey(KeyCode.S) || (Input.GetKeyDown(KeyCode.DownArrow))) { btnRightThumbDown = true; } else { btnRightThumbDown = false; }
        if (Input.GetKey(KeyCode.A) || (Input.GetKeyDown(KeyCode.LeftArrow))) { btnRightThumbLeft = true; } else { btnRightThumbLeft = false; }
        if (Input.GetKey(KeyCode.D) || (Input.GetKeyDown(KeyCode.RightArrow))) { btnRightThumbRight = true; } else { btnRightThumbRight = false; }

        if (Input.GetKeyDown(KeyCode.Escape)) { btnStart = true; } else { btnStart = false; }
        if (Input.GetKeyDown(KeyCode.Escape)) { btnBack = true; } else { btnBack = false; }

        //bullet spray
        if (Input.GetKeyDown(KeyCode.Q)) { btnRightStick = true; } else { btnRightStick = false; }
        //inventory
        if (Input.GetKeyDown(KeyCode.E)) { btnX = true; } else { btnX = false; }
        //sprint
        if (Input.GetKeyDown(KeyCode.Space)) { btnLeftStick = true; } else { btnLeftStick = false; }
        //unused / towers
        if (Input.GetKeyDown(KeyCode.Z)) { btnDPadUp = true; } else { btnDPadUp = false; }
        //freeze
        if (Input.GetKeyDown(KeyCode.X)) { btnDPadLeft = true; } else { btnDPadLeft = false; }
        //fire
        if (Input.GetKeyDown(KeyCode.C)) { btnDPadRight = true; } else { btnDPadRight = false; }
        //nuke
        if (Input.GetKeyDown(KeyCode.V)) { btnDPadDown = true; } else { btnDPadDown = false; }


    }


}