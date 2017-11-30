using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lights : MonoBehaviour {

    public List<GameObject> objects;

    private List<Light> lightScripts;
    private List<Door> doorScripts;

    private bool isEnabled;

    public bool IsEnabled
    {
        get { return isEnabled; }
        set { isEnabled = value; }
    }


    // Use this for initialization
    void Start () {
        isEnabled = true;

        foreach (GameObject gObject in objects)
        {
            if(gObject.GetComponent<Light>() != null)
            {
                lightScripts.Add(gObject.GetComponent<Light>());
            } else if(gObject.GetComponent<Door>() != null)
            {
                doorScripts.Add(gObject.GetComponent<Door>());
            }
        }
	}
	
	// Update is called once per frame
	void Update () {

        //Checking if the box is enabled
		if(isEnabled == false)
        {
            //If it is not..
            foreach(Light light in lightScripts)
            {
                //disable all lights assosciated with it
                light.enabled = false;
            }

            //If it is not...
            foreach(Door door in doorScripts)
            {
                //Open any closed doors and close any open doors
                door.ChangeDoorState();
            }
        }
	}
}
