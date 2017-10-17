using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power : MonoBehaviour
{
    Vector3 coalShaft = new Vector3(-0.39f, 3.61f, -4.97f );
    public int power = 0;

    private void OnCollisionEnter (Collision collision)
    {
        if (collision.collider.tag == "coal")
        {
            collision.transform.position = coalShaft;
            power++;
            print("Coal in furnace");
        }

    }
}
