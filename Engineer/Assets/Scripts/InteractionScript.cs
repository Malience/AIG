using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class InteractionScript : MonoBehaviour {
    private EVRButtonId trigger = EVRButtonId.k_EButton_SteamVR_Trigger;

    private SteamVR_Controller.Device controller;
    private SteamVR_TrackedObject trackedController;
    private FixedJoint joint;
    private SpringJoint leverJoint;

    [SerializeField]
    private GameObject obj;


    // Use this for initialization
    void Start () {
        trackedController = this.gameObject.GetComponent<SteamVR_TrackedObject>();
        joint = this.gameObject.GetComponent<FixedJoint>();
        leverJoint = this.gameObject.GetComponent<SpringJoint>();
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
            if (obj != null)
            {
                if (obj.CompareTag("Interactable")) joint.connectedBody = obj.GetComponent<Rigidbody>();
                else
                {
                    leverJoint.connectedBody = obj.GetComponent<Rigidbody>();
                }
            }
            else
            {
                joint.connectedBody = null;
                leverJoint.connectedBody = null;
            }
        }
        else if (controller.GetPressUp(trigger) && (joint.connectedBody != null || leverJoint.connectedBody != null))
        {
            if (obj.CompareTag("Interactable"))
            {
                Rigidbody rb = joint.connectedBody.GetComponent<Rigidbody>();
                joint.connectedBody = null;
                rb.velocity = controller.velocity * 3;
                rb.angularVelocity = controller.angularVelocity * 3;
            } else
            {
                joint.connectedBody = null;
                leverJoint.connectedBody = null;
            }
        }
	}

    void OnTriggerStay(Collider o)
    {
        if (o.CompareTag("Interactable") || o.CompareTag("Lever"))
        {
            obj = o.gameObject;
        }
    }

    void OnTriggerExit(Collider o)
    {
        if (joint.connectedBody != null || leverJoint.connectedBody != null) return;
        obj = null;
    }
}
