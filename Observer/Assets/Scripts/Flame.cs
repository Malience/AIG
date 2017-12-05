using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame : MonoBehaviour {
    public MazeController mcont;
    public GameObject parent;
    public Transform trans;
    public float killchance = 1;

    void OnTriggerStay2D(Collider2D c)
    {
        if (!mcont.server) return;
        if(Random.value <= killchance)
        {
            mcont.KillEnemy(c.gameObject.GetComponent<Enemy>().id);
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
