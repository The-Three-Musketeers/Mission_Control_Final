using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Win_Screen_Satellite_Animation : MonoBehaviour {

    public AudioSource audio;
    Material sky;

    void Start() {
        ScreenChanges.launch_sounds();
        sky = RenderSettings.skybox;
        sky.SetFloat("_AtmosphereThickness", 0);
        audio.Play();
    }

    // Update is called once per frame
    void Update() {
        Vector3 old_pos = gameObject.transform.position;
        Vector3 old_rot = gameObject.transform.rotation.eulerAngles;
        //Stop moving to the left at a given point
        Vector3 new_pos = old_pos;
        if (old_pos.x > 100) {
            new_pos = new Vector3(old_pos.x - 10, old_pos.y, old_pos.z);
        }
        Vector3 new_rot = new Vector3(old_rot.x, old_rot.y + 0.1f, old_rot.z + 1f);
        gameObject.transform.position = new_pos;
        gameObject.transform.rotation = Quaternion.Euler(new_rot);
    }
    public void reset() {
        audio.Stop();
    }

}
