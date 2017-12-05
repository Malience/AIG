using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour {
    public NavCoord[] nodes = new NavCoord[24];
    Vector2 doorLoc; //The actual location as opposed to the node?
    [SerializeField]
    public bool gen = true;
    [SerializeField]
    bool rand = true;
    [SerializeField]
    public int seed;
    [SerializeField]
    Vector2 start;
    [SerializeField]
    bool pathing;

    // Use this for initialization
    void Start () {
        //Center, the place all enemies are going to
        //nodes[0] = Reserved for the door
        //Center Corners
        nodes[1] = new NavCoord(2, 2, 1);
        nodes[2] = new NavCoord(4, 2, 2);
        nodes[3] = new NavCoord(2, 4, 3);
        nodes[4] = new NavCoord(4, 4, 4);
        //5-6 center doors //0-6 total 
        //Inner Corners
        nodes[7] = new NavCoord(1, 1, 7);
        nodes[8] = new NavCoord(5, 1, 8);
        nodes[9] = new NavCoord(1, 5, 9);
        nodes[10] = new NavCoord(5, 5, 10);
        //11-16 inner doors //7-16 total // There may or may not be a 16th door, the world may never know
        //Outer corners
        nodes[17] = new NavCoord(0, 0, 17);
        nodes[18] = new NavCoord(6, 0, 18);
        nodes[19] = new NavCoord(0, 6, 19);
        nodes[20] = new NavCoord(6, 6, 20);
        //21-23 outer doors //17-23 total
        
    }

    // Update is called once per frame
    void Update () {
        if (gen)
        {
            gen = false;
            if (rand) seed = Random.Range(0, 1000000000);
            MazeGen(seed);
        }
        if (pathing)
        {
            pathing = false;
            Node p = path(start, 0);
            string s = "" + p.id;
            p = p.next;
            while (p != null) { s += "\t-->\t" + p.id; p = p.next; }
            Debug.Log(s);
        }
	}

    private void placeCoord(int[] map, int[] nextmap, byte slot, byte nextslot, int loc, int off)
    {
        int div = map.Length >> 2; //fast divide by 4
        if (map[loc] < 0)
        {
            nodes[slot] = new NavCoord(translate(div, loc, off), slot);
            map[loc] = slot;
        } else nodes[slot] = nodes[map[loc]];
        if (nextmap != null)
        {
            int nextdiv = nextmap.Length >> 2;
            int nextloc = 1 + ((loc / div) << 1) + loc;
            nodes[nextslot] = new NavCoord(translate(nextdiv, nextloc, off - 1), nextslot);
            nextmap[nextloc] = nextslot;
            if(nodes[slot].n2 == null) nodes[slot].n2 = nodes[nextslot];
            else nodes[slot].n3 = nodes[nextslot];
            if (nodes[nextslot].n2 == null) nodes[nextslot].n2 = nodes[slot];
            else nodes[nextslot].n3 = nodes[nextslot];
        }
    } 

    private Vector2 translate(int div, int loc, int off)
    {
        switch (loc / div) {
            case 0:
                return new Vector2(off + loc % div, off);
            case 1:
                return new Vector2(off + div - 1, off + loc % div);
            case 2:
                return new Vector2(off + div - loc % div - 1, off + div - 1);
            case 3:
                return new Vector2(off, off + div -  loc % div - 1);
        }
        return Vector2.zero;
    }

    public void MazeGen(int seed)
    {
        for(int i = 1; i < 21; i++) //I KNOW, SUPER INEFFICIENT!
        {
            if (nodes[i] == null) continue;
            nodes[i].n2 = null;
            nodes[i].n3 = null;
        }
        Random.InitState(seed);
        nodes[16] = nodes[7]; // whoops, overcounted, too late to fix now

        int[] centermap = { 1, -1, 2, 2, -1, 4, 4, -1, 3, 3, -1, 1 };
        int[] innermap = { 7, -1, -1, -1, 8, 8, -1, -1, -1, 10, 10, -1, -1, -1, 9, 9, -1, -1, -1, 7 };
        int[] outermap = { 17, -1, -1, -1, -1, -1, 18, 18, -1, -1, -1, -1, -1, 20, 20, -1, -1, -1, -1, -1, 19, 19, -1, -1, -1, -1, -1, 17};
        
        placeCoord(centermap, null, 0, 0, 1 + Random.Range(0, 4) * 3, 2);

        int rand1 = Random.Range(0, 11);
        placeCoord(centermap, innermap, 5, 11, rand1, 2);
        int rand2 = Random.Range(0, 11);
        if (rand2 == rand1)
        {
            int halflen = centermap.Length >> 1;
            rand2 += rand2 >= halflen ? -halflen : halflen;
        }
        placeCoord(centermap, innermap, 6, 12, Random.Range(0, 12), 2);

        rand1 = Random.Range(0, 20);
        placeCoord(innermap, outermap, 13, 21, rand1, 1);

        rand2 = Random.Range(0, 20);
        if (rand2 == rand1)
        {
            int halflen = innermap.Length >> 1;
            rand2 += rand2 >= halflen ? -halflen : halflen;
        }
        placeCoord(innermap, outermap, 14, 22, rand2, 1);

        int rand3 = Random.Range(0, 20);
        if (rand3 == rand1 || rand3 == rand2)
        {
            int div = (rand1 + rand2) / 2;
            if (div == rand1 || div == rand2)
            {
                if (rand2 + 2 >= 19) rand3 = rand2 - 3;
                else rand3 = rand2 + 2;
            }
            else rand3 = div;
        }
        placeCoord(innermap, outermap, 15, 23, rand3, 1);

        connectMap(centermap);
        connectMap(innermap);
        connectMap(outermap);
    }

    public void printMap()
    {
        int[,] map = new int[7, 7];
        for (int i = 0; i < 7; i++) for (int j = 0; j < 7; j++) map[i, j] = -1;
        for (int i = 0; i < 24; i++)
        {
            map[(int)nodes[i].pos.x, (int)nodes[i].pos.y] = nodes[i].id;
        }
        map[3, 3] = -100;
        string s = "";
        for (int i = 6; i >= 0; i--)
        {
            for (int j = 0; j < 7; j++)
                s += map[j, i] + "\t";
            s += "\n";
        }

        Debug.Log(s);

        s = "";
        for (int i = 0; i < 24; i++)
        {
            s += nodes[i].id + " --> " +
                (nodes[i].n0 != null ? ("\t" + nodes[i].n0.id) : "\t") +
                (nodes[i].n1 != null ? ("\t" + nodes[i].n1.id) : "\t") +
                (nodes[i].n2 != null ? ("\t" + nodes[i].n2.id) : "\t") +
                (nodes[i].n3 != null ? ("\t" + nodes[i].n3.id) : "\t") + "\n";
        }
        Debug.Log(s);
    }

    private void connectMap(int[] map)
    {
        int cur = map[0];
        int length = map.Length;
        for (int i = 0; i < length; i++)
        {
            if (map[i] < 0 || nodes[cur] == nodes[map[i]]) continue;
            nodes[cur].n0 = nodes[map[i]];
            nodes[map[i]].n1 = nodes[cur];
            cur = map[i];
        }
    }

    //Pathing should always go to the 
    public Node path(float x, float y) { return path(new Vector2(x, y), 0); }
    public Node path(Vector2 start, int avoid)
    {
        //Nowhere to go
        if (nodes[0] == null) return null;
        NavCoord adj1;
        NavCoord adj2;

        int tier = getTier(start);
        if (tier > 7) return null;
        else if (tier < 2) {
            if (tier == 0) { adj1 = nodes[17]; adj2 = nodes[18]; }
            else { adj1 = nodes[19]; adj2 = nodes[20]; }
            for (int i = 21; i < 24; i++) {
                if (nodes[i].pos.x < start.x) if (nodes[i].pos.x > adj1.pos.x && nodes[i].pos.y - start.y < 1.5f) adj1 = nodes[i];//left
                    else if (nodes[i].pos.x < adj2.pos.x && nodes[i].pos.y - start.y < 1.5f) adj1 = nodes[i];//right
            }
        } else if (tier < 4) {
            if (tier == 2) { adj1 = nodes[17]; adj2 = nodes[19]; }
            else { adj1 = nodes[18]; adj2 = nodes[20]; }
            for (int i = 21; i < 24; i++) {
                if (nodes[i].pos.y < start.y) if (nodes[i].pos.y > adj1.pos.y && nodes[i].pos.x - start.x < 1.5f) adj1 = nodes[i];//bot
                    else if (nodes[i].pos.y < adj2.pos.y && nodes[i].pos.x - start.x < 1.5f) adj1 = nodes[i];//top
            }
        } else if (tier < 6)
        {
            if (tier == 4) { adj1 = nodes[7]; adj2 = nodes[8]; }
            else { adj1 = nodes[9]; adj2 = nodes[10]; }
            for (int i = 11; i < 17; i++)
            {
                if (nodes[i].pos.x < start.x) if (nodes[i].pos.x > adj1.pos.x && nodes[i].pos.y - start.y < 1.5f) adj1 = nodes[i];//left
                else if (nodes[i].pos.x < adj2.pos.x && nodes[i].pos.y - start.y < 1.5f) adj1 = nodes[i];//right
            }
        } else {
            if (tier == 6) { adj1 = nodes[7]; adj2 = nodes[9]; }
            else { adj1 = nodes[8]; adj2 = nodes[10]; }
            for (int i = 11; i < 17; i++)
            {
                if (nodes[i].pos.y < start.y) if (nodes[i].pos.y > adj1.pos.y && nodes[i].pos.x - start.x < 1.5f) adj1 = nodes[i];//bot
                else if (nodes[i].pos.y < adj2.pos.y && nodes[i].pos.x - start.x < 1.5f) adj1 = nodes[i];//top
            }
        }


        Node[] visited = new Node[25]; //All the nodes + start
        visited[0] = new Node(nodes[0]);
        List<Node> heap = new List<Node>(10);
        heap.Add(visited[0]);
        NodeComparer comparer = new NodeComparer();

        Node current = null;
        while (heap.Count > 0)
        {
            current = heap[0];
            heap.RemoveAt(0);

            if (avoid != 0 && current.id == avoid) continue;
            if (current.id == 24) return current; //redacted: ---------------YES I REALIZE THIS CHECKS THE MEMORY ADDRESS!!

            NavCoord coord = nodes[current.id];

            if (coord == adj1 || coord == adj2)
            {
                if (visited[24] == null)
                {
                    visited[24] = new Node(start, current);
                    heap.Add(visited[24]);
                }
                else visited[24].SetIfBetter(current);
            }

            int id = coord.n0.id;
            if (visited[id] == null)
            {
                visited[id] = new Node(coord.n0, current, start);
                heap.Add(visited[id]);
            } else visited[id].SetIfBetter(current);
            id = coord.n1.id;
            if (visited[id] == null)
            {
                visited[id] = new Node(coord.n1, current, start);
                heap.Add(visited[id]);
            }
            else visited[id].SetIfBetter(current);

            if (coord.n2 != null)
            {
                id = coord.n2.id;
                if (visited[id] == null)
                {
                    visited[id] = new Node(coord.n2, current, start);
                    heap.Add(visited[id]);
                }
                else visited[id].SetIfBetter(current);
            }

            if (coord.n3 != null)
            {
                id = coord.n3.id;
                if (visited[id] == null)
                {
                    visited[id] = new Node(coord.n3, current, start);
                    heap.Add(visited[id]);
                }
                else visited[id].SetIfBetter(current);
            }

            heap.Sort(comparer);
        }
        
        return null;
    }

    private int getTier(Vector2 pos)
    {
        //Outer
        if (pos.y < 2) return 0;
        if (pos.y >= 6) return 1;
        if (pos.x < 2) return 2;
        if (pos.x >= 6) return 3;
        //Inner
        if (pos.y < 3) return 4;
        if (pos.y >= 5) return 5;
        if (pos.x < 3) return 6;
        if (pos.x >= 5) return 7;
        return 8;
    }

    private class NodeComparer : Comparer<Node> {
        public override int Compare(Node x, Node y){return x.cost > y.cost ? 1 : (x.cost == y.cost ? 0 : -1);}
    }
}
