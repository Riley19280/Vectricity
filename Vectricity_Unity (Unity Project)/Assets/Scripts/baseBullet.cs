using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class baseBullet : MonoBehaviour
{

    #region Vars

	public float speed = 15f;
	public float damage = 20;
	public bool isFire = false;
	public bool isAOE = false;
	public bool isPlayer = true;
	public bool penetrates = false;
	public bool tracksEnemy = false;
	public bool bounces = false;
	public float AOERADIUS = 1;

	int maxBounces = 4;
	int bounceCount = 0;

	int destructTimer = 600;

    #endregion
	GameObject nearEnemy;

	void Start(){
		gameObject.transform.DetachChildren();
	}

	Vector3 lastPos;

	void FixedUpdate ()
	{
		destructTimer--;
		if(destructTimer<0)
			Die();

		//doing different things based on attributes of the nullet
		if (tracksEnemy)
			nearEnemy = EnemyManager.getNearest (transform.position);

		Vector3 pos = Camera.main.WorldToViewportPoint (transform.position);

		//Vector3 velVec = transform.up;

		if (pos.x < 0 || pos.x > 1 || pos.y < 0 || pos.y > 1) {
			if (!bounces) {
				Die ();
			} else {
				speed *=-1;

				if(bounceCount >maxBounces)
					Destroy(gameObject);
				bounceCount++;
			}

		}
		lastPos = transform.position;


		//moving the bullet
		if (tracksEnemy && nearEnemy != null) {

			Vector3 t = nearEnemy.transform.position - transform.position;

			t.Normalize ();
			transform.position += t;

		} else {
			transform.position +=transform.up * speed * Time.deltaTime;
		
		}

	}

	public void Die ()
	{  
		try {
			if (gameObject.transform.parent.GetComponent<Delete> () != null && gameObject.transform.parent != null)
				Destroy (gameObject.transform.parent.gameObject);
		} catch (Exception ex) {
		}

		Destroy (gameObject); 
	}

	//doing area of effect damage
	public void AOE ()
	{
		foreach (GameObject e in NearestEnemys(AOERADIUS)) {
			if (e.tag == "enemy") {
				e.GetComponent<Enemy> ().TakeDamage (0, 10);
				e.GetComponent<Enemy> ().TakeDamage (damage);
			}
		}
	}

	//getting all the nearest enemies
	public List<GameObject> NearestEnemys (float rad)
	{
		List<GameObject> toAtt = new List<GameObject> ();

		RaycastHit2D[] hits = Physics2D.CircleCastAll (transform.position, rad, transform.position);

		foreach (RaycastHit2D hit in hits) {
			toAtt.Add (hit.transform.gameObject);
		}


		return toAtt;

	}

}
