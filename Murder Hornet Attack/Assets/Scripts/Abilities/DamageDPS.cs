using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDPS : MonoBehaviour
{
    public string BuffName = "untitled";
    public float Damage = 0f;
    public float Delay = 0f;
    public float ApplyDamageNTimes = 10f;
    public float ApplyEveryNSeconds = .1f;

    private int appliedTimes = 0;
    private Insect entity;

    public void BeginDPS()
    {
        entity = GetComponent<Insect>();
        if (entity != null)
        {
            StartCoroutine(Dps());
        }
        else
        {
            Destroy(this);
        }
    }

    IEnumerator Dps()
    {
        // Delay before first damage
        yield return new WaitForSeconds(Delay);


        while (appliedTimes < ApplyDamageNTimes)
        {
            print("Damaged!!");
            entity.TakeDamage(Damage);
            yield return new WaitForSeconds(ApplyEveryNSeconds);
            appliedTimes++;
            print("appliedTimes!");
        }

        Destroy(this);
    }

}
