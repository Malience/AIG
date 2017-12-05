using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnaceController : MonoBehaviour {
    public static FurnaceController furnace;
    public MazeController mcont;

    [SerializeField]
    int syncframe = 20;
    int frame;

    [SerializeField] Light furnaceLight;
    [SerializeField]
    float lightStep;
    [SerializeField]
    float lightMax;
    [SerializeField]
    float initScale;
    [SerializeField]
    float scaleStep;

    [SerializeField]
    public float powerStep;
    [SerializeField]
    public float powerLoss;

    [SerializeField]
    public float elevatorDrain;
    [SerializeField]
    public float elevatorCharge;
    [SerializeField]
    public float elevatorMax;
    [SerializeField]
    public float elevatorPower;

    [SerializeField]
    public float centerTrapDrain;
    [SerializeField]
    public float innerTrapDrain;
    [SerializeField]
    public float outerTrapDrain;

    [SerializeField]
    public LeverScript chargelever;
    [SerializeField]
    public LeverScript pushToClose;

    [SerializeField]
    public float genMax;
    [SerializeField]
    float powerMax;

    [SerializeField]
    public float powerGen;
    [SerializeField]
    public float power;
    [SerializeField]
    public int coalBurning;

    [SerializeField]
    Vector3 coalSpawn;

    List<GameObject> coal = new List<GameObject>();
    List<GameObject> removal = new List<GameObject>();

    public bool CanPowerTrap(int trap)
    {
        float needed = (trap == -1 ? centerTrapDrain : (trap < 2 ? innerTrapDrain : outerTrapDrain));
        return needed < power;
    }

    // Use this for initialization
    void Start () {
        furnace = this;
	}

    bool lastLever = false;

    [SerializeField]
    int maxfloor = 3;
    [SerializeField]
    int floor = 1;

	// Update is called once per frame
	void FixedUpdate () {
        if (!mcont.started) return;
        foreach (GameObject o in coal)
        {
            o.transform.localScale -= Vector3.one * scaleStep;
            if (o.transform.localScale.x < 0)
            {
                removal.Add(o);
            }
        }
        if (removal.Count > 0)
        {
            foreach (GameObject o in removal)
            {
                o.transform.localPosition = coalSpawn;
                o.transform.localScale = Vector3.one * initScale;
                coal.Remove(o);
                coalBurning--;
                NetworkManager.manager.SendByteCode(NetworkManager.BURN);
            }
            removal.Clear();
        }
        powerGen += coalBurning * powerStep - powerLoss;
        if (powerGen < 0) powerGen = 0;
        else if (powerGen > genMax) powerGen = genMax;
        power += powerGen;

        if(pushToClose.active)
        {
            if(elevatorPower == elevatorMax)
            {
                if(floor >= maxfloor)
                {
                    Debug.Log("YOU WIN!!!!");
                    NetworkManager.manager.SendByteCode(NetworkManager.VICT);
                }
                elevatorPower = 0;
                mcont.resetMap();
                int rand = Random.Range(0, 1000000000);
                mcont.MapGen(rand);
                mcont.nextwave = 15f;
                mcont.powerbuilt += 1;
                mcont.power += mcont.powerbuilt * 3;
                mcont.wavetimers -= 1;
                floor++;
            }
        }
        else if (chargelever.active & power >= elevatorDrain & elevatorPower != elevatorMax)
        {
            power -= elevatorDrain;
            elevatorPower += elevatorCharge;
            if (elevatorPower > elevatorMax) elevatorPower = elevatorMax;
        }

        if (mcont.doorlever.active)
            power -= centerTrapDrain;
        for (int i = 0; i < 5; i++)
        {
            if (mcont.traplever[i].active) power -= i < 2 ? innerTrapDrain : outerTrapDrain;
        }

        if(chargelever.active & !lastLever)
        {
            NetworkManager.manager.SendByteCode(NetworkManager.LEVE);
        }

        if (!chargelever.active & lastLever)
        {
            NetworkManager.manager.SendByteCode(NetworkManager.EVEL);
        }
        lastLever = chargelever.active;

        if (power < 0) power = 0;
        else if (power > powerMax) power = powerMax;

        

        furnaceLight.intensity = powerGen / genMax * lightMax;

        frame++;
        if(frame >= syncframe)
        {
            frame = 0;
            NetworkManager.manager.SendSync();
        }
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.tag != "Interactable") return;
        coal.Add(c.gameObject);
        coalBurning ++;
        NetworkManager.manager.SendByteCode(NetworkManager.COAL);
    }
}
