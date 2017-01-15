using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class EnemyManager
{
	static List<GameObject> enemies = new List<GameObject> ();

	public static void add (GameObject g)
	{
		enemies.Add (g);
	}

	public static void remove (GameObject g)
	{
		enemies.Remove (g);
	}

	public static void CLEARALL ()
	{
		enemies.Clear ();
		closest = null;
	}

	static GameObject closest;
	//gets the nearest enemies to the one requested
	public static GameObject getNearest (Vector3 pos)
	{
		float dist = Mathf.Infinity;
		try {
			foreach (GameObject g in enemies) {
				if (closest != null) {
					if (Vector3.Distance (closest.transform.position, g.transform.position) < dist) {
						closest = g;
						dist = Vector3.Distance (closest.transform.position, g.transform.position);
					}
				} else {
					if (g != null) {
						closest = g;
						dist = Vector3.Distance (closest.transform.position, g.transform.position);
					}
				}

			}
		} catch (System.Exception) {
			//should not happen
		}
		return closest;
	}

	public static int GetCount ()
	{
		return enemies.Count;
	}
}
