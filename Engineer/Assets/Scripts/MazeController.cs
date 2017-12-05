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
    public Maze maze;

    [SerializeField]
    Vector3 offset;

    [SerializeField]
    Enemy[] enemy;
    [SerializeField]
    Group[] group;
    [SerializeField]
    Flame[] flame;

    [SerializeField]
    float splitChance = 0.2f;
    [SerializeField]
    float splitMod = 0.1f;

    EnemyArrayTable enemyTable;

    public bool server = true;

    bool hit = false;

    [SerializeField]
    Rigidbody doorRB;

    public void Hit(Enemy e)
    {
        if (!hit) {
            hit = true;
            KillEnemy(e.id);
            //Door script here
            doorRB.constraints = RigidbodyConstraints.None;
            doorRB.AddExplosionForce(20, new Vector3(Random.value, Random.value, -10), 20);
            //doorRB.freezeRotation = false;
            //doorRB.AddTorque(new Vector3(Random.value * 10, 0, 0));
            //doorRB.AddForce(new Vector3(50, 0, 0));
        }
        else
        {
            NetworkManager.manager.SendByteCode(NetworkManager.DEFE);
            //Lose Script here
        }
    }

    public void KillEnemy(byte id)
    {
        Enemy enemy = this.enemy[id];
        Group group = this.group[enemy.group];
        enemy.parent.SetActive(false);
        enemyTable.destroyEnemy(id);
        NetworkManager.manager.SendKillEnemy(id);
        if (!server) return;
        float split = ++group.casualties * splitMod + splitChance;
        if(Random.value < split && enemyTable.hasGroup())
        {
            SplitGroup(group);
        }
        if (group.casualties >= group.squad.Length) enemyTable.destroyGroup(enemy.group);
    }

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
        }

        enemyTable = new EnemyArrayTable(maxEnemies, maxGroups);

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


        MapGen(mapSeed);
    }

    [SerializeField]
    float easyMod = 0.5f;
    [SerializeField]
    float normalMod = 1;
    [SerializeField]
    float hardMod = 2;

    [SerializeField]
    float diffMod;

    [SerializeField]
    public float powerbuilt = 5;
    [SerializeField]
    public float wavetimers = 10;
    [SerializeField]
    public float nextwave = 10f;
    [SerializeField]
    public float power = 10;

    public void resetMap()
    {
        for (int i = 0; i < maxGroups; i++)
        {
            bool pushed = false;
            Group g = group[i];
            if (g.squad == null) continue;
            for (int j = 0; j < g.squad.Length; j++)
            {
                Enemy e = g.squad[j];
                if (e != null)
                {
                    enemyTable.destroyEnemy(e.id);
                    g.squad[j] = null;
                    if (!pushed)
                    {
                        pushed = true;
                        enemyTable.destroyGroup(e.group);
                    }
                }
            }
        }
    }

    [SerializeField]
    GameObject gameStart;

    [SerializeField]
    public bool started = false;

    public void FixedUpdate()
    {
        if (!started) return;
        nextwave -= Time.fixedDeltaTime;
        if(nextwave <= 0)
        {
            power += powerbuilt * (1 + Random.value * diffMod);

            if(Random.value * diffMod * power > 0.2f)
            {
                
                byte loc1 = (byte)Random.Range(0, 4);
                int len = (int)(2 + power / 5 * Random.value);
                power -= 5 * len - 10;
                CreateGroup(len, loc1);
                if (Random.value * diffMod * power > 0.9f)
                {
                    int loc2 = Random.Range(0, 4);
                    if (loc2 == loc1) loc2 = (loc1 == 3) ? 2 : (loc1 + 1);

                    len = (int)(2 + power / 5 * Random.value);
                    power -= 5 * len;
                    CreateGroup(len, (byte)loc2);
                }
            }

            nextwave = wavetimers - power * diffMod / 5 * (1 - Random.value);
        }
    }

    public void CreateGroup(int len)
    {
        CreateGroup(len, (byte)Random.Range(0, 4));
    }
    public void CreateGroup(int len, byte loc)
    {
        if (!enemyTable.hasGroup() || !enemyTable.hasEnemies(len)) return;
        byte groupid = enemyTable.createGroup();
        byte[] enemyids = enemyTable.createEnemy(len);
        CreateGroup(groupid, loc, enemyids, len);
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

        NetworkManager.manager.SendSpawn(groupid, loc, enemyids);
    }

    public void SplitGroup(int group) { SplitGroup(this.group[group]); }
    public void SplitGroup(Group group)
    {
        //SPLIT CODE
        int glen = group.squad.Length;
        byte[] ids = new byte[glen];
        byte len = 0;

        for (int i = glen - 1; i >= 0; i--)
        //for(int i = 0; i < glen; i++)
        {
            if (group.squad[i] == null || !group.squad[i].parent.activeInHierarchy) break;
            ids[len++] = group.squad[i].id;
            group.squad[i] = null;
        }
        if (len <= 0) return;
        byte groupid = enemyTable.createGroup();
        group.casualties += len;
        Enemy[] squad = new Enemy[len];
        NetworkManager.manager.SendSplit(enemy[ids[0]].id, groupid, len);
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
        if(NetworkManager.manager != null) NetworkManager.manager.SendNext(maze.seed);
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

        for (var i = 4; i > 0; i--)
        {
            int rand = Random.Range(0, i);
            LeverScript temp = traplever[i];
            traplever[i] = traplever[rand];
            traplever[rand] = temp;
        }
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
    public LeverScript doorlever;
    [SerializeField]
    public LeverScript[] traplever = new LeverScript[5];

    [SerializeField]
    public LeverScript easylever;
    [SerializeField]
    public LeverScript mediumlever;
    [SerializeField]
    public LeverScript hardlever;


    private bool[] traplast = new bool[6];
    // Update is called once per frame
    void Update () {
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

        if (!started)
        {
            if(easylever.active || mediumlever.active || hardlever.active)
            {
                diffMod = easylever.active ? easyMod : (mediumlever.active ? normalMod : hardMod);
                gameStart.SetActive(false);
                started = true;
            }
            return;
        }


        if (traplast[0] != doorlever.active) NetworkManager.manager.SendTrap(0, doorlever.active);
        flame[0].parent.SetActive(doorlever.active && FurnaceController.furnace.CanPowerTrap(-1));
        traplast[0] = doorlever.active;
        for (int i = 0; i < 5; i++)
        {
            flame[i + 1].parent.SetActive(traplever[i].active && FurnaceController.furnace.CanPowerTrap(i));
            if (traplast[i + 1] != traplever[i].active) NetworkManager.manager.SendTrap((byte)(i + 1), traplever[i].active);
            traplast[i + 1] = traplever[i].active;
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
        
        
        for (int i = 0; i < maxGroups; i++)
            group[i].move();
	}
}
