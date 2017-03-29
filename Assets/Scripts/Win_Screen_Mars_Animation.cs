﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Win_Screen_Mars_Animation : MonoBehaviour{

    public static AudioSource audio;
    Material sky;

    void Start() {
        sky = RenderSettings.skybox;
        sky.SetFloat("_AtmosphereThickness", 0);
        ColorChange.ChangeColor(ColorChange.getColor());
        audio = ScreenChanges.audio2;
    }

    // Update is called once per frame
    void Update() {
        Vector3 old_pos = gameObject.transform.position;
        Vector3 new_pos = new Vector3(old_pos.x + 80f, old_pos.y + 15f, old_pos.z);
        gameObject.transform.position = new_pos;
        //Decrease the launch sounds over time
        audio.volume -= 0.005f;
        //Stop it if volume decreases to 0
        if (audio.volume <= 0 && audio.isPlaying) {
            audio.Stop();
        }
    }

    public void reset(){
        audio.volume = 1;
        if (audio.isPlaying) {
            ScreenChanges.launch_sounds();
        }
    }

}