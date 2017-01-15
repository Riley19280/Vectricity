using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using GoogleMobileAds.Api;

public class PlayerShip : MonoBehaviour
{
	//the main player

    #region Vars
	System.Random rand = new System.Random ();

	public Color Color { get; protected set; }

	public PlayerIndex playerIndex;
	public float Health = 100;
	public float speed;
	public float	baseSpeed = 10, sprintSpeed = 40, baseSprintSpeed = 40;
	protected int bulletSprayCooldown = 0, sprintCooldown = 0;
	public int fireRate = 5;
	int shootTmr = 0;
	int secTmr = 60;
	public enum Ammo
	{
		B1,
		B2,
		B3,
		B4,
		B5,
		L1,
		L2,
		L3,
		L4,
		L5,
		M1,
		M2,
		M3,
		M4,
		M5
	}
	public Ammo ammo;

	public int BulletTier { get; set; }

	public int LaserTier { get; set; }

	public int MissileTier { get; set; }

	private float myWidth;
	private float myHeight;

    #endregion

	public void Start ()
	{

		speed = baseSpeed;
		transform.position = Vector3.zero;// GM.ScreenDims / 2;

		myWidth = GetComponent<SpriteRenderer> ().bounds.size.x;
		myHeight = GetComponent<SpriteRenderer> ().bounds.size.y;
		Health = GM.maxHealth;
		baseSpeed = 10;
		sprintSpeed = 50;
		baseSprintSpeed = 50;
		fireRate = 5;

		//set the player color
		#region pick color
		switch (playerIndex) {
		case PlayerIndex.One:
			Color = Color.red;
			break;
		case PlayerIndex.Two:
			Color = Color.green;
			break;

		case PlayerIndex.Three:
			Color = Color.blue;
			break;
		case PlayerIndex.Four:
			Color = Color.yellow;
			break;
		}
		#endregion

		BulletTier = 1;
		LaserTier = 1;
		MissileTier = 1;
		ammo = Ammo.B1;
	

	}

	void FixedUpdate ()
	{

		ShootControls ();

	}

	public void Update ()
	{

		if (secTmr == 0) {
			secTmr = 60;
		}
		secTmr--;

		RegenAmt ();
		UpdateHealth ();
		ControlPlayer ();

		BulletSpray ();
		Sprint ();
		CheckBuyMenu ();
		DoPowerups ();

		//keep player in view

		var left = Camera.main.ViewportToWorldPoint(Vector3.zero).x;
		var right = Camera.main.ViewportToWorldPoint(Vector3.one).x;
		var top = Camera.main.ViewportToWorldPoint(Vector3.zero).y;
		var bottom = Camera.main.ViewportToWorldPoint(Vector3.one).y;
		float x = transform.position.x, y = transform.position.y;
		var renderer =gameObject.GetComponent<SpriteRenderer>();

		if (transform.position.x <= left + renderer.bounds.extents.x) {
			x = left + renderer.bounds.extents.x;
		} else if (transform.position.x >= right - renderer.bounds.extents.x) {
			x = right - renderer.bounds.extents.x;
		}
		if (transform.position.y <= top + renderer.bounds.extents.y) {
			y = top + renderer.bounds.extents.y;
		} else if (transform.position.y >= bottom - renderer.bounds.extents.y) {
			y = bottom - renderer.bounds.extents.y;
		}
		transform.position = new Vector3(x, y, transform.position.z);

	}
//handles collisions with enemies and enemy bullets
	void OnCollisionEnter2D (Collision2D coll)
	{

		if (coll.transform.tag == "enemy") {
			coll.gameObject.GetComponent<Enemy> ().MeeleeAttack (gameObject);
		}

		if (coll.transform.tag == "bullet") {
			if (!coll.gameObject.GetComponent<baseBullet> ().isPlayer) {
				TakeDamage (coll.gameObject.GetComponent<baseBullet> ().damage);
			}
		}
	}

