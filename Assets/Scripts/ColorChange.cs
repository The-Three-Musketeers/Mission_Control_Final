using UnityEngine;

public class ColorChange : MonoBehaviour {

    //Global (for persistence)
    public static int color = 0;

    // Use this for initialization
    int counter = 0;
    static Color[] colorList = { Color.red, Color.green, Color.blue};
    //ArduinoNet.Serial serial;

	void Start () {
        ChangeColor(color);
        //serial = ArduinoNet.Serial.Connect("COM4");
        //if(serial != null) {
        //    serial.OnButtonPressed += Serial_OnButtonPressed;
        //}
	}

    private void Serial_OnButtonPressed(object sender, ArduinoNet.ArduinoEventArg arg) {
        var buttonID = arg.Value;
        ChangeColor(buttonID);
    }

    public static void ChangeColor(int colorIndex) {
        var rocket = GameObject.Find("rocket");
        if(rocket == null) {
            print("Rocket not found!");
            return;
        }
        colorIndex = colorIndex % colorList.Length;
        color = colorIndex;
        foreach (var obj in rocket.GetComponentsInChildren(typeof(Renderer))) {
            var render = obj.GetComponent<Renderer>();
            foreach (var material in render.materials)
            {
                if (material.name.Contains("FrontCol") || material.name.Contains("_Gray_") || material.name.Contains("Material"))
                    material.SetColor("_Color", colorList[colorIndex]);

            }
        }
    }

    public static int getColor() {
        return color;
    }
}
