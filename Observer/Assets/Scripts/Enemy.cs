using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public Node path;
    public GameObject parent; //BECAUSE STUPID UNITY DOESN'T HAVE THIS AS A FIELD LIKE ANY GOOD ENGINE WOULD
    //I MEAN LIKE COME ON ITS SO SIMPLE!!!!!
    public Transform trans;
    public byte id;
    public byte group;

    public void move(float movement)
    {
        if (path == null) return;
        float dist = Vector2.Distance(trans.localPosition, path.pos);
        while(dist < movement)
        {
            trans.localPosition = path.pos;
            if (path.next == null) return;
            path = path.next;
            movement -= dist;
            dist = Vector2.Distance(trans.localPosition, path.pos);
        }

        float div = movement / Vector2.Distance(trans.localPosition, path.pos);
        //Debug.Log(div);
        trans.localPosition = Vector2.Lerp(trans.localPosition, path.pos, div);
    }

    public void setPath(Node newpath, Node oldpath)
    {
        if(oldpath == null)
        {
            this.path = newpath;
            return;
        }
        Node cur = path;
        while (cur.next != oldpath) cur = cur.next;
        cur.next = newpath;
    }

	// Use this for initialization
	void Start() {  
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
