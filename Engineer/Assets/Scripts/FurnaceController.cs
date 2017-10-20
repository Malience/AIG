using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnaceController : MonoBehaviour {
    public static FurnaceController furnace;

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
    public LeverScript lever;

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

    // Use this for initialization
    void Start () {
        furnace = this;
	}

    bool lastLever = false;

	// Update is called once per frame
	void FixedUpdate () {
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

        if (lever.active & power >= elevatorDrain & elevatorPower != elevatorMax)
        {
            power -= elevatorDrain;
            elevatorPower += elevatorCharge;
            if (elevatorPower > elevatorMax) elevatorPower = elevatorMax;
        }

        if(lever.active & !lastLever)
        {
            NetworkManager.manager.SendByteCode(NetworkManager.LEVE);
        }

        if (!lever.active & lastLever)
        {
            NetworkManager.manager.SendByteCode(NetworkManager.EVEL);
        }
        lastLever = lever.active;

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
