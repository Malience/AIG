using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavCoord {
    public Vector2 pos;
    //Because of the shape of the maze a node can only be connected to 3 other nodes
    public NavCoord n0, n1, n2, n3;
    public byte id;

    public NavCoord(float x, float y, byte id)
    {
        pos = new Vector2(x, y);
        this.id = id;
    }

    public NavCoord(Vector2 v, byte id)
    {
        pos = v;
        this.id = id;
    }
}
