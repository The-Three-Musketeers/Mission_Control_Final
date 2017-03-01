using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartExplosion : MonoBehaviour {

    public static void Explode()
    {
        var fx = GameObject.Find("FX");
        if (fx == null) {
            return;
        }
        var particle = fx.GetComponent<ParticleSystem>();
        particle.Play();
        Destroy(fx, particle.main.duration);
    }
}
