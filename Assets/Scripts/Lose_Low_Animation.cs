using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lose_Low_Animation : MonoBehaviour {

    public static AudioSource audio;

    void Start() {
        ColorChange.ChangeColor(ColorChange.getColor());
        audio = ScreenChanges.audio2;
    }

    // Update is called once per frame
    void Update () {
        Vector3 old_pos = gameObject.transform.position;
        Vector3 new_pos = new Vector3(old_pos.x - 30, old_pos.y - 30, old_pos.z);
        gameObject.transform.position = new_pos;
        //Decrease the launch sounds over time
        if (new_pos.y < 0) {
            audio.volume -= 0.01f;
            //Stop it if volume decreases to 0
            if (audio.volume <= 0 && audio.isPlaying) {
                audio.Stop();
            }
        }
	}

    public void reset() {
        audio.volume = 1;
        if (audio.isPlaying) {
            ScreenChanges.launch_sounds();
        }
    }

}
