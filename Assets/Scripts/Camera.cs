using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	public Transform target = null;
	int distance = -1000;
	float lift = 17.0f;

	void LateUpdate() {
		transform.position = target.position + new Vector3(0,lift,distance);
		transform.LookAt(target);
	}

}
