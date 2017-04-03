using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script was heavily adapted from official Unity tutorial code found at
//https://unity3d.com/learn/tutorials/topics/graphics/realtime-global-illumination-daynight-cycle
//This script handles the maintenance of the day/night cycle, as well as controlling the background
//details on the gameplay screen. Attach it to the sun.

public class Skybox : MonoBehaviour {

    //Internal references
    Material sky;
    float skySpeed = 1;
    Light mainLight;
    //Public references
    public Transform cam;
    public static Renderer water;
    public Transform worldProbe;
    public Gradient nightDayColor;
    public float maxIntensity = 3f;
    public float minIntensity = 0f;
    public float minPoint = -0.2f;
    public float maxAmbient = 1f;
    public float minAmbient = 0f;
    public float minAmbientPoint = -0.2f;
    public float dayAtmosphereThickness = 1.5f;
    public float nightAtmosphereThickness = 1.5f;
    public static float globalAtmosphereThickness = 1.5f;
    public Vector3 dayRotateSpeed = new Vector3(-0.123f, 0, 0); //Make this static so we can speed up the cycle as the rocket nears space
    public Vector3 nightRotateSpeed = new Vector3(-0.123f, 0, 0);

    private static bool launching = false;

	// Use this for initialization
	void Start () {
        sky = RenderSettings.skybox;
        mainLight = GetComponent<Light>();
        water = GameObject.Find("Ocean").GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
        //Updating the water and global reflections
        Vector3 tvec = cam.position;
        worldProbe.transform.position = tvec;
        water.material.mainTextureOffset = new Vector2(Time.time / 100, 0);
        water.material.SetTextureOffset("_DetailAlbedoMap", new Vector2(0, Time.time / 80));
        //Updating the sky
        float tRange = 1 - minPoint;
        float dot = Mathf.Clamp01((Vector3.Dot(mainLight.transform.forward, Vector3.down) - minPoint) / tRange);
        float i = ((maxIntensity - minIntensity) * dot) + minIntensity;
        mainLight.intensity = i;
        tRange = 1 - minAmbientPoint;
        dot = Mathf.Clamp01((Vector3.Dot(mainLight.transform.forward, Vector3.down) - minAmbientPoint) / tRange);
        i = ((maxAmbient - minAmbient) * dot) * minAmbient;
        RenderSettings.ambientIntensity = i;
        mainLight.color = nightDayColor.Evaluate(dot);
        RenderSettings.ambientLight = mainLight.color;
        if (launching) {
            globalAtmosphereThickness -= 0.01f;
            if (globalAtmosphereThickness <= 0) {
                globalAtmosphereThickness = 0;
                water.transform.localScale = new Vector3(0, 0, 0);
            }
        }
        sky.SetFloat("_AtmosphereThickness", globalAtmosphereThickness);
        if (dot > 0) {
            transform.Rotate(dayRotateSpeed * Time.deltaTime * skySpeed);
        }
        else {
            transform.Rotate(nightRotateSpeed * Time.deltaTime * skySpeed);
        }
    }

    public static void leavingAtmosphere() {
        launching = true;
    }

    public static void reset() {
        launching = false;
        globalAtmosphereThickness = 1.5f;
        water.transform.localScale = new Vector3(7000, 1, 7000);
    }
}
