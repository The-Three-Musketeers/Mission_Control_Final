using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChange : MonoBehaviour {

    // Use this for initialization
    int counter = 0;
    Color[] colorList = { Color.red, Color.white, Color.yellow, Color.blue};

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.N))
        {
            foreach(var obj in GetComponentsInChildren(typeof(Renderer)))
            {
                var render = obj.GetComponent<Renderer>();
                foreach(var material in render.materials)
                {
                    if(material.name.Contains("FrontCol") || material.name.Contains("_Gray_") || material.name.Contains("Material"))
                        material.SetColor("_Color", colorList[counter]);
                    
                }
            }
            counter = (counter + 1) % colorList.Length;

        }
	}
}
