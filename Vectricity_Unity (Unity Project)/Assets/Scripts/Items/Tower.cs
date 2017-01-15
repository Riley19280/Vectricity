using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class Tower : MonoBehaviour
{

	int alpha = 255;
	float scale = 1;
	int destructTimer = GM.towerTime;
	int shootTmr = 15;
	float rotation;

	//logic for shooting the enemy and self destructing when its time expires
	void FixedUpdate ()
	{

		destructTimer--;

		if (destructTimer <= 255)
			alpha--;
		if (alpha <= 0)
			alpha = 0;

		if (destructTimer <= 0) {
			Destroy (gameObject);
		}

		if (shootTmr > 0)
			shootTmr--;
		if (shootTmr == 0 && EnemyManager.GetCount () > 0) {
			shootTmr = 15;
			shoot ();
		}
	}

	void Update ()
	{
		//rotation manager
		//sets rotation to face the target
		if (EnemyManager.GetCount () > 0) {
			Vector3 fne = EnemyManager.getNearest (transform.position).transform.position;

			if (fne == Vector3.zero) {
				fne = PlayerManager.player (1).transform.position;
			}

			Vector3 diff = fne - transform.position;
			diff.Normalize ();

			float rot_z = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (0f, 0f, rot_z - 90), .75f);

		}
	}



	//fires a bullet
	private void shoot ()
	{
		Instantiate (Art.LaserT1, transform.position, transform.rotation);
		//- new Vector2(sprite.width / 2, sprite.height / 2)
	}


}