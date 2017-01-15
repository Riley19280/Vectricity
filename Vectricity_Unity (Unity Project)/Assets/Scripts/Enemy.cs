using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class Enemy : MonoBehaviour
{
	//refactor and organize theese vars
    #region Vars
	public string TYPE = "";
	public bool isBoss = false;
	public int value;
	public bool shoots = false;
	public float health = 100, damage = 10;
	public float speed = 6;
	protected bool IsDead = false;
	protected float difficulty = 1;
	protected float scale = 1.0f;
	protected bool isTakingDamage;
	protected float dmgPerTick;
	protected float dmgTime;
	protected bool tarDead = false;
	protected int atkCooldown = 0;
	bool needsDestroy = false;

    #endregion


	public void Start ()
	{
		health *= GM.Difficulty;
		EnemyManager.add (gameObject);
	}

	void FixedUpdate ()
	{

		CheckDead ();

		//update meelee timer
		atkCooldown--;
		if (atkCooldown < 0)
			atkCooldown = 0;

		//update take damage timer


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

		//DO MOVEMENT
		FollowPlayer ();

		//change type of bullet shot based on wave number
		if (shoots && GM.InGame) {
			if (atkCooldown <= 0) {
				if (WaveManager.waveNum < 10) {
					GameObject g = (GameObject)GameObject.Instantiate (Art.Bolt, transform.position, transform.rotation);
					g.GetComponent<baseBullet> ().isPlayer = false;
					g.GetComponent<baseBullet> ().damage= 25;
					foreach (baseBullet c in g.GetComponentsInChildren<baseBullet>()) {
						c.isPlayer = false;
					}
				} else if (WaveManager.waveNum >= 10 && WaveManager.waveNum <= 20) {
					GameObject g = (GameObject)GameObject.Instantiate (Art.Bolt, transform.position, transform.rotation);
					g.GetComponent<baseBullet> ().isPlayer = false;
					g.GetComponent<baseBullet> ().damage= 50;
					foreach (baseBullet c in g.GetComponentsInChildren<baseBullet>()) {
						c.isPlayer = false;
					}
				} else {
					GameObject g = (GameObject)GameObject.Instantiate (Art.Bolt, transform.position, transform.rotation);
					g.GetComponent<baseBullet> ().isPlayer = false;
					g.GetComponent<baseBullet> ().damage= 75;
					foreach (baseBullet c in g.GetComponentsInChildren<baseBullet>()) {
						c.isPlayer = false;
					}
				}
				atkCooldown = 75;
			}
           
		}


	}

	void LateUpdate ()
	{
		if (needsDestroy)
			Destroy (gameObject);
	}

    #region behavours

	void OnCollisionEnter2D (Collision2D coll)
	{
		if (coll.transform.tag == "bullet") {
			if (coll.gameObject.GetComponent<baseBullet> ().isPlayer) {
				if (coll.gameObject.GetComponent<baseBullet> ().isAOE)
					coll.gameObject.GetComponent<baseBullet> ().AOE ();
				else
					TakeDamage (coll.gameObject.GetComponent<baseBullet> ().damage);

				if (coll.gameObject.GetComponent<baseBullet> ().isFire) {
					TakeDamage (50, 180);
				}
				if (!coll.gameObject.GetComponent<baseBullet> ().penetrates)
					Destroy (coll.gameObject);
			}

		}

		if (coll.gameObject.layer == 11) {

			if (coll.gameObject.GetComponent<Firewall> () != null) {
				TakeDamage (50, 180);
			} else if (coll.gameObject.GetComponent<Freeze> () != null) {
				speed = 4;
			}
		}
	}

	protected void FollowPlayer ()
	{

		try {

			if (PlayerManager.player (1) != null) {
				tarDead = false;

			} else
				tarDead = true;
		} catch (System.ArgumentNullException e) {

		}


		if (!tarDead) {

			//rotation manager
			Vector3 diff = PlayerManager.player (1).transform.position - transform.position;
			diff.Normalize ();

			float rot_z = Mathf.Atan2 (diff.y, diff.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler (0f, 0f, rot_z - 90);

			//transform.position += -transform.up * speed* Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, PlayerManager.player (1).transform.position, speed * Time.deltaTime);
		} else {

		}
	}

	public void MeeleeAttack (GameObject pl)
	{

		if (atkCooldown <= 0) {
			pl.GetComponent<PlayerShip> ().TakeDamage (damage);
			atkCooldown = 30;
		}

	}


    #endregion

	private void CheckDead ()
	{
		if (health <= 0) {
			Die ();
		}
	}

	//what to do if the enemy dies, depending on type
	float minX = Camera.main.ViewportToWorldPoint (new Vector2 (0, 0)).x;
	float maxX = Camera.main.ViewportToWorldPoint (new Vector2 (1, 1)).x;
	float minY = Camera.main.ViewportToWorldPoint (new Vector2 (0, 0)).y;
	float maxY = Camera.main.ViewportToWorldPoint (new Vector2 (1, 1)).y;

	public void Die ()
	{
		EnemyManager.remove (gameObject);
		int offset = 5;

		if (TYPE == "splitter") {
			Instantiate (Art.Triangle, new Vector2 (Random.Range (minX, maxX), Random.Range (minY, maxY)), Quaternion.identity);
			Instantiate (Art.Triangle, new Vector2 (Random.Range (minX, maxX), Random.Range (minY, maxY)), Quaternion.identity);
			Instantiate (Art.Triangle, new Vector2 (Random.Range (minX, maxX), Random.Range (minY, maxY)), Quaternion.identity);
			Instantiate (Art.Triangle, new Vector2 (Random.Range (minX, maxX), Random.Range (minY, maxY)), Quaternion.identity);
			Instantiate (Art.Triangle, new Vector2 (Random.Range (minX, maxX), Random.Range (minY, maxY)), Quaternion.identity);
			Instantiate (Art.Triangle, new Vector2 (Random.Range (minX, maxX), Random.Range (minY, maxY)), Quaternion.identity);

		}
		if (TYPE == "splitter2") {
			Instantiate (Art.Splitter, new Vector2 (Random.Range (minX, maxX), Random.Range (minY, maxY)), Quaternion.identity);
			Instantiate (Art.Splitter, new Vector2 (Random.Range (minX, maxX), Random.Range (minY, maxY)), Quaternion.identity);
			Instantiate (Art.Splitter, new Vector2 (Random.Range (minX, maxX), Random.Range (minY, maxY)), Quaternion.identity);
		}
		GM.Points += value;
		GM.maxPoints += value;
		GM.killCount ++;
		needsDestroy = true;
	}

	//set amount damage taking
	public void TakeDamage (float dmg)
	{
		health -= dmg;
	}

	//damage taking over time
	public void TakeDamage (float dmg, float time)
	{
		isTakingDamage = true;
		dmgPerTick = dmg / time;
		dmgTime = time;
		GetComponent<SpriteRenderer> ().color = Color.red;
	}

}