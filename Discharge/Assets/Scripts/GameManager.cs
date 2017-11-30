using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    //List of scenes the gameManager has to go through
    public List<string> scenes;

    //Scene we are currently on
    private int sceneIndex;

    public int SceneIndex
    {
        get { return sceneIndex; }
        set
        {
            int index = value;

            //Making sure the same index isn't being passed in
            if(index != SceneIndex)
            {
                //Making sure Scene Index can't be set outside bounds
                if (index >= 0 && index <= scenes.Count)
                {
                    sceneIndex = index;
                }
            }
        }
    }

    //Enum of our game states
    public enum States { Playing, Won, Lost };
    public enum AudioStates { Hidden, Discovered, Chased };

    private int[] enemyStateCount;

    //Property with Getter and Setter
    private States currState;

    public States CurrState
    {
        get { return currState; }
        set { currState = value; }
    }

    private AudioTrack audioTrack;

    //Reference to the player
    GameObject player;
    Player playerScript;

    //List of all the enemies on the scene
    List<GameObject> enemyList;
    List<Enemy> enemyScripts;

    //Array of all the lights on the scene
    private GameObject[] lights;
    private Light[] lightScripts;

    //Time to complete the level
    float maxTimer;

    //Time left to complete the level
    float levelTimer;

    private void Start()
    {
        currState = States.Playing;

        //Setting scene to the first
        sceneIndex = 0;

        //No level timer for the first scene (Main Menu probably)
        levelTimer = -1;

        lights = GameObject.FindGameObjectsWithTag("detectLight");
        for(int i = 0; i < lights.Length; i++)
        {
            lightScripts[i] = lights[i].GetComponent<Light>();
        }

        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<Player>();

        audioTrack = GetComponent<AudioTrack>();

        enemyStateCount = new int[audioTrack.TrackSize];
        for (int i = 0; i < enemyStateCount.Length; i++)
        {
            enemyStateCount[i] = 0;
        }

    }

    // Update is called once per frame
    void Update () {
        //If current State is 'Lost'
        if (currState == States.Lost)
        {
            //Restart the level
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        //Checking to see if there is a light
        if (lights.Length >= 1)
        {
            //Going through each light
            foreach(Light light in lightScripts)
            {
                bool inLight = false;

                //Making sure the light is on before testing
                if (light.enabled == true && light.type != LightType.Directional)
                {
                    //Checking a certain distance (can change - just a placeholder)
                    if (Vector3.Distance(light.transform.position, player.transform.position) <= 20)
                    {
                        //Debug.Log("Distance passed");
                        RaycastHit hit;

                        Debug.DrawRay(light.transform.position, light.transform.forward * 10, Color.green);

                        //Checking to see if anything is in between the light and the position
                        if (Physics.Raycast(light.transform.position, light.transform.forward, out hit))
                        {
                            Debug.Log(hit.transform.tag);
                            //If the player is hit, nothing is in the way
                            if (hit.collider.gameObject.tag == "Player")
                            {
                                //Set the player to not being lit
                                inLight = true;
                            }
                        }
                    }
                }

                playerScript.IsLit = inLight;
            }
        }

        //If current State is 'Won'
        else if (currState == States.Won)
        {
            //Increment the level then load
            sceneIndex++;

            //If we got passed our amount of scenes, send us back to the main menu
            if (sceneIndex > scenes.Count)
            {
                sceneIndex = 0;
            }

            //Load the scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        //If there is a timer...
        if(levelTimer >= 0)
        {
            //Decrement it based on time passed
            levelTimer -= Time.deltaTime;

            //If the timer runs out...
            if (levelTimer <= 0)
            {
                //Set the timer to the max timer
                levelTimer = maxTimer;

                //Flag the player as having lost
                currState = States.Lost;
            }
        }

        //Resetting the currState if our state has changed
        if (currState == States.Won || currState == States.Lost)
        {
            currState = States.Playing;
        }
	}

    //Keeping track of how many enemies are in each state
    public void UpdateTrack(int index, int modifier) {
        enemyStateCount[index] += modifier;
        if(enemyStateCount[2] >= 1)
        {
            audioTrack.CurrState = AudioTrack.State.t3;
        } else if(enemyStateCount[1] >= 1) {
            audioTrack.CurrState = AudioTrack.State.t2;
        } else {
            audioTrack.CurrState = AudioTrack.State.t1;
        }
    }
}
