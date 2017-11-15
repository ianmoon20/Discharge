﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrack : MonoBehaviour {

	private AudioSource[] tracks;
	public enum State {t1, t2, t3};
	public State curState = State.t1;

	// Use this for initialization
	void Start () {
		tracks = GetComponents<AudioSource>();
		tracks[0].Play();
		tracks[1].mute = true;
		tracks[1].Play();
		tracks[2].mute = true;
		tracks[2].Play();
	}

	// Update is called once per frame
	void Update () {
		if(curState == State.t1) {
			tracks[1].mute = true;
			tracks[2].mute = true;
		}

		else if(curState == State.t2) {
			tracks[1].mute = false;
			tracks[2].mute = true;
		}

		else if(curState == State.t3) {
			tracks[1].mute = false;
			tracks[2].mute = false;
		}
	}
}