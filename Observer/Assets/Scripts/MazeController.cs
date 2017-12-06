using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeController : MonoBehaviour {
    private const string ENEMY_PREFAB = "Enemy";
    private const string DOOR_PREFAB = "Door";
    private const string FLAME_PREFAB = "Flame";

    public static MazeController mcont;


    [SerializeField]
    int maxEnemies = 5;
    [SerializeField]
    int maxGroups = 3;

    [SerializeField]
    Maze maze;

    [SerializeField]
    Vector3 offset;

    [SerializeField]
    Enemy[] enemy;
    [SerializeField]
    Group[] group;
    [SerializeField]
    public Flame[] flame;

    [SerializeField]
    float splitChance = 0.2f;
    [SerializeField]
    float splitMod = 0.1f;

    public bool server = true;

    public void KillEnemy(byte id)
    {
        Enemy enemy = this.enemy[id];
        Group group = this.group[enemy.group];
        enemy.parent.SetActive(false);
        if (!server) return;
        float split = ++group.casualties * splitMod + splitChance;
        if(Random.value < split)
        {
           // SplitGroup(group, 4);
        }
    }

    [SerializeField]
    float speed = 0.25f;

    // Use this for initialization
    void Start () {
        mcont = this;

        enemy = new Enemy[maxEnemies];
        group = new Group[maxGroups];

        for(byte i = 0; i < maxEnemies; i++)
        {
            GameObject o = Instantiate(Resources.Load(ENEMY_PREFAB), new Vector3(-1, -1, 0), Quaternion.identity) as GameObject;
            enemy[i] = o.GetComponent<Enemy>();
            enemy[i].trans = o.transform;
            enemy[i].parent = o;
            enemy[i].trans.parent = this.transform;
            enemy[i].id = i;
            o.SetActive(false);
        }

        for(int i = 0; i < maxGroups; i++)
        {
            group[i] = new Group();
            group[i].speed = speed;
        }

        flame = new Flame[6];
        for(int i = 0; i < 6; i++)
        {
            GameObject o = Instantiate(Resources.Load(FLAME_PREFAB)) as GameObject;
            flame[i] = o.GetComponent<Flame>();
            flame[i].trans = o.transform;
            flame[i].parent = o;
            flame[i].trans.parent = this.transform;
            flame[i].mcont = this;
            o.SetActive(false);
        }

        for(int i = 0; i < 11; i++)
        {
            door[i] = Instantiate(Resources.Load(DOOR_PREFAB)) as GameObject;
            door[i].SetActive(false);
            doortrans[i] = door[i].transform;
            doortrans[i].parent = this.transform;
        }


        //MapGen(mapSeed);
    }

    public void CreateGroup(byte groupid, byte loc, byte[] enemyids) { CreateGroup(groupid, loc, enemyids, enemyids.Length); }
    public void CreateGroup(byte groupid, byte loc, byte[] enemyids, int len)
    {
        Enemy[] squad = new Enemy[len];
        group[groupid].squad = squad;
        Vector3 start = new Vector3((loc & 0x1) > 0 ? 6 : 0, loc > 1 ? 6 : 0, 0);
        Vector3 off = loc > 1 ? offset * -1 : offset;
        for (int i = 0; i < len; i++)
        {
            squad[i] = enemy[enemyids[i]];
            squad[i].parent.SetActive(true);
            squad[i].trans.localPosition = start;
            squad[i].group = groupid;
            start += off;
        }
        group[groupid].ResetPath(maze);
        group[groupid].casualties = 0;
    }

    public void SplitGroup(int group, byte groupid, byte elen) { SplitGroup(this.group[group], groupid, elen); }
    public void SplitGroup(Group group, byte groupid, byte elen)
    {
        //SPLIT CODE
        int glen = group.squad.Length;
        byte[] ids = new byte[glen];
        int len = 0;

        for (int i = glen - 1; i >= 0; i--)
        //for(int i = 0; i < glen; i++)
        {
            if (group.squad[i] == null || !group.squad[i].parent.activeInHierarchy) break;
            if (len >= elen) break;
            ids[len++] = group.squad[i].id;
            group.squad[i] = null;
        }
        if (len <= 0) return;
        Enemy[] squad = new Enemy[len];
        group = this.group[groupid];
        group.squad = squad;
        for (int i = 0; i < len; i++)
        {
            squad[i] = enemy[ids[i]];
            squad[i].group = groupid;
        }
        group.PathOpposite(maze);
        group.casualties = 0;
    }

    GameObject[] door = new GameObject[11];
    Transform[] doortrans = new Transform[11];
    int next = 0;

    [SerializeField]
    bool genMap = false;
    [SerializeField]
    int mapSeed = -1;

    public void MapGen(int seed)
    {
        maze.seed = seed < 0 ? Random.Range(0, 1000000000) : seed;
        maze.MazeGen(maze.seed);

        next = 0;

        door[next].SetActive(true);
        Vector2 dir = new Vector2(3, 3) - maze.nodes[0].pos;
        doortrans[next++].localPosition = maze.nodes[0].pos + dir * .5f;

        PlaceDoor(5);
        PlaceDoor(6);
        PlaceDoor(13);
        PlaceDoor(14);
        PlaceDoor(15);

        nextflame = 0;

        AdjustFlame(0, 0);
        AdjustFlame(11, 1);
        AdjustFlame(12, 1);
        AdjustFlame(21, 2);
        AdjustFlame(22, 2);
        AdjustFlame(23, 2);
    }

    int nextflame = 0;

    [SerializeField]
    float flameScale = 1.3f;
    [SerializeField]
    float innerFlameScale = 4;
    [SerializeField]
    float outerFlameScale = 4;

    [SerializeField]
    float centerKillChance = 1;
    [SerializeField]
    float innerKillChance = 0.03f;
    [SerializeField]
    float outerKillChance = 0.005f;

    public void AdjustFlame(int node, int level)
    {
        Flame f = flame[nextflame++];
        f.killchance = level == 0 ? centerKillChance : (level == 1 ? innerKillChance : outerKillChance);
        NavCoord nav = maze.nodes[node];
        Vector3 v = (nav.pos - (node == 0 ? new Vector2(3,3) : nav.n2.pos)).normalized;
        bool vertical = v.y == 0;
        float scale = level == 0 ? flameScale : (level == 1 ? innerFlameScale : outerFlameScale);
        f.trans.localScale = new Vector3(vertical ? flameScale : scale, vertical ? scale : flameScale, 0);
        f.trans.localPosition = nav.pos;
        f.parent.SetActive(false);
    }

    public void PlaceDoor(int node)
    {
        NavCoord nav = maze.nodes[node];
        door[next].SetActive(true);
        Vector2 dir = nav.n2.pos - nav.pos;
        doortrans[next++].localPosition = nav.pos + dir * .5f;
        if(nav.n3 != null)
        {
            door[next].SetActive(true);
            dir = nav.n3.pos - nav.pos;
            doortrans[next++].localPosition = nav.pos + dir * .5f;
        } else door[next++].SetActive(false);
    }

    [SerializeField]
    bool createGroup = false;

    [SerializeField]
    byte groupid;
    [SerializeField]
    byte loc;
    [SerializeField]
    byte[] enemyids;


    [SerializeField]
    bool printMap = false;

    [SerializeField]
    int printGroup = 0;
    [SerializeField]
    bool printPath = false;


    [SerializeField]
    public bool[] lever = new bool[6];



    // Update is called once per frame
    void Update () {
        for(int i = 0; i < 6; i++)
        {
            flame[i].parent.SetActive(lever[i] && UIScript.ui.CanPowerTrap(i));
        }
        if (printPath)
        {
            printPath = false;
            Node n = group[printGroup].squad[0].path;
            string s = n.id + " (" + n.pos.x + ", " + n.pos.y + ")";
            while(n.next != null)
            {
                n = n.next;
                s += " --> " + n.id + " (" + n.pos.x + ", " + n.pos.y + ")";
            }
            Debug.Log(s);
        }
        if (printMap)
        {
            printMap = false;
            maze.printMap();
        }
        if (genMap)
        {
            genMap = false;
            MapGen(mapSeed);
        }
        if (createGroup)
        {
            createGroup = false;
            CreateGroup(groupid, loc, enemyids);
        }
        for (int i = 0; i < maxGroups; i++)
            group[i].move();
	}
}
