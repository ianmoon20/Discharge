using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lights : MonoBehaviour {

    public List<Light> lights;

    private bool isEnabled;

    public bool IsEnabled
    {
        get { return isEnabled; }
        set { isEnabled = value; }
    }


    // Use this for initialization
    void Start () {
        isEnabled = true;
	}
	
	// Update is called once per frame
	void Update () {

        //Checking if the box is enabled
		if(isEnabled == false)
        {
            //If it is not..
            foreach(Light light in lights)
            {
                //disable all lights assosciated with it
                light.enabled = false;
            }
        }
	}
}
