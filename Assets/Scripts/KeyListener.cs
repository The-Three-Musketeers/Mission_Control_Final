using UnityEngine;
using System;
using System.Collections;
using ArduinoNet;

//This script listens for keys and sends out signals to control
//the game state appropriately.

public class KeyListener : MonoBehaviour {

    //System state variables for UI
    public static bool fuel_selected = false;
    public static bool angle_selected = false;

    //Serial initialization
    public static Serial serial = Serial.Connect("COM3");

    // Update is called once per frame
    //The controls are as follows:
    //Q - Quit the Application
    //Left Arrow - Go to the previous screen in the sequence
    //A - Left option button
    //D - Right option button
    //S - Select option button
    //F - Hold to select the fuel on the gameplay screen
    //G - Hold to select the angle on the gameplay screen
    //Up Arrow - See 'Manual_Slider.cs' for details
    //Down Arrow - Ditto

    void Start() {
        if (serial != null)
        {
            serial.OnButtonPressed += Serial_OnButtonPressed;
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            Selector.prev_option();
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            Selector.next_option();
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            Manual_Click.click();
        }
        if (Input.GetKey(KeyCode.F)) {
            GameState.select_fuel();
        }
        else {
            GameState.unselect_fuel();
        }
        if (Input.GetKey(KeyCode.G)) {
            GameState.select_angle();
        }
        else {
            GameState.unselect_angle();
        }
    }

    private void Serial_OnButtonPressed(object sender, ArduinoEventArg arg)
    {
        if (arg.Value == 0)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => Manual_Click.click());
        }
        if (arg.Value == 1)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => Selector.prev_option());
        }
        if (arg.Value == 2)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => Selector.next_option());
        }
        if (arg.Value == 3)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => Application.Quit());
        }
    }
}

