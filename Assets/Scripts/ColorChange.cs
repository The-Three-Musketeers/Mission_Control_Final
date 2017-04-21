using UnityEngine;

public class ColorChange : MonoBehaviour {

    //Global (for persistence)
    public static int color = 0;

    // Use this for initialization
    int counter = 0;
    static Color[] colorList = { Color.red, Color.green, Color.blue};
    //ArduinoNet.Serial serial;

    //Start out by defaulting the color to red (o)
	void Start () {
        ChangeColor(color);
	}

    //CChange the color to the index specified (Red - 0, Green - 1, Blue - 2)
    public static void ChangeColor(int colorIndex) {
        var rocket = GameObject.Find("rocket");
        //Check to make sure there is actually a rocket; don't do anything if there is not
        if(rocket == null) {
            return;
        }
        colorIndex = colorIndex % colorList.Length;
        color = colorIndex;
        //Change every component of the Rocket to be a specifically colored variant of the default
        foreach (var obj in rocket.GetComponentsInChildren(typeof(Renderer))) {
            var render = obj.GetComponent<Renderer>();
            foreach (var material in render.materials) {
                //The the material's name contains any of the following keywords, color that material
                if (material.name.Contains("FrontCol") || material.name.Contains("_Gray_") || material.name.Contains("Material"))
                    material.SetColor("_Color", colorList[colorIndex]);
            }
        }
    }

    //Returns the currently selected color
    public static int getColor() {
        return color;
    }
}
