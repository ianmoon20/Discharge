using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevel : MonoBehaviour {
    GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Use this for initialization
    void OnTriggerEnter(Collider other) {
		if(other.gameObject.tag == "Player")
        {
            gameManager.CurrState = GameManager.States.Won;
        }
	}
}
