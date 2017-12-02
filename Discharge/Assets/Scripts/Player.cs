using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Player : MonoBehaviour {

	private ThirdPersonCharacter tpc;
	private ThirdPersonUserControl tpuc;
	private bool sprinting = false;
	private bool crouching = false;
	private SphereCollider noiseBubble;

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
	}
	
	// Update is called once per frame
	void Update () {
		crouching = tpc.Crouching;
		sprinting = tpuc.Sprinting;

		if(tpc.M_ForwardAmount <= 0){
			noiseBubble.radius = 0;
		}else if (crouching) {
			noiseBubble.radius = crouchRadius;
		} else if (sprinting) {
			noiseBubble.radius = sprintRadius;
		} else {
			noiseBubble.radius = walkRadius;
		}

        if (Input.GetButtonDown("Fire1"))
        {
            RaycastHit hit;

            //Checking to see if anything is in between the light and the position
            if (Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out hit, 2f))
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
