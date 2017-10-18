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
    float maxElevatorPower;
    [SerializeField]
    float elevatorPower;

    [SerializeField]
    int blinkTimerMax;

    [SerializeField]
    Color blinkColor;
    int blinkTimer;
    bool blink;


    [SerializeField]
    float maxPower;
    [SerializeField]
    public float power;

    // Use this for initialization
    void Start () {
        ui = this;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if(elevatorPower >= maxElevatorPower)
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
            elevator.transform.localScale = Vector2.Lerp(minElevatorScale, maxElevatorScale, elevatorPower / maxElevatorPower);
            elevatorColor.color = Color.Lerp(minElevatorColor, maxElevatorColor, Mathf.Pow(elevatorPower / maxElevatorPower, 8));
        }

        powerBar.transform.localScale = Vector2.Lerp(minPowerScale, maxPowerScale, power / maxPower);
    }
}
