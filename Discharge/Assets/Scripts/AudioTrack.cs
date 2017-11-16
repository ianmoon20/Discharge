using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrack : MonoBehaviour {

	private AudioSource[] tracks;

    public int TrackSize
    { 
        get {
            return tracks.Length;
        }
    }
    
        

	public enum State {t1, t2, t3};

    private State currState = State.t1;

    public State CurrState
    {
        get { return currState; }
        set { currState = value; }
    }

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
        switch (currState)
        {
            case (State.t1):
                tracks[1].mute = true;
                tracks[2].mute = true;
                break;
            case (State.t2):
                tracks[1].mute = false;
                tracks[2].mute = true;
                break;
            case (State.t3):
                tracks[1].mute = false;
                tracks[2].mute = false;
                break;
            default:
                tracks[1].mute = true;
                tracks[2].mute = true;
                break;
        }
	}
}
