using UnityEngine;
using System.Collections;

//This script handles how a button should behave when it is selected, specifically that it should enlarge and wait
//to be clicked, and how it should behave when it is not selected, specifially that it should do nothing.

public class Selected : MonoBehaviour {

	public int selection;

	public float default_width;
	public float default_height;

	int flag = 0;
	
	// Update is called once per frame
	void Update () {
		//If the option is not selected, it should be the default width and height
		if (Selector.get_option () != selection) {
			gameObject.GetComponent<RectTransform> ().sizeDelta = new Vector2 (default_width, default_height);
			flag = 0;
			//If the option is selected, it should increase in size by 100 each, but only once
		} else if (Selector.get_option () == selection && flag == 0) {
			gameObject.GetComponent<RectTransform> ().sizeDelta = new Vector2 (default_width + 100, default_height + 100);
			Manual_Click.set_name("Button" + selection.ToString());
			flag = 1;
		}
	}
}
