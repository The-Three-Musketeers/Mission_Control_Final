using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//This scipt interprets a button press as a mouse click for the purposes
//of simulating hardware functionality.

public class Manual_Click : MonoBehaviour {

    public static string buttonName = "Button";
    public static bool clicked = false;

    void Update()
    {
        if (clicked)
        {
            GameObject.Find(buttonName).GetComponent<Button>().onClick.Invoke();
            reset();
        }
    }
    public static void click()
    {
        clicked = true;
    }

    public static void set_name(string new_name) {
        buttonName = new_name;
    }

    public static void reset()
    {
        buttonName = "Button";
        clicked = false;
        Selector.reset();
    }
}
