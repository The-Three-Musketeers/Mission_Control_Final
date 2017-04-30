using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

//This class keeps track of all game variables, persisting across
//multiple screens.

public class GameState : MonoBehaviour {

	public static string mission;
	public static string rocket_color;
	public static bool fuel_selected = false;
	public static bool angle_selected = false;

    public static AudioSource audio2;

    //Bunch of getters and setters

    public static string get_mission() {
		return mission;
	}

	public void set_mission(string new_mission) {
		mission = new_mission;
	}

	public string get_rocket_color() {
		return rocket_color;
	}

	public void set_rocket_color(string new_rocket_color) {
		rocket_color = new_rocket_color;
	}

	public static void select_fuel() {
		fuel_selected = true;
	}

	public static void unselect_fuel() {
		fuel_selected = false;
	}

	public static void select_angle() {
		angle_selected = true;
	}

	public static void unselect_angle() {
		angle_selected = false;
	}

    //Start out by checking to see if audio2 (Launch sound) is null,
    //and setting it appropriately for use later
    private void Start() {
        if (audio2 == null && GameObject.Find("Audio2") != null){
            audio2 = GameObject.Find("Audio2").GetComponent<AudioSource>();
            DontDestroyOnLoad(audio2);
        }
    }
}
