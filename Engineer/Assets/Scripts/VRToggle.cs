using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class VRToggle : MonoBehaviour {

    public bool VREnabled;
    public float renderScale;
	// Use this for initialization
	void Start () {
        VRSettings.enabled = VREnabled;
        VRSettings.renderScale = renderScale;
        
    }
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.V)) VRSettings.enabled = VREnabled = !VREnabled;
	}
}