	void OnCollisionStay2D (Collision2D coll)
	{
		if (coll.gameObject.tag == "enemy") {
			coll.gameObject.GetComponent<Enemy> ().MeeleeAttack (gameObject);
		}


	}

    #region Methods

	//checking if the player has used any powerups or placed items
	private void DoPowerups ()
	{
		//time bomb
		if (InputManager.PlayerInput (playerIndex).BtnDpadDown) {
			if (GM.Points >= 1000) {
				GameObject.Find ("GameGUIScreen(Clone)").GetComponent<GameGUI> ().DoTimeBomb ();
				GM.Points -= 1000;
			}
		}
		//freeze
		if (InputManager.PlayerInput (playerIndex).BtnDpadLeft) {
			if (GM.Points >= 25) {
				GameObject.Instantiate (Art.Freeze, transform.position, Quaternion.identity);

				GM.Points -= 25;
			}
		}
		//firewall
		if (InputManager.PlayerInput (playerIndex).BtnDpadRight) {
			if (GM.Points >= 50) {
				GameObject.Instantiate (Art.Firewall, transform.position, Quaternion.identity);
				GM.Points -= 50;
			}
		}

		//tower
		if (InputManager.PlayerInput (playerIndex).BtnDpadUp) {
			if (GM.Points >= 75) {
				GameObject.Instantiate (Art.Tower, transform.position, Quaternion.identity);
				GM.Points -= 75;
			}
		}
	}

	public PlayerIndex SetIndex {
		get { return playerIndex; }
		set { playerIndex = value; }
	}

	//regenerates thje players health
	private void RegenAmt ()
	{
		if (secTmr == 0) {
			Health += GM.regenerationAmt;
		}
	}

	//handles the PlayerShip sprint ability
	private void Sprint ()
	{
		//sprint
		if (InputManager.PlayerInput (playerIndex).BtnLeftStick) {
			if (sprintCooldown <= 0) {
				sprintSpeed = baseSprintSpeed;
				sprintCooldown = 500;
				GM.AudioMgr.GetComponent<AudioManager> ().playSoundEffect (Art.Woosh);
			}
		}

		sprintCooldown--;
		if (sprintCooldown < 0) {
			sprintCooldown = 0;
		}

		if (sprintSpeed != baseSpeed) {
			sprintSpeed--;
			speed = sprintSpeed;
		}

	
	}

	//checking if they have opened the buy menu
	private void CheckBuyMenu ()
	{
		//buymenu
		if (GM.InGame == true)
		if (InputManager.PlayerInput (playerIndex).BtnX) {
			ScreenManager.AddScreen (GameObject.Instantiate (Art.BuyScreen));
		}

	}

	//doing the 360 degree bullet spray
	private void BulletSpray ()
	{
		//bullet spray
		if (InputManager.PlayerInput (playerIndex).BtnRightStick) {

			if (bulletSprayCooldown == 0) {
				for (int i = 1; i <= 360; i += 15) {

					SWITCH_AMMO (Quaternion.Euler (0, 0, i));

					GM.AudioMgr.GetComponent<AudioManager> ().playSoundEffect (Art.Cannon);
				}
			}

			bulletSprayCooldown = 600;
		}
		if (bulletSprayCooldown > 0) {
			bulletSprayCooldown--;
		}
	}

	//handles shooting the right bullet based on ammo type
	private void ShootControls ()
	{
		//shoot controls
#if UNITY_ANDROID
		shootTmr--;
		
		if (shootTmr <= 0) {
			SWITCH_AMMO(transform.localRotation);
				
				shootTmr = fireRate/2;
		}
#else
		if ((((Input.GetMouseButton (0))) && Time.timeScale != 0)) {

			shootTmr--;

			if (shootTmr <= 0) {
				SWITCH_AMMO (transform.localRotation);

				shootTmr = fireRate;
			}
		}
		#endif
	}

