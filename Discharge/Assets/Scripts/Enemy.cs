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
    public EnemyState currentState;

    //the enemys initial state
    private EnemyState initialState;

    //how fast the enemy can move
    private float moveSpeed;

    //Where the enemy starts out
    private Vector3 startLocation;

    //where the enemy will patrol
    private Vector3[] path;
	[SerializeField] int pathIndex = 0;

    //floats for spotting the player
    [SerializeField] float discoverDelay = 5;
    private float discoverProgress;

    //The enemies current destination
    private Vector3 targetDestination;

    [SerializeField]GameObject laser;
    [SerializeField]float shotCooldown = 3.0f; //Cooldown for shots
    private float cooldownProgress = 0.0f; //Progress of current bullet cooldown
    private int numShotsFired = 0; //Number of shows fired for stormtrooper aiming
    private List<Vector3> oldBulletPos; //Old position of the bullet
    private List<Vector3> targetPositions; //Position of the target
    [SerializeField]float bulletSpeed = 10.0f; //Speed of the bullet
    private bool isFiring = false; //If the enemy is currently firing
    private List<GameObject> lasers; //Renders the laser line
	private AudioSource laserSound;

    private bool[] audioSwitch = {false, false};

    private Light spotLight;

    //floats for how far enemy sweeps for player
    private Quaternion sweepAngle;
    private Quaternion sweepProgress;

    //floats to reactivate a disabled enemy
    private float reactivateTime;
    private float reactivateProgress;

    //floats for giving up investigation
    private float investigateTime;
    private float investigateProgress;

    //floats for giving up a chase
    private float chaseTime;
    private float chaseProgress;

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

    //The player script for light detection
    private Player playScript;

	// Use this for initialization
	void Start () {
        currentState = (EnemyState)initState;
        initialState = (EnemyState)initState;
        path = new Vector3[setPath.Length];
        path = setPath;
        //pathIndex = 0;
        targetDestination = path[pathIndex];
        startLocation = gameObject.transform.position;
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        target = GameObject.FindGameObjectsWithTag("Player")[0];
        playScript = target.GetComponent<Player>();
        discoverDelay = 1f;
        playerCapColl = target.GetComponent<CapsuleCollider>();
        sweepProgress = gameObject.transform.rotation;
        sweepAngle = Quaternion.AngleAxis(360, Vector3.up); ;
        reactivateProgress = 0;
        reactivateTime = 2;
        investigateTime = 5;
        investigateProgress = 0;
        chaseTime = 1f;
        chaseProgress = 0;
		oldBulletPos = new List<Vector3>();
		targetPositions = new List<Vector3>();
		lasers = new List<GameObject>();
		laserSound = GetComponent<AudioSource>();
		spotLight = transform.GetChild(1).GetComponent<Light>();
    }

	// Update is called once per frame
	void Update () {
        //Switch statement to call correct behavior
        switch (currentState)
        {
            //behavior is placed in methods for etter organization
            case EnemyState.Patrolling:
				if(!spotLight.enabled)
					spotLight.enabled = true;
				if(spotLight.color != Color.white)
					spotLight.color = Color.white;
                patrolling();
                break;
            case EnemyState.Investigating:
				if(!spotLight.enabled)
					spotLight.enabled = true;
				if(spotLight.color != Color.yellow)
					spotLight.color = Color.yellow;
                investigating();
                break;
            case EnemyState.Chasing:
				if(!spotLight.enabled)
					spotLight.enabled = true;
				if(spotLight.color != Color.red)
					spotLight.color = Color.red;
                chasing();
                break;
            case EnemyState.Sweeping:
                sweeping();
                break;
            case EnemyState.Disabled:
                spotLight.enabled = false;
                disabled();
                break;


        }

        //moved detection up here so it can be called without progressing the timer
        if (detection())
        {
            discoverProgress += Time.deltaTime;
			currentState = EnemyState.Investigating;
			targetDestination = target.transform.position;

            //if timer completes, starts chasing, also if player is in light
            if (discoverProgress >= discoverDelay || playScript.IsLit)
            {
				if(audioSwitch[1] == false) {
					audioSwitch[1] = true;
					gameManager.UpdateTrack(2, 1);
				}

                currentState = EnemyState.Chasing;
            }
        }
        else
        {
            discoverProgress = 0;
            numShotsFired = 0;
        }

        if(isFiring) {
			cooldownProgress += Time.deltaTime;
			if(cooldownProgress >= shotCooldown) {
				fire();
				cooldownProgress = 0.0f;
			}
		}

		simulateProjectiles();
	}

    /// <summary>
    /// Sets the enemy navigation target
    /// </summary>
    private void setDestination()
    {
        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.destination = targetDestination;
        if(currentState == EnemyState.Chasing)
        {
            agent.speed = 4f;
        }
        else
        {
            agent.speed = 2.5f;
        }
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

            if (!detection())
            {
                investigateProgress += Time.deltaTime;
                if (investigateProgress > investigateTime)
                {
					audioSwitch[0] = false;
                    investigateProgress = 0;
					gameManager.UpdateTrack(1, -1);
                    currentState = EnemyState.Patrolling;
                }
            }
        }

        //Adding 1 to the Investigating Count in Game Manager
        setDestination();
   }

    /// <summary>
    /// Behavior for chasing state
    /// </summary>
    private void chasing()
    {

        //moves towards target
        targetDestination = target.transform.position;

        if (!detection())
        {
            chaseProgress += Time.deltaTime;
            if(chaseProgress >= chaseTime)
            {
                audioSwitch[1] = false;
                gameManager.UpdateTrack(2, -1);
                isFiring = false;
                currentState = EnemyState.Investigating;
            }

        }

        //if close to enemy, stops moving and looks at them.
        if ((targetDestination - gameObject.transform.position).magnitude < 3)
            {

                Vector3 targetDir = targetDestination;
                targetDir.y = gameObject.transform.position.y;
                gameObject.transform.LookAt(targetDir);
                targetDestination = gameObject.transform.position;
            }

		if(currentState == EnemyState.Chasing)
		{
			isFiring = true;

		}
		setDestination();

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

        //reactivates enemy if enough time passes
        targetDestination = gameObject.transform.position;
        reactivateProgress += Time.deltaTime;

        if(reactivateProgress >= reactivateTime)
        {
            currentState = EnemyState.Patrolling;
        }
        setDestination();

    }

    /// <summary>
    /// Method for recognizing where the player is.
    /// </summary>
    private bool detection()
    {
        Vector3 heightAdjustment = new Vector3(0, playerCapColl.center.y + playerCapColl.height / 2, 0);
        //gets dot product to determine if facing player
        Vector3 targetDir = (target.transform.position + heightAdjustment - transform.position).normalized;
        float dot = Vector3.Dot(transform.forward, targetDir);

        float angle = Mathf.Rad2Deg * Mathf.Acos(dot);

        if (angle < 30)
        {
            //raycast to see if there are obstacles
            RaycastHit hit;

            if (Physics.Raycast(transform.position, targetDir, out hit, 10.0f))

                if (hit.transform.gameObject.tag == "Player")
                {

                    return true;
                }

        }
        return false;

    }

    /// <summary>
    /// Investigates a location
    /// </summary>
    /// <param name="noiseLocation"></param>
    public void investigateLocation(Vector3 noiseLocation)
    {
        //only sets investigation if not actively chasing enemy
        if(currentState != EnemyState.Chasing)
        {
			if(audioSwitch[0] == false) {
				audioSwitch[0] = true;
				gameManager.UpdateTrack(1, 1);
			}

            currentState = EnemyState.Investigating;
        }



        targetDestination = noiseLocation;
    }

    /// <summary>
    /// sets enemy state to disabled
    /// </summary>
    public void disableEnemy()
    {
        currentState = EnemyState.Disabled;
    }

    public void fire()
    {
		GameObject lr = (GameObject)Instantiate(laser);
		lasers.Add(lr);
		oldBulletPos.Add(transform.position);
		numShotsFired++;

		Vector3 randomDeviation = Vector3.zero;

		if(numShotsFired <= 3) {
			randomDeviation = (transform.up * Random.Range(-0.5f, 0.5f)) + (transform.right * Random.Range(-0.5f, 0.5f));
		}

		targetPositions.Add(((target.transform.position +
								 playerCapColl.center - transform.position))
								 .normalized + randomDeviation);
		laserSound.Play();
    }

    public void simulateProjectiles()
    {
		for(int i = 0; i < lasers.Count; i++)
		{
			RaycastHit hit;
			Vector3 newPos = oldBulletPos[i] + targetPositions[i] * bulletSpeed * Time.deltaTime;
			Vector3 bulletDir = newPos - oldBulletPos[i];

			Physics.Raycast(oldBulletPos[i], bulletDir, out hit, 1);
			lasers[i].GetComponent<LineRenderer>().SetPosition(0, oldBulletPos[i]);
			lasers[i].GetComponent<LineRenderer>().SetPosition(1, newPos + bulletDir);

			if(hit.collider != null) {
				Destroy(lasers[i]);
				lasers.RemoveAt(i);
				oldBulletPos.RemoveAt(i);
				targetPositions.RemoveAt(i);

				if(hit.transform.gameObject.tag == "Player")
					gameManager.CurrState = GameManager.States.Lost;
			}

			else {
				oldBulletPos[i] = newPos;
			}
		}
    }
}
