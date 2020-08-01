using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBuff : MonoBehaviour
{
    public string BuffName = "untitled";
    public float SpeedMuliplier = 1f;
    public bool Unique;

    public float Delay = 0f;
    public float Duration = 10f;


    private HornetController entity;

    public void Start()
    {
        // check
        if (Unique) ;
    }

    public void BeginBuff()
    {
        if (Unique)
        {
            print("Unique");
            SpeedBuff[] buffs = GetComponents<SpeedBuff>();
            for (int i = 0; i < buffs.Length; i++)
            {

                if (buffs[i].BuffName == BuffName && buffs[i] != this){
                    print("Not Unique");
                    Destroy(this);
                    return;
                }
            }
        }

        entity = GetComponent<HornetController>();
        if (entity != null)
        {
            StartCoroutine(Buff());
        }
        else
        {
            Destroy(this);
        }
    }




    /**
     * The best way, probably, to make this is to have a static function that
     * reapplies the buff of each "SpeedBuff" attached to a game object while
     * not resseting the their timer. This should happen each time a buff
     * expires or a new one is added.
     */

    IEnumerator Buff()
    {
        // Delay before first damage
        yield return new WaitForSeconds(Delay);

        entity.ForwardSpeed *= SpeedMuliplier;
        entity.SideSpeed *= SpeedMuliplier;

        yield return new WaitForSeconds(Duration);

        entity.ResetSpeed();

        Destroy(this);
    }
}
