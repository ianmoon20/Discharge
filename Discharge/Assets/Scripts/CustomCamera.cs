using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CustomCamera : MonoBehaviour {
	
	private GameObject player;
	private Vector3 displacement;
	[SerializeField] float Sensitivity = 2.0f;
	[SerializeField] float distanceFromPlayer = 1;
	[SerializeField] float cameraHeight = 1.6f;
	[SerializeField] float lookAtHeight = 1.2f;
	[SerializeField] float rotation = 0;
	[SerializeField] float yRotation = 0;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectsWithTag ("Player") [0];
		rotation = Mathf.Deg2Rad * rotation;
		yRotation = Mathf.Deg2Rad * yRotation;
	}

	// Update is called once per frame
	void Update () {
		float turnAmountX = CrossPlatformInputManager.GetAxis("Mouse X") * -2.0f;
		float turnAmountY = CrossPlatformInputManager.GetAxis("Mouse Y") * 1.0f;
		turnAmountX *= Time.deltaTime;
		turnAmountY *= Time.deltaTime;
		rotation += turnAmountX;
		yRotation += turnAmountY;
		if(yRotation + turnAmountY < (Mathf.Deg2Rad * -45))
			yRotation = (Mathf.Deg2Rad * -45);
		if(yRotation + turnAmountY > (Mathf.Deg2Rad * 45))
			yRotation = (Mathf.Deg2Rad * 45);
		
		displacement = new Vector3 (Mathf.Cos (rotation), 0, Mathf.Sin (rotation));
		displacement = displacement.normalized * distanceFromPlayer;
		displacement.y = cameraHeight;

        this.transform.position = player.transform.position + displacement;
		this.transform.LookAt (player.transform.position + new Vector3 (0, lookAtHeight, 0));
		this.transform.LookAt (gameObject.transform.position + new Vector3(0,Mathf.Sin(yRotation),0) + (gameObject.transform.forward*Mathf.Cos(yRotation)));
	}
}
