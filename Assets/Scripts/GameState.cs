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

    //Handling for inactivity timer
    int timer;
    public static AudioSource audio2;

    public string get_mission() {
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

    private void Start() {
        if (audio2 == null) { audio2 = GameObject.Find("Audio2").GetComponent<AudioSource>(); DontDestroyOnLoad(audio2); }
    }

    //The Update function is for the inactivity timer only
    private void Update() {
        //If the user presses a key, reset the timer
        if (Input.anyKeyDown) {
            timer = 0;
        }
        timer += 1;
        //If the timer reaches 9000 (five minutes at 30fps) with no input, reset everything
        //and go to the Title Screen
        if (timer >= 9000) {
            LaunchPad.reset();
            Manual_Click.reset();
            //Rare case, if reset on gameplay screen, make sure to change the music
            if (SceneManager.GetActiveScene().name == "Gameplay") {
                ScreenChanges.changeMusic();
            }
            //Rare case, if rocket is launching and it resets, turn off the launch sounds
            if (audio2.isPlaying == true) {
                ScreenChanges.launch_sounds();
            }
            Selector.reset();
            Skybox.reset();
            ScreenChanges.staticSpecificScene("TitleScreen");
            Camera.reset();
        }
    }

}
