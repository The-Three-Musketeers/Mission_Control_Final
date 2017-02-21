using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//This script types up the string held in the text field of a text object letter by letter.
//It ultimately has the effect of animating the text like a typewriter.
//This code was adapted from code found at answers.unity3d.com/questions/50104

public class Typer : MonoBehaviour {

	float delay_time;
	Text text_object;
	string text_transcript;

	// Use this for initialization
	void Start () {
		delay_time = 0.05f;
		//Grab the Text type object and it's default text, then clear the text
		text_object = GameObject.Find ("Text").GetComponent<Text> ();
		text_transcript = text_object.text;
		text_object.text = "";
		//Trigger the typing animation
		StartCoroutine(type ());
	}
	
	IEnumerator type() {
		//For every character in the specified text, add it to the Text object and wait
		foreach (char letter in text_transcript.ToCharArray()) {
			text_object.text += letter;
			yield return new WaitForSeconds (delay_time);
		}
	}
}
