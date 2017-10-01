using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class InteractionScript : MonoBehaviour {
    private EVRButtonId trigger = EVRButtonId.k_EButton_SteamVR_Trigger;

    private SteamVR_Controller.Device controller;
    private SteamVR_TrackedObject trackedController;
    private FixedJoint joint;

    [SerializeField]
    private GameObject obj;

    // Use this for initialization
    void Start () {
        trackedController = this.gameObject.GetComponent<SteamVR_TrackedObject>();
        joint = this.gameObject.GetComponent<FixedJoint>();
        controller = SteamVR_Controller.Input((int)trackedController.index);
    }
	
	// Update is called once per frame
	void Update () {
		if(controller == null)
        {
            Debug.Log("Everything has gone wrong, panic");
            return;
        }

        if (controller.GetPressDown(trigger))
        {
            if (obj != null) joint.connectedBody = obj.GetComponent<Rigidbody>();
            else
                joint.connectedBody = null;
        }
        else if (controller.GetPressUp(trigger))
        {
            Rigidbody rb = joint.connectedBody.GetComponent<Rigidbody>();
            joint.connectedBody = null;
            rb.velocity = controller.velocity;
            rb.angularVelocity = controller.angularVelocity;
        }
	}

    void OnTriggerStay(Collider o)
    {
        if (o.CompareTag("Interactable"))
        {
            obj = o.gameObject;
        }
    }

    void OnTriggerExit(Collider o)
    {
        obj = null;
    }
}
