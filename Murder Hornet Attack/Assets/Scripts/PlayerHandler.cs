using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    public static int BeesMurderedCount;
    public static int HornetMurderedCount;
    public int MaxHealth = 50;
    public int MaxShot = 5;
    private HornetController player;

    private float lastPlasmaCharge = 0;
    public float PlasmaChargeRate = 1;

    public void ResetBeesMurderedCount()
    {
        BeesMurderedCount = 0;
    }
    public void ResetHornetMurderedCount()
    {
        HornetMurderedCount = 0;
    }
    public void ResetStats()
    {
        ResetBeesMurderedCount();
        ResetHornetMurderedCount();
    }

    private void Update()
    {
        if (player)
        {
            if(player.ShotCount < MaxShot && lastPlasmaCharge + PlasmaChargeRate < Time.fixedTime)
            {
                lastPlasmaCharge = Time.fixedTime;
                player.ShotCount += 1;
                FindObjectOfType<LevelHandler>().UpdatePlayerStats();
            }
            else if(player.ShotCount == MaxShot)
            {
                lastPlasmaCharge = Time.fixedTime;

            }
        }
        else
        {
            //if (FindObjectOfType<HornetController>())
            //Debug.Log("Looking for Player");
            player = FindObjectOfType<HornetController>();
        }
    }
}
