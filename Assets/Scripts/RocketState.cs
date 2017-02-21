using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//This class keeps track of the different variables associated with the rocket,
//namely its fuel level and angle.

public class RocketState : MonoBehaviour {

	public static float fuel = 500;
	public static float angle = 60;

	const float max_fuel = 100;
	const float max_angle = 180;

	void Update() {
		set_fuel (GameObject.Find ("Fuel Slider").GetComponent<Slider> ().value);
		set_angle (GameObject.Find ("Angle Slider").GetComponent<Slider> ().value);
	}

	public float get_fuel() {
		return fuel;
	}

	public void set_fuel(float new_fuel) {
		fuel = max_fuel * new_fuel;
	}

	public float get_angle() {
		return angle;
	}

	public void set_angle(float new_angle) {
		angle = max_angle * new_angle;
	}

}
