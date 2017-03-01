using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script enables camera tracking for the rocket. Attach
//it to the main camera in the gameplay screen.

public class Camera : MonoBehaviour {
	
	// Update is called once per frame
	public Transform target = null;
	int distance = -1000;
	static float lift = 17f;

	void LateUpdate() {
		transform.position = target.position + new Vector3(0,lift,distance);
		transform.LookAt(target);
	}

    public static void launchShift() {
        lift = -575f;
    }

    public static void reset() {
        lift = 17f;
    }

}
