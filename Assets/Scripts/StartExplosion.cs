using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script handles the initiation of the animation
//for the launch sequence. Attach it to the particle
//system that is to represent the launch explosion.

public class StartExplosion : MonoBehaviour {

    public static void Explode() {
        var fx = GameObject.Find("FX");
        //Check to make sure the particle effect exists
        if (fx == null) {
            return;
        }
        //Asseuming it does, play it and destroy the object
        //when it's done
        var particle = fx.GetComponent<ParticleSystem>();
        particle.Play();
        Destroy(fx, particle.main.duration);
    }
}
