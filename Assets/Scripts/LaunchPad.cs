using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script listens for the return key, plays the animation,
//and signals to the RocketBehavior class to launch the rocket
//when it is done.
public class LaunchPad : MonoBehaviour {

    public static bool animationDone = false;
    bool button_hit_flag = false;
    public float animationSpeed;
    public Animation _animation;

    void Start () {
        _animation = GetComponent<Animation>();
        animationSpeed = 5f;
    }

	// Update is called once per frame
    void Update () {
        Animation animation = GetComponent<Animation>();
        if (Input.GetKeyDown(KeyCode.Return)) {
	    _animation["Expand"].speed = animationSpeed;
            _animation.Play();
            RocketBehavior.launch = true;
            button_hit_flag = true;
        }
        //When it's done, signal to RocketBehavior.cs that it is done.
        if (button_hit_flag == true && _animation.isPlaying == false) {
            animationDone = true;
            button_hit_flag = false;
        }
    }

    public static void reset() {
        animationDone = false;
    }
}
