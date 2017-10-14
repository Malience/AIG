using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Levers : MonoBehaviour
{
    bool on = false;
    public float timer;

    private void OnTriggerStay(Collider col)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            on = true;
            Debug.Log("Lever on");
            StartCoroutine("leverOff");
        }
    }

    IEnumerator leverOff()
    {
        yield return new WaitForSeconds(timer);
        on = false;
        Debug.Log("Lever off");
    }
}
