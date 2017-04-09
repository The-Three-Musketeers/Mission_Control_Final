using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//This scipt interprets a button press as a mouse click for the purposes
//of simulating hardware functionality.

public class Manual_Click : MonoBehaviour {

    public static string buttonName = "Button";
    public static bool clicked = false;

    void Update() {
        //If the button is to be clicked, invoke a click event and reset
        if (clicked) {
            GameObject.Find(buttonName).GetComponent<Button>().onClick.Invoke();
            reset();
        }
    }

    //Trigger the boolean flag specifying that a click has occurred
    public static void click() {
        clicked = true;
    }

    //Set the buttonName to te specified string
    //Used for if there is more than one option
    public static void set_name(string new_name) {
        buttonName = new_name;
    }

    //Reset the ButtonName and clicked boolean to their default values
    //Also reset the Selector script's state
    public static void reset() {
        buttonName = "Button";
        clicked = false;
        Selector.reset();
    }
}
