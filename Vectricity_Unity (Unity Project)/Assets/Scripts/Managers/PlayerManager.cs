using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public static class PlayerManager
{
	//used to handle the management of multiople players, kept for ease of adding more players on the future

	public static List<GameObject> players = new List<GameObject> ();
	static GameObject player1;
	static GameObject player2;
	static GameObject player3;
	static GameObject player4;

	public static void Initialize ()
	{
		if (player1 == null) {
			player1 = GameObject.Instantiate (Art.PlayerGO);
			player1.GetComponent<PlayerShip> ().playerIndex = PlayerIndex.One;
			if (!players.Contains (player1))
				players.Add (player1);
		}
	}

	public static void RespawnPlayers ()
	{
		Initialize ();

	}

	public static GameObject player (PlayerIndex pl)
	{
		switch (pl) {
		case PlayerIndex.One:
			return player1;
		case PlayerIndex.Two:
			return player2;
		case PlayerIndex.Three:
			return player3;
		case PlayerIndex.Four:
			return player4;
		default:
			return null;
		}

	}

	/// <summary>
	/// returns player index from 1 to 4
	/// </summary>
	/// <param name="pl"></param>
	/// <returns></returns>
	public static GameObject player (int pl)
	{
		switch (pl) {
		case 1:
			return player1;
		case 2:
			return player2;
		case 3:
			return player3;
		case 4:
			return player4;
		default:
			return null;
		}


	}
}