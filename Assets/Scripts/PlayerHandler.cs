using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    public static int BeesMurderedCount;
    public static int HornetMurderedCount;

    public int eggCount = 0, flowerCount = 0, royalJellyCount = 0;
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

    public bool AddHealth(float Health)
    {
        if (player)
        {
            if(player.Health < MaxHealth)
            {
                player.AddedHealth(Health);
                return true;
            }
            
        }
        else Debug.Log("Player missing for AddHealth()...");
        return false;
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
    public bool SetMaxShotBuff(float maxShotBuff, float maxShotBuffTime)
    {
        float remainingBuffTime = Mathf.Max(0, this.maxShotBuffTime + maxShotBuffStart - Time.time);
        if (remainingBuffTime > 0)
        {
            CompareBuffs(ref maxShotBuff, ref maxShotBuffTime, this.maxShotBuff, remainingBuffTime);

        }
        this.maxShotBuff = (int)maxShotBuff;
        this.maxShotBuffTime = maxShotBuffTime;
        maxShotBuffStart = Time.fixedTime;
        return true;
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
    public bool SetPlasmaChargeRateBuff(float chargeRate, float chargeRateTime)
    {
        float remainingBuffTime = Mathf.Max(0, plasmaChargeRateBuffTime + plasmaChargeRateBuffStart - Time.time);
        if (remainingBuffTime > 0)
        {
            CompareBuffs(ref chargeRate, ref chargeRateTime, plasmaChargeRateBuff, remainingBuffTime);

        }
        plasmaChargeRateBuff = chargeRate;
        plasmaChargeRateBuffTime = chargeRateTime;
        plasmaChargeRateBuffStart = Time.fixedTime;
        return true;
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
    public bool SetPlasmaPowerBuff(float DamageBuff, float DamageBuffTime)
    {
        float remainingBuffTime = Mathf.Max(0, plasmaDamageBuffTime + plasmaDamageBuffStart - Time.time);
        if(remainingBuffTime > 0)
        {
            CompareBuffs(ref DamageBuff, ref DamageBuffTime, plasmaDamageBuff, remainingBuffTime);
            
        }
        
        plasmaDamageBuff = DamageBuff;
        plasmaDamageBuffTime = DamageBuffTime;
        plasmaDamageBuffStart = Time.fixedTime;

        return true;
    }
    private void CompareBuffs(ref float newBuff, ref float newBuffTime, float currentBuff, float currentBuffRemaining)
    {
        if ((newBuff > 1 && currentBuff < newBuff) || (newBuff < 1 && newBuff < currentBuff))
        {
            float buffTotal = currentBuff * currentBuffRemaining;
            newBuffTime += buffTotal / newBuff;
        }
        else
        {
            float buffTotal = newBuff * newBuffTime;
            newBuffTime = currentBuffRemaining + buffTotal / newBuff;
            newBuff = currentBuff;
        }
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

    public void AddEggs(int count)
    {
        eggCount += count;
    }

    public void AddFlowers(int count)
    {
        flowerCount += count;
    }
    public void AddRoyalJelly(int count)
    {
        royalJellyCount += count;
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