	void SWITCH_AMMO (Quaternion q)
	{
		switch (ammo) {
		case Ammo.B1:
			GameObject.Instantiate (Art.BulletT1, transform.position, q);
			break;
		case Ammo.B2:
			GameObject.Instantiate (Art.BulletT2, transform.position, q);
			break;
		case Ammo.B3:
			GameObject.Instantiate (Art.BulletT3, transform.position, q);
			break;
		case Ammo.B4:
			GameObject.Instantiate (Art.BulletT4, transform.position, q);
			break;
		case Ammo.B5:
			GameObject.Instantiate (Art.BulletT5, transform.position, q);
			break;
		case Ammo.L1:
			GameObject.Instantiate (Art.LaserT1, transform.position, q);
			break;
		case Ammo.L2:
			GameObject.Instantiate (Art.LaserT2, transform.position, q);
			break;
		case Ammo.L3:
			GameObject.Instantiate (Art.LaserT3, transform.position, q);
			break;
		case Ammo.L4:
			GameObject.Instantiate (Art.LaserT4, transform.position, q);
			break;
		case Ammo.L5:
			GameObject.Instantiate (Art.LaserT5, transform.position, q);
			break;
		case Ammo.M1:
			GameObject.Instantiate (Art.MissileT1, transform.position, q);
			break;
		case Ammo.M2:
			GameObject.Instantiate (Art.MissileT2, transform.position, q);
			break;
		case Ammo.M3:
			GameObject.Instantiate (Art.MissileT3, transform.position, q);
			break;
		case Ammo.M4:
			GameObject.Instantiate (Art.MissileT4, transform.position, q);
			break;
		case Ammo.M5:
			GameObject.Instantiate (Art.MissileT5, transform.position, q);
			break;
		}
	}

	//hanedles player movement controls
	private void ControlPlayer ()
	{
#if UNITY_ANDROID
		int total = 0;

		foreach(Touch t in Input.touches){
			if(t.position.x <GM.ScreenDims.x/2)
				total++;
		}


		if(total>0){
		transform.position += new Vector3 (InputManager.PlayerInput(playerIndex).ThumbStickLeftX * speed, InputManager.PlayerInput(playerIndex).ThumbStickLeftY * speed, 0) * Time.deltaTime;
		}
		//rotation manager
		
		float angle = InputManager.PlayerInput(playerIndex).ThumbStickRightDeg;
		transform.rotation = Quaternion.Euler (new Vector3 (0, 0, angle));


#else

		if (InputManager.PlayerInput (1).BtnRightThumbUp) {
			transform.position += new Vector3 (0, speed, 0) * Time.deltaTime;
		}
		if (InputManager.PlayerInput (1).BtnRightThumbDown) {
			transform.position += new Vector3 (0, -speed, 0) * Time.deltaTime;
		}
		if (InputManager.PlayerInput (1).BtnRightThumbLeft) {
			transform.position += new Vector3 (-speed, 0) * Time.deltaTime;
		}
		if (InputManager.PlayerInput (1).BtnRightThumbRight) {
			transform.position += new Vector3 (speed, 0, 0) * Time.deltaTime;
		}

		//rotation manager

		Vector3 mousePos = Input.mousePosition;
		mousePos.z = -10;

		Vector3 objectPos = Camera.main.WorldToScreenPoint (transform.position);
		mousePos.x = mousePos.x - objectPos.x;
		mousePos.y = mousePos.y - objectPos.y;

		float angle = (Mathf.Atan2 (mousePos.y, mousePos.x) - (float)Math.PI / 2) * Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler (new Vector3 (0, 0, angle));

#endif



	}

	public void Die ()
	{
		Destroy (gameObject);
		GM.InGame = false;
		Time.timeScale = 0;


		ScreenManager.AddScreen (GameObject.Instantiate (Art.HighscoreScreen));
	}

	public void TakeDamage (float dmg)
	{
		Health -= dmg;
	}

	private void UpdateHealth ()
	{
		if (Health > GM.maxHealth) {
			Health = GM.maxHealth;
		}
		if (Health <= 0)
			Die ();


	}

    #endregion


}
