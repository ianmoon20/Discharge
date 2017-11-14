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
		crouching = tpc.GetCrouching ();
		sprinting = tpuc.GetSprint ();

		if (crouching) {
			noiseBubble.radius = 10;
		} else if (sprinting) {
			noiseBubble.radius = 50;
		} else {
			noiseBubble.radius = 20;
		}
	}

	void OnTriggerEnter(Collider c){
		if (c.gameObject.tag == "Enemy") {
			Debug.Log ("An Enemy Found me!");
		}
	}
}
