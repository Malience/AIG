using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScript : MonoBehaviour {
    public static UIScript ui;

    [SerializeField]
    GameObject elevator;
    [SerializeField]
    GameObject powerBar;
    [SerializeField]
    SpriteRenderer elevatorColor;

    [SerializeField]
    Vector2 minElevatorScale;
    [SerializeField]
    Vector2 maxElevatorScale;
    [SerializeField]
    Vector2 minPowerScale;
    [SerializeField]
    Vector2 maxPowerScale;

    [SerializeField]
    Color minElevatorColor;
    [SerializeField]
    Color maxElevatorColor;

    [SerializeField]
    float elevatorMax;
    [SerializeField]
    public float elevatorPower;

    [SerializeField]
    int blinkTimerMax;

    [SerializeField]
    Color blinkColor;
    int blinkTimer;
    bool blink;


    [SerializeField]
    float powerMax = 100000;
    [SerializeField]
    public float power;

    [SerializeField]
    public float powerStep;
    [SerializeField]
    public float powerLoss;

    [SerializeField]
    public float elevatorDrain;
    [SerializeField]
    public float elevatorCharge;

    [SerializeField]
    public float genMax;

    [SerializeField]
    public float powerGen;
    [SerializeField]
    public int coalBurning;

    [SerializeField]
    public bool lever;


    // Use this for initialization
    void Start () {
        ui = this;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        powerGen += coalBurning * powerStep - powerLoss;
        if (powerGen < 0) powerGen = 0;
        else if (powerGen > genMax) powerGen = genMax;
        power += powerGen;

        if (lever & power >= elevatorDrain & elevatorPower != elevatorMax)
        {
            power -= elevatorDrain;
            elevatorPower += elevatorCharge;
            if (elevatorPower > elevatorMax) elevatorPower = elevatorMax;
        }

        if (power < 0) power = 0;
        else if (power > powerMax) power = powerMax;





        elevator.transform.localScale = Vector2.Lerp(minElevatorScale, maxElevatorScale, elevatorPower / elevatorMax);
        if (elevatorPower >= elevatorMax)
        {
            blinkTimer++;
            if(blinkTimer >= blinkTimerMax)
            {
                blinkTimer = 0;
                blink = !blink;
                elevatorColor.color = blink? blinkColor : maxElevatorColor;
            }
        } else
        {
            elevatorColor.color = Color.Lerp(minElevatorColor, maxElevatorColor, Mathf.Pow(elevatorPower / elevatorMax, 8));
        }

        powerBar.transform.localScale = Vector2.Lerp(minPowerScale, maxPowerScale, power / powerMax);
    }
}
