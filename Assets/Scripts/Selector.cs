using UnityEngine;
using System.Collections;

//This script provides functionality for manually selecting from three
//options. It features wrap-around handling.

public class Selector : MonoBehaviour {

	public static int option_selected = 1;

	public static int get_option() {
		return option_selected;
	}

	public static void next_option() {
		option_selected = option_selected + 1;
		if (option_selected == 4) {
			option_selected = 1;
		}
	}

	public static void prev_option() {
		option_selected = option_selected - 1;
		if (option_selected == 0) {
			option_selected = 3;
		}
	}

    public static void reset()
    {
        option_selected = 1;
    }
}
