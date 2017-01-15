using UnityEngine;
using System.Collections;

public class Delete : MonoBehaviour {

	void Start(){
		gameObject.transform.DetachChildren();
		Destroy(gameObject);
	}
}
