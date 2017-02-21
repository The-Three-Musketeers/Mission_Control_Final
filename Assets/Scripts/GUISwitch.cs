using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUISwitch : MonoBehaviour {

    //This static function switches the GUI into launch mode when the rocket is launched.
    public static void launch_mode()
    {
        GameObject fuel_ui = GameObject.Find("Fuel Slider");
        GameObject angle_ui = GameObject.Find("Angle Slider");
        fuel_ui.GetComponent<Slider>().interactable = false;
        angle_ui.GetComponent<Slider>().interactable = false;
        angle_ui.transform.localScale = new Vector3(0, 0, 0);
    }
}
