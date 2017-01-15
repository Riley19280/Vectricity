using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Firewall : MonoBehaviour
{
	int destructTimer = GM.firewallTime;

	//self destruct
	void FixedUpdate ()
	{
		destructTimer--;

		if (destructTimer <= 0) {
			Destroy (gameObject);
		}
	}
}