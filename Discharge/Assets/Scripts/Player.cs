using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Player : MonoBehaviour {

    public Animator anim;

	private ThirdPersonCharacter tpc;
	private ThirdPersonUserControl tpuc;
	private bool sprinting = false;
	private bool crouching = false;
	private SphereCollider noiseBubble;
	private AudioSource[] audioSources;
	bool playNum = false;
	[SerializeField]float stepRate = 0.2f;
	float stepProgress = 0.0f;
	float playVolume = 0.5f;
	Vector3 curPos;
	Vector3 prevPos;

    //Property for whether the player is in light or not
    public bool isLit;

    public bool IsLit
    {
        get { return isLit; }
        set { isLit = value; }
    }


    [SerializeField] float crouchRadius = 5;
	[SerializeField] float walkRadius = 10;
	[SerializeField] float sprintRadius = 25;

	// Use this for initialization
	void Start () {
		tpc = gameObject.GetComponent<ThirdPersonCharacter> ();
		tpuc = gameObject.GetComponent<ThirdPersonUserControl> ();
		noiseBubble = gameObject.GetComponent<SphereCollider> ();
		audioSources = gameObject.GetComponents<AudioSource>();
		curPos = prevPos = tpc.transform.position;

        anim = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update () {
		curPos = tpc.transform.position;
		if((prevPos.x + 0.005f) < curPos.x || (prevPos.x - 0.005f) > curPos.x ||
		   (prevPos.z + 0.005f) < curPos.z || (prevPos.z - 0.005f) > curPos.z) {
			stepProgress += Time.deltaTime;
		}

		prevPos = curPos;

		crouching = tpc.Crouching;
		sprinting = tpuc.Sprinting;

		if(tpc.M_ForwardAmount <= 0){
			noiseBubble.radius = 0;
		}else if (crouching) {
			noiseBubble.radius = crouchRadius;
			stepRate = 0.4f;
			playVolume = 0.15f;
		} else if (sprinting) {
			noiseBubble.radius = sprintRadius;
			stepRate = 0.50f;
			playVolume = 0.35f;
		} else {
			noiseBubble.radius = walkRadius;
			stepRate = 0.5f;
			playVolume = 0.25f;
		}

		if(stepProgress >= stepRate) {
				int audioNum = playNum ? 1 : 0;
				audioSources[audioNum].volume = playVolume;
				audioSources[audioNum].Play();
				playNum = !playNum;
				stepProgress = 0;
		}

        if (Input.GetButtonDown("Fire1"))
        {
            RaycastHit hit;

            //Checking to see if anything is in between the light and the position
			if (Physics.Raycast(gameObject.transform.position + new Vector3(0,1.6f,0), gameObject.transform.forward, out hit, 2f))
            {
                Debug.Log(hit.transform.tag);
                //If the player is hit, nothing is in the way
                if (hit.transform.gameObject.tag == "lightBox")
                {
                    hit.transform.GetComponent<Lights>().IsEnabled = !hit.transform.GetComponent<Lights>().IsEnabled;
                }
            }
        }
    }

	void OnTriggerStay(Collider c){
		if (c.gameObject.tag == "Enemy") {
            Enemy enemy = c.gameObject.GetComponent<Enemy>();
            enemy.investigateLocation(this.gameObject.transform.position);
		}
	}
}
