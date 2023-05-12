using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    public static int BeesMurderedCount;
    public static int HornetMurderedCount;

    public static int eggCount = 0, flowerCount = 0, royalJellyCount = 0;

    public int _maxHealth = 50;
    public int MaxHealth { get { return _maxHealth + eggCount * 5; } }

    // ------- Capacity ---------
    public int _maxShot = 5;
    public int MaxShot { get { return _maxShot + eggCount * 2; } }
    private int maxShotBuff;
    private float maxShotBuffTime;
    private float maxShotBuffStart;

    private HornetController player;

    // ------- Charge Rate --------
    public float _plasmaChargeRate = 1;
    public float PlasmaChargeRate { get { return _plasmaChargeRate - 0.3f * flowerCount; } }
    private float lastPlasmaCharge = 0;
    private float plasmaChargeRateBuff;
    private float plasmaChargeRateBuffTime;
    private float plasmaChargeRateBuffStart;

    // ------- Power -----------
    public float _plasmaDamage = 1;
    private float plasmaDamage { get { return _plasmaDamage + 0.5f * royalJellyCount; } }
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
        if (maxShotBuffStart + maxShotBuffTime > Time.fixedTime) return maxShotBuff + MaxShot;
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
        if (plasmaChargeRateBuffStart + plasmaChargeRateBuffTime > Time.fixedTime) return plasmaChargeRateBuff * PlasmaChargeRate;
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
        if (plasmaDamageBuffStart + plasmaDamageBuffTime > Time.fixedTime) return plasmaDamageBuff + plasmaDamage;
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
        if (newBuff > 1 && currentBuff < newBuff) //replace currentBuff
        {
            float buffTotal = currentBuff * currentBuffRemaining;
            newBuffTime += buffTotal / newBuff;
        }else if (newBuff < 1 && newBuff < currentBuff)//replace currentBuff
        {
            float buffTotal = currentBuffRemaining / currentBuff;
            newBuffTime += buffTotal * newBuff;
        }
        else //add to currentBuffTime
        {
            if(newBuff < 1)
            {
                float buffTotal = newBuffTime / newBuff;
                newBuffTime = currentBuffRemaining + buffTotal * currentBuff;
            }
            else
            {
                float buffTotal = newBuff * newBuffTime;
                newBuffTime = currentBuffRemaining + buffTotal / currentBuff;
            }
            newBuff = currentBuff;
        }
    }
    public float GetPlasmaPowerBuffTime()
    {
        float remainder = plasmaDamageBuffStart + plasmaDamageBuffTime - Time.fixedTime;
        return Mathf.Clamp(remainder, 0, remainder);
    }

    public static void ResetBeesMurderedCount()
    {
        BeesMurderedCount = 0;
    }
    public static void ResetHornetMurderedCount()
    {
        HornetMurderedCount = 0;
    }
    public static void ResetStats()
    {
        ResetBeesMurderedCount();
        ResetHornetMurderedCount();
        eggCount = 0;
        flowerCount = 0;
        royalJellyCount = 0;
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
