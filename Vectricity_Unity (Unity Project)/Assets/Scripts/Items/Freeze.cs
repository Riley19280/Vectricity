using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Freeze : MonoBehaviour
{
	int destructTimer = GM.freezeTime;

	//self destruct
	void FixedUpdate ()
	{
		destructTimer--;

		if (destructTimer <= 0) {
			Destroy (gameObject);
		}
	}
}

