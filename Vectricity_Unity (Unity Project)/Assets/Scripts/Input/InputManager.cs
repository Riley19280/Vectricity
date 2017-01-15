using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public static class InputManager
{
	static InputMethod p1Input, p2Input, p3Input, p4Input;

	public static void Update ()
	{
		//update players inputs
		if (p1Input != null)
			p1Input.Update ();
		if (p2Input != null)
			p2Input.Update ();
		if (p3Input != null)
			p3Input.Update ();
		if (p4Input != null)
			p4Input.Update ();
	}

	public static InputMethod PlayerInput (PlayerIndex player)
	{
		switch (player) {
		case PlayerIndex.One:
			return p1Input;

		case PlayerIndex.Two:
			return p2Input;

		case PlayerIndex.Three:
			return p3Input;

		case PlayerIndex.Four:
			return p4Input;
		}

		return null;

	}

	/// <summary>
	/// get a player from a number
	/// </summary>
	/// <param name="player"></param>
	/// <returns></returns>
	public static InputMethod PlayerInput (int player)
	{
		switch (player) {
		case 1:
			return p1Input;

		case 2:
			return p2Input;

		case 3:
			return p3Input;

		case 4:
			return p4Input;
		}

		return null;
	}

	public static void addPlayer1 ()
	{
#if UNITY_ANDROID
		p1Input = new MobileInput();
		p1Input.Start();
#else
		p1Input = new UnityInput ();

#endif

		p1Input.SetPlayer = PlayerIndex.One;
	}
}