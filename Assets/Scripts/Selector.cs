using UnityEngine;
using System.Collections;

//This script provides functionality for manually selecting from three
//options. It features wrap-around handling.
//This goes on the ScriptManager object

public class Selector : MonoBehaviour {

	public static int option_selected = 1;
    public static AudioSource beep;

    //Start by finding the audio for the transition beeps
    private void Start() {
        beep = GameObject.Find("SelectorBeep").GetComponent<AudioSource>();
    }

    //Returns the option that is currently selected
	public static int get_option() {
		return option_selected;
	}

    //Selects the next option in the sequence, with wrap-around
	public static void next_option() {
        beep.Play();
		option_selected = option_selected + 1;
        //Wraparound handling
		if (option_selected == 4) {
			option_selected = 1;
		}
	}

    //Selects the previous option in the sequence, with wrap-around
	public static void prev_option() {
        beep.Play();
        option_selected = option_selected - 1;
        //Wraparound handling
		if (option_selected == 0) {
			option_selected = 3;
		}
	}

    //Resets the option_selected to the first one.
    //Call this upon leaving the Select Mission Screen.
    public static void reset() {
        option_selected = 1;
    }
}
