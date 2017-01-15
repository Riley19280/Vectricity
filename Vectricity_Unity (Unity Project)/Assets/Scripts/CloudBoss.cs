using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class CloudBoss : Enemy
{
	int attTime = 15;
	GameObject target;
	List<bolt> bolts = new List<bolt> ();
	List<bolt> delBolts = new List<bolt> ();

	//this is the couldboss, has since been removed form the game

	void Start ()
	{
       

		damage = 0;
		value = 500;
		speed = 10;
      
		health = 20000 * difficulty;
		scale = 1.0f;

		speed = 4;
		damage = 75;
		isBoss = true;
	}

	public  void Update ()
	{
		if (health <= 0)
			Die ();

		if (attTime > 0)
			attTime--;
		if (attTime == 0) {
			Attack ();
			attTime = 60;
		}

		foreach (bolt b in bolts) {
			if (b.dead == true)
				delBolts.Add (b);
			else {
				//   b.Update();
			}
		}

		foreach (bolt b in delBolts) {
			bolts.Remove (b);
		}
		delBolts.Clear ();

		dmgTime--;
		if (dmgTime <= 0) {
			dmgTime = 0;
			isTakingDamage = false;
		}

		//update take damage
		if (isTakingDamage == true) {
			health -= dmgPerTick;

		} else {
			GetComponent<SpriteRenderer> ().color = Color.white;
		}


	}

	private void Attack ()
	{
		target = findTarget ();
		if (Vector2.Distance (target.transform.position, transform.position) < 400)
			Shoot (target);


	}

	private void Shoot (GameObject target)
	{
		//   bolts.Add(new bolt(transform.position, target));

	}

	private GameObject findTarget ()
	{
		return PlayerManager.player (UnityEngine.Random.Range (1, PlayerManager.players.Count));

	}

	class bolt : MonoBehaviour
	{
		public bool dead = false;
		Rect lightning;
		float rotation, damage = 50;
		int tmr = 4;
		Vector2 midLoc, moveVec, playerLoc;
      
		void Update ()
		{
		}
	}

}