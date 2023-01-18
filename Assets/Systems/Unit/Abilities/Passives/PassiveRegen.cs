using System.Collections;
using System.Collections.Generic;
using TwoBears.Unit;
using UnityEngine;

[RequireComponent(typeof(BaseUnit))]
public class PassiveRegen : MonoBehaviour
{
    [Header("Stats")]
    public int amount = 3;
    public float tickSpeed = 0.33f;

    //Components
    private BaseUnit unit;

    //Tick
    float timeToTick;

    //Mono
    private void Awake()
    {
        unit = GetComponent<BaseUnit>();
        timeToTick = 1.0f / tickSpeed;
    }
    private void Update()
    {
        //Unit must be alive
        if (unit.Health == 0) return;

        //Countdown tick
        timeToTick -= Time.deltaTime;

        if (timeToTick <= 0)
        {
            //Reset time to tick
            timeToTick = 1.0f / tickSpeed;

            //Heals unit by amount
            unit.RestoreHealth(amount);
        }
    }
}
