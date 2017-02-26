using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script enables camera tracking for the rocket. Attach
//it to the main camera in the gameplay screen.

public class Camera : MonoBehaviour {
	
	// Update is called once per frame
	public Transform target = null;
	int distance = -1000;
	float lift = 17.0f;

	void LateUpdate() {
		transform.position = target.position + new Vector3(0,lift,distance);
		transform.LookAt(target);
	}

}
