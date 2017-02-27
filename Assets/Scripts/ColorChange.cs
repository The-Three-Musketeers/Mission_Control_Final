using UnityEngine;

public class ColorChange : MonoBehaviour {

    // Use this for initialization
    int counter = 0;
    static Color[] colorList = { Color.red, Color.green, Color.blue};
    ArduinoNet.Serial serial;

	void Start () {
        serial = ArduinoNet.Serial.Connect("COM3");
        if(serial != null)
        {
            serial.OnButtonPressed += Serial_OnButtonPressed;
        }
	}

    private void Serial_OnButtonPressed(object sender, ArduinoNet.ArduinoEventArg arg)
    {
        var buttonID = arg.Value;
        ChangeColor(buttonID);
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.N))
        {
            ChangeColor(counter++);
        }
	}

    public static void ChangeColor(int colorIndex)
    {
        var rocket = GameObject.Find("rocket");
        if(rocket == null)
        {
            print("Rocket not found!");
            return;
        }
        colorIndex = colorIndex % colorList.Length;
        foreach (var obj in rocket.GetComponentsInChildren(typeof(Renderer)))
        {
            var render = obj.GetComponent<Renderer>();
            foreach (var material in render.materials)
            {
                if (material.name.Contains("FrontCol") || material.name.Contains("_Gray_") || material.name.Contains("Material"))
                    material.SetColor("_Color", colorList[colorIndex]);

            }
        }
    }
}
