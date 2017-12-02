using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VRToggle : MonoBehaviour {

    public bool VREnabled;
    public float renderScale;
	// Use this for initialization
	void Start () {
        XRSettings.enabled = VREnabled;
        //XRSettings.renderScale = renderScale;
        XRSettings.eyeTextureResolutionScale = renderScale;
    }
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.V)) XRSettings.enabled = VREnabled = !VREnabled;
	}
}
