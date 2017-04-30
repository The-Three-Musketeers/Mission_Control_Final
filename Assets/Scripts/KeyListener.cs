using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using ArduinoNet;
using WindowsInput;
using System.Windows.Forms;

//This script listens for keys and sends out signals to control
//the game state appropriately.

public class KeyListener : MonoBehaviour {

    //System state variables for UI
    public static bool fuel_selected = false;
    public static bool angle_selected = false;

    //Serial initialization
    public static Serial serial = Serial.Connect("COM3");

    // Update is called once per frame
    //The controls are as follows for the keyboard:
    //Q - Quit the Application
    //A - Left option button
    //D - Right option button
    //S - Select option button
    //F - Hold to select the fuel on the gameplay screen
    //G - Hold to select the angle on the gameplay screen
    //Up Arrow - See 'Manual_Slider.cs' for details
    //Down Arrow - Ditto
    //ENTER - Launch the Rocket on the Gameplay Screen
    //J - Change color of rocket to Red
    //K - Change color of rocket to Green
    //L - Change color of rocket to Blue

    //This doesn't listen for the rocket launch,
    //drop fuel, or launch pad animation keys. Those are handled
    //in their respective scripts. This one is specifically for 
    //dealing with the UI.

    //Additionally, this is only for keyboard inputs, which are
    //intended only for developer use.

    int delay_timer;

    //Start out by getting the inputs from the analog controller
    //But only if the controller is connected (i.e. serial != null)
    void Start() {
        if (serial != null) {
            serial.OnButtonPressed += Serial_OnButtonPressed;
            serial.OnSlideChanged += Serial_OnSlideChanged;
            serial.OnKnobChanged += Serial_OnKnobChanged;
        } else {
          System.Diagnostics.Process.GetCurrentProcess().Kill();
          MessageBox.Show("No Controller Connected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
        }
    }

    //Called once per frame, polls for inputs of all kinds
    void Update() {
        delay_timer += 1;
        //Quit with Q
        if (Input.GetKeyDown(KeyCode.Q)) {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
        //Option Selection Keys
        if (Input.GetKeyDown(KeyCode.A)) {
            Selector.prev_option();
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            Selector.next_option();
        }
        //Manual Click Key - DON'T DO THIS ON THE GAMEPLAY SCREEN - LEADS TO WEIRD BEHAVIOR
        if (Input.GetKeyDown(KeyCode.S) && SceneManager.GetActiveScene().name != "Gameplay") {
            Manual_Click.click();
        }
        //Color Changing Keys
        if (Input.GetKeyDown(KeyCode.J)) {
            ColorChange.ChangeColor(0);
        }
        if (Input.GetKeyDown(KeyCode.K)) {
            ColorChange.ChangeColor(1);
        }
        if (Input.GetKeyDown(KeyCode.L)) {
            ColorChange.ChangeColor(2);
        }
        //Fuel and Angle Controllers
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

    //This function handles the mapping of different Arduino inputs,
    //noted as arg.Value, into actual function calls for different parts
    //of the game.
    private void Serial_OnButtonPressed(object sender, ArduinoEventArg arg) {
        if (delay_timer >= 2) {
            //Left Arrow
            if (arg.Value == 1) {
                UnityMainThreadDispatcher.Instance().Enqueue(() => Selector.prev_option());
                delay_timer = 0;
            }
            //Launch-OK Button
            if (arg.Value == 0) {
                UnityMainThreadDispatcher.Instance().Enqueue(() => {
                    //If on the gameplay screen, this button acts as the ENTER key
                    if (SceneManager.GetActiveScene().name == "Gameplay")
                        InputSimulator.SimulateKeyPress(VirtualKeyCode.RETURN);
                    //Otherwise it acts as a mouse click
                    else
                        Manual_Click.click();
                });
                delay_timer = 0;
            }
            //Right Arrow
            if (arg.Value == 2) {
                UnityMainThreadDispatcher.Instance().Enqueue(() => Selector.next_option());
                delay_timer = 0;
            }
            //Red Rutton
            if (arg.Value == 3) {
                UnityMainThreadDispatcher.Instance().Enqueue(() => InputSimulator.SimulateKeyPress(VirtualKeyCode.VK_J));
                delay_timer = 0;
            }
            //Green Button
            if (arg.Value == 4) {
                UnityMainThreadDispatcher.Instance().Enqueue(() => InputSimulator.SimulateKeyPress(VirtualKeyCode.VK_K));
                delay_timer = 0;
            }
            //Blue Button
            if (arg.Value == 5) {
                UnityMainThreadDispatcher.Instance().Enqueue(() => InputSimulator.SimulateKeyPress(VirtualKeyCode.VK_L));
                delay_timer = 0;
            }
        }
    }

    //Handling for if the slider is changed
    private void Serial_OnSlideChanged(object sender, ArduinoEventArg arg) {
        //If the rocket is not currently being launched, get the clamped value of the slider
        if (RocketBehavior.launch == false) {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var obj = GameObject.Find("Fuel Slider");
                if (obj != null)
                    obj.GetComponent<Slider>().value = arg.Value / 255f;
                delay_timer = 0;
            });
        }
    }

    //Handling for if the angle knob is changed
    private void Serial_OnKnobChanged(object sender, ArduinoEventArg arg) {
        //If the rocket is not currently being launched, get the clamped value of the angle
        if (RocketBehavior.launch == false) {
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                var obj = GameObject.Find("Angle Slider");
                if (obj != null)
                    obj.GetComponent<Slider>().value = arg.Value / 255f;
                delay_timer = 0;
            });
        }
    }


}

