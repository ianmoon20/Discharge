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
    public States currState;

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
    public GameObject[] lights;
    public Light[] lightScripts;

    //Time to complete the level
    float maxTimer;

    //Time left to complete the level
    float levelTimer;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        currState = States.Playing;

        //Setting scene to the first
        sceneIndex = 0;

        //No level timer for the first scene (Main Menu probably)
        levelTimer = -1;

        lights = GameObject.FindGameObjectsWithTag("detectLight");
        lightScripts = new Light[lights.Length];

        for (int i = 0; i < lights.Length; i++)
        {
            lightScripts[i] = lights[i].GetComponent<Light>();
        }

        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<Player>();

        audioTrack = GetComponent<AudioTrack>();

        if(audioTrack != null)
        {
            enemyStateCount = new int[audioTrack.TrackSize];
            for (int i = 0; i < enemyStateCount.Length; i++)
            {
                enemyStateCount[i] = 0;
            }
        }
    }

    // Update is called once per frame
    void Update () {
        bool inLight = false;

        //If current State is 'Lost'
        if (currState == States.Lost)
        {
            //Restart the level
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        //If current State is 'Won'
        else if (currState == States.Won || Input.GetKeyDown(KeyCode.Delete))
        {
            //Increment the level then load
            sceneIndex++;

            //If we got passed our amount of scenes, send us back to the main menu
            if (sceneIndex > scenes.Count)
            {
                sceneIndex = 0;
            }

            Debug.Log((SceneManager.GetActiveScene().buildIndex + 1));
            Debug.Log(SceneManager.sceneCountInBuildSettings);
            SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
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

        //Checking to see if there is a light
        if (lights.Length >= 1)
        {
            //Going through each light
            foreach (Light light in lightScripts)
            {
                //Making sure the light is on before testing
                if (light.enabled == true)
                {
                    //Checking a certain distance (can change - just a placeholder)
                    if (Vector3.Distance(light.transform.position, player.transform.position) <= light.range)
                    {
                        //Debug.Log("Distance passed");
                        RaycastHit hit;

                        Vector3 targetDir = (player.transform.position + new Vector3(0, .8f, 0)) - light.transform.position;
                        Debug.DrawRay(light.transform.position, targetDir * light.range, Color.green);

                        //Checking to see if anything is in between the light and the position
                        if (Physics.Raycast(light.transform.position, targetDir, out hit))
                        {
                            if (light.type == LightType.Spot)
                            {
                                if (Vector3.Angle(targetDir, light.transform.forward) < light.spotAngle / 2.0f)
                                {
                                    if (hit.collider.gameObject.tag == "Player")
                                    {
                                        Debug.Log("Hi Spot");
                                        //Set the player to not being lit
                                        inLight = true;
                                    }
                                }
                            }
                            //If the player is hit and we're a point light, nothing is in the way
                            else if (hit.collider.gameObject.tag == "Player")
                            {
                                Debug.Log("Hi Point");
                                //Set the player to not being lit
                                inLight = true;
                            }
                        }
                    }
                }

                //playerScript.IsLit = inLight;
            }
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
