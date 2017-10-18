using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnaceController : MonoBehaviour {
    [SerializeField]
    public static FurnaceController furnace;

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
    float powerStep;
    [SerializeField]
    float powerLoss;


    [SerializeField]
    float genMax;
    [SerializeField]
    float powerMax;

    [SerializeField]
    float powerGen;
    [SerializeField]
    float power;
    [SerializeField]
    int coalBurning;

    List<GameObject> coal = new List<GameObject>();

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        foreach (GameObject o in coal)
        {
            o.transform.localScale -= Vector3.one * scaleStep;
            if (o.transform.localScale.x < 0)
            {
                o.transform.localPosition = Vector3.one * 0.3f;
                o.transform.localScale = Vector3.one * initScale;
                coal.Remove(o);
                coalBurning--;
            }
        }
        powerGen += coalBurning * powerStep - powerLoss;
        if (powerGen < 0) powerGen = 0;
        else if (powerGen > genMax) powerGen = genMax;
        power += powerGen;
        if (power < 0) power = 0;
        else if (power > powerMax) power = powerMax;

        furnaceLight.intensity = powerGen / genMax * lightMax;
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.tag != "Interactable") return;
        coal.Add(c.gameObject);
        coalBurning ++;
    }
}
