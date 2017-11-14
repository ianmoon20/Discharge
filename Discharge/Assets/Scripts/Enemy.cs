using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    //enum for enemy behavior
    private enum EnemyState
    {
        Patrolling,
        Investigating,
        Chasing,
        Sweeping,
        Disabled
    };

    //variables for the enemy class

    //the current state the enemy is in
    private EnemyState currentState;

    //the enemys initial state
    private EnemyState initialState;

    //how fast the enemy can move
    private float moveSpeed;

    //Where the enemy starts out
    private Vector3 startLocation;

    //where the enemy will paatrol
    private Vector3[] path;
    private int pathIndex;

    //floats for spotting the player
    private float discoverDelay;
    private float discoverProgress;

    //The enemys current destination
    private Vector3 targetDestination;

    //floats for shooting at player
    private float shotCooldown;
    private float cooldownProgress;
    //float for stormtrooper aiming
    private float numShotsFired;

    //floats for how far enemy sweeps for player
    private float sweepAngle;
    private float sweepProgress;

    //floats to reactivate a disaled enemy
    private float reactivateTime;
    private float reactivateProgress;

    //getters and setters
    public float MoveSpeed { get { return moveSpeed; } }
    public Vector3 StartLocation { get { return startLocation; } }
    public Vector3 TargetDestination {get{return targetDestination;} }
    public Vector3[] Path { get { return path; } }


    //public variables for initialization
    public Vector3[] setPath;
    public int initState;

	// Use this for initialization
	void Start () {
        currentState = (EnemyState)initState;
        initialState = (EnemyState)initState;
        path = new Vector3[setPath.Length];
        path = setPath;
        pathIndex = 0;
        targetDestination = path[pathIndex];
        startLocation = gameObject.transform.position;

    }
	
	// Update is called once per frame
	void Update () {

        //Switch statement to call correct behavior
        switch (currentState)
        {

            //behavior is placed in methods for etter organization
            case EnemyState.Patrolling:
                patrolling();
                break;
            case EnemyState.Investigating:
                investigating();
                break;
            case EnemyState.Chasing:
                chasing();
                break;
            case EnemyState.Sweeping:
                sweeping();
                break;
            case EnemyState.Disabled:
                disabled();
                break;


        }



        setDestination();
	}

    /// <summary>
    /// Sets the enemy navigation target
    /// </summary>
    private void setDestination()
    {
        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.destination = targetDestination;
    }


    /// <summary>
    /// Behavior for patrolling state
    /// </summary>
    private void patrolling()
    {
        //check if x and z are correct
        if(gameObject.transform.position.x == targetDestination.x && gameObject.transform.position.z == targetDestination.z)
        {
            
            if (pathIndex < path.Length - 1)
            {
                pathIndex++;
            }
            else
            {
                pathIndex = 0;
            }
            targetDestination = path[pathIndex];
        }
    }

    /// <summary>
    /// Behavior for investigating state
    /// </summary>
    private void investigating()
    {

    }

    /// <summary>
    /// Behavior for chasing state
    /// </summary>
    private void chasing()
    {

    }

    /// <summary>
    /// Behavior for sweeping state
    /// </summary>
    private void sweeping()
    {

    }

    /// <summary>
    /// Behavior for disabled state
    /// </summary>
    private void disabled()
    {

    }

    /// <summary>
    /// Method for recognizing where the player is.
    /// </summary>
    private void detection()
    {

    }
}
