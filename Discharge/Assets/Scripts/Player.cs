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

        
	}

	void OnTriggerStay(Collider c){
		if (c.gameObject.tag == "Enemy") {
            Enemy enemy = c.gameObject.GetComponent<Enemy>();
            enemy.investigateLocation(this.gameObject.transform.position);
		}
	}
}
