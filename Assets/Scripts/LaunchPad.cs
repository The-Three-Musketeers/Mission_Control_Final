using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchPad : MonoBehaviour {



    void Start () {
    }

	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            var animation = GetComponent<Animation>();
            animation.Play();
        }
	}
}
