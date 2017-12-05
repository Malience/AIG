using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverScript : MonoBehaviour {
    HingeJoint hinge;
    private const float MIN = -155;
    private const float MAX = -25;
    private const float MID = (MAX - MIN) / 2 + MIN;

    [SerializeField]
    public bool active = false;

	// Use this for initialization
	void Start () {
        hinge = gameObject.GetComponent<HingeJoint>();
	}
	
	// Update is called once per frame
	void Update () {
        active = hinge.angle < MID;
	}
}
