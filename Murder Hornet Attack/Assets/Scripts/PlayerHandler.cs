using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    public static int BeesMurderedCount;
    public static int HornetMurderedCount;
    public int MaxHealth = 50;
    public int MaxShot = 5;
    private int maxShotBuff;
    private float maxShotBuffTime;
    private float maxShotBuffStart;

    private HornetController player;

    private float lastPlasmaCharge = 0;
    public float PlasmaChargeRate = 1;
    private float plasmaChargeRateBuff;
    private float plasmaChargeRateBuffTime;
    private float plasmaChargeRateBuffStart;

    private float plasmaDamage = 1;
    private float plasmaDamageBuff;
    private float plasmaDamageBuffTime;
    private float plasmaDamageBuffStart;

    public void AddHealth(float Health)
    {
        if (player) player.AddedHealth(Health);
        else Debug.Log("Player missing for AddHealth()...");
    }
    public int GetMaxHealth()
    {
        return MaxHealth;
    }
    public int GetMaxShot()
    {
        if (maxShotBuffStart + maxShotBuffTime > Time.fixedTime) return maxShotBuff;
        else return MaxShot;
    }
    public void SetMaxShotBuff(int maxShotBuff, float maxShotBuffTime)
    {
        this.maxShotBuff = maxShotBuff;
        this.maxShotBuffTime = maxShotBuffTime;
        maxShotBuffStart = Time.fixedTime;
    }
    public float GetMaxShotBuffTime()
    {
        float remainder = maxShotBuffStart + maxShotBuffTime - Time.fixedTime;
        return Mathf.Clamp(remainder, 0, remainder);
    }
    public float GetPlasmaChargeRate()
    {
        if (plasmaChargeRateBuffStart + plasmaChargeRateBuffTime > Time.fixedTime) return plasmaChargeRateBuff;
        else return PlasmaChargeRate;
    }
    public void SetPlasmaChargeRateBuff(float chargeRate, float chargeRateTime)
    {
        plasmaChargeRateBuff = chargeRate;
        plasmaChargeRateBuffTime = chargeRateTime;
        plasmaChargeRateBuffStart = Time.fixedTime;
    }
    public float GetPlasmaChargeRateBuffTime()
    {
        float remainder = plasmaChargeRateBuffTime + plasmaChargeRateBuffStart - Time.fixedTime;
        return Mathf.Clamp(remainder, 0, remainder);
    }

    public float GetPlasmaPower()
    {
        if (plasmaDamageBuffStart + plasmaDamageBuffTime > Time.fixedTime) return plasmaDamageBuff;
        else return plasmaDamage;
    }
    public void SetPlasmaPowerBuff(float DamageBuff, float DamageBuffTime)
    {
        plasmaDamageBuff = DamageBuff;
        plasmaDamageBuffTime = DamageBuffTime;
        plasmaDamageBuffStart = Time.fixedTime;
    }
    public float GetPlasmaPowerBuffTime()
    {
        float remainder = plasmaDamageBuffStart + plasmaDamageBuffTime - Time.fixedTime;
        return Mathf.Clamp(remainder, 0, remainder);
    }

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
            if(player.ShotCount < GetMaxShot() && lastPlasmaCharge + GetPlasmaChargeRate() < Time.fixedTime)
            {
                lastPlasmaCharge = Time.fixedTime;
                player.ShotCount += 1;
                //FindObjectOfType<LevelHandler>().UpdatePlayerStats();
            }
            else if(player.ShotCount > GetMaxShot())
            {
                player.ShotCount = GetMaxShot();
                lastPlasmaCharge = Time.fixedTime;
            }
            else if(player.ShotCount == GetMaxShot())
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
