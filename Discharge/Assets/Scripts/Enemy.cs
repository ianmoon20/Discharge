using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour {
    //enum for enemy behavior
    public enum EnemyState
    {
        Patrolling,
        Investigating,
        Chasing,
        Sweeping,
        Disabled
    };

    //variables for the enemy class

    //reference of player
    private GameObject target;

    //reference to player capsule collider
    private CapsuleCollider playerCapColl; 

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
    [SerializeField] float discoverDelay = 5;
    private float discoverProgress;

    //The enemys current destination
    private Vector3 targetDestination;

    //floats for shooting at player
    private float shotCooldown;
    private float cooldownProgress;
    //float for stormtrooper aiming
    private float numShotsFired;

    //floats for how far enemy sweeps for player
    private Quaternion sweepAngle;
    private Quaternion sweepProgress;

    //floats to reactivate a disaled enemy
    private float reactivateTime;
    private float reactivateProgress;

    //floats for giving up investigation
    private float investigateTime;
    private float investigateProgress;

    //getters and setters
    public float MoveSpeed { get { return moveSpeed; } }
    public Vector3 StartLocation { get { return startLocation; } }
    public Vector3 TargetDestination {get{return targetDestination;} }
    public Vector3[] Path { get { return path; } }
    public EnemyState CurrentState { get { return currentState; } }

    private GameManager gameManager;


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
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        target = GameObject.FindGameObjectsWithTag("Player")[0];
        discoverDelay = 5f;
        playerCapColl = target.GetComponent<CapsuleCollider>();
        sweepProgress = gameObject.transform.rotation;
        sweepAngle = Quaternion.AngleAxis(360, Vector3.up); ;
        reactivateProgress = 0;
        reactivateTime = 2;
        investigateTime = 5;
        investigateProgress = 0;

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

        detection();

        
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

        setDestination();
    }

    /// <summary>
    /// Behavior for investigating state
    /// </summary>
    private void investigating()
    {
        
        
        if ((targetDestination - gameObject.transform.position).magnitude < 3)
        {

            Vector3 targetDir = targetDestination;
            targetDir.y = gameObject.transform.position.y;
            gameObject.transform.LookAt(targetDir);
            targetDestination = gameObject.transform.position;
        }
        if (gameObject.transform.position.x == targetDestination.x && gameObject.transform.position.z == targetDestination.z)
        {
            investigateProgress += Time.deltaTime;
            if(investigateProgress > investigateTime)
            {
                investigateProgress = 0;
                currentState = EnemyState.Patrolling;
            }
        }
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
        targetDestination = gameObject.transform.position;
        reactivateProgress += Time.deltaTime;
        
        if(reactivateProgress >= reactivateTime)
        {
            currentState = EnemyState.Patrolling;
        }

    }

    /// <summary>
    /// Method for recognizing where the player is.
    /// </summary>
    private void detection()
    {
        
        Vector3 heightAdjustment = new Vector3(0, playerCapColl.center.y + playerCapColl.height / 2,0);
        //gets dot product to determine if facing player
        Vector3 targetDir = target.transform.position + heightAdjustment - transform.position;
        float dot = Vector3.Dot(transform.forward, targetDir.normalized);

        float angle = Mathf.Rad2Deg * Mathf.Acos(dot);

        if (angle < 30)
        {


            //raycast to see if there are obstacles
            RaycastHit hit;

            if (Physics.Raycast(transform.position, targetDir.normalized, out hit, 10.0f))

                if (hit.transform.gameObject.tag == "Player")
                {
                    discoverProgress += Time.deltaTime;
                    
                    if (discoverProgress >= discoverDelay)
                    {
                        gameManager.CurrState = GameManager.States.Lost;
                    }
                }
                else
                {
                    discoverProgress = 0;
                }

        }


    }


    /// <summary>
    /// Investigates a location
    /// </summary>
    /// <param name="noiseLocation"></param>
    public void investigateLocation(Vector3 noiseLocation)
    {
        currentState = EnemyState.Investigating;

        //Adding 1 to the Investigating Count in Game Manager
        gameManager.UpdateTrack(1, 1);
        targetDestination = noiseLocation;
    }

    /// <summary>
    /// sets enemy state to disabled
    /// </summary>
    public void disableEnemy()
    {
        currentState = EnemyState.Disabled;
    }
}
