using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    //Setting up the singleton
    public static GameManager instance = null;
    
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

    //List of all the lights on the scene
    List<GameObject> lightsList;
    List<Lights> lightScripts;

    //Time to complete the level
    float maxTimer;

    //Time left to complete the level
    float levelTimer;

    //Called before ANY Start method
    void Awake() {
        //Instantiate the Game Manager if their isn't one
        if(instance == null)
        {
            instance = this;
        }

        //Destroy the Game Manager if their is one that isn't this one
        //Prevents numerous Game Managers when reloading menus
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        //Telling Unity to let us keep our nice things
        DontDestroyOnLoad(gameObject);

        //Initializing the game
        InitGame();
    }

    //Initializes the gameManager for the level
    //Should function properly for the Playtest
    void InitGame() {
        currState = States.Playing;

        //Setting scene to the first
        sceneIndex = 0;

        //No level timer for the first scene (Main Menu probably)
        levelTimer = -1;
    }

    private void Start()
    {
        audioTrack = GetComponent<AudioTrack>();

        enemyStateCount = new int[audioTrack.TrackSize];
        for (int i = 0; i < enemyStateCount.Length; i++)
        {
            enemyStateCount[i] = 0;
        }

        Debug.Log(enemyStateCount);
    }

    // Update is called once per frame
    void Update () {
        //If current State is 'Lost'
        if (currState == States.Lost)
        {
            //Restart the level
            LoadScene(sceneIndex);
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
            LoadScene(sceneIndex);
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

    void LoadScene(int level) {
        enemyStateCount = new int[audioTrack.TrackSize];
        for (int i = 0; i < enemyStateCount.Length; i++)
        {
            enemyStateCount[i] = 0;
        }

        //Loading the Scene at the passed in index
        SceneManager.LoadScene(scenes[level]);
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
