using UnityEngine;
using System.Collections;

//This script dynamically alters the tint on the rocket model used on the gameplay screen based on the player's
//choice of color on the previous screen.

public class Rocket_Color_Change : MonoBehaviour {
	
	// Update is called once per frame
	void Start() {		
		int len = gameObject.GetComponent<MeshRenderer> ().materials.Length;
		//Based on the value held in GameState's rocket-color field, cycle through all the materials in the Mesh
		//Renderer Component and shift the colors in the appropriate direction, effectively "tinting" the object.
		//If it is supposed to be red, tint in the direction of .r
		if (GameState.rocket_color == "Red") {
			for (int i = 0; i < len; i++) {
				Color curr = gameObject.GetComponent<MeshRenderer> ().materials [i].color;
				curr.r += (float)100 / (float)256;
				curr.g -= (float)100 / (float)256;
				curr.b -= (float)100 / (float)256;
				gameObject.GetComponent<MeshRenderer> ().materials [i].SetColor("_Color", curr);
			}
		}
		//If it is supposed to be green, tint in the direction of .g
		if (GameState.rocket_color == "Green") {
			for (int i = 0; i < len; i++) {
				Color curr = gameObject.GetComponent<MeshRenderer> ().materials [i].color;
				curr.r -= (float)100 / (float)256;
				curr.g += (float)100 / (float)256;
				curr.b -= (float)100 / (float)256;
				gameObject.GetComponent<MeshRenderer> ().materials [i].SetColor("_Color", curr);
			}
		}
		//If it is supposed to be blue, tint in the direction of .b
		if (GameState.rocket_color == "Blue") {
			for (int i = 0; i < len; i++) {
				Color curr = gameObject.GetComponent<MeshRenderer> ().materials [i].color;
				curr.r -= (float)100 / (float)256;
				curr.g -= (float)100 / (float)256;
				curr.b += (float)100 / (float)256;
				gameObject.GetComponent<MeshRenderer> ().materials [i].SetColor("_Color", curr);
			}
		}	
	}
}
