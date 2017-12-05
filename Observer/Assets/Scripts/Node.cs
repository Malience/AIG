using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {
    public Vector2 pos;
    public Node next;
    public int id;
    public float cost;

    public Node(float x ,float y)
    {
        pos = new Vector2(x, y);
        this.id = 24; //Isn't a NavCoord
    }

    public Node(Vector2 loc)
    {
        pos = loc;
        this.id = 24; //Isn't a NavCoord
    }

    public Node(Vector2 loc, Node next)
    {
        pos = loc;
        this.id = 24; //Isn't a NavCoord
        this.next = next;
        cost = next.cost + Vector2.Distance(pos, next.pos);
    }

    public Node(NavCoord node)
    {
        this.pos = node.pos;
        this.id = node.id;
    }

    public Node(NavCoord coord, Node next, Vector2 target)
    {
        this.pos = coord.pos;
        this.id = coord.id;
        this.next = next;
        cost = next.cost + Vector2.Distance(coord.pos, target) + Vector2.Distance(coord.pos, next.pos);
    }

    public void SetIfBetter(Node next)
    {
        if(this.next == null)
        {
            return;
        } 
        float nextcost = next.cost + Vector2.Distance(this.pos, next.pos) - this.next.cost - Vector2.Distance(this.pos, this.next.pos);
        if (nextcost < 0) {
            cost += nextcost;
            this.next = next;
        }
    }
}
