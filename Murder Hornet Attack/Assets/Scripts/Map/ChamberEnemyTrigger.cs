using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChamberEnemyTrigger : ChamberTrigger
{
    public GameObject EnemyPrefab;
    private bool triggered;
    protected override void OnExit(Collider2D collision)
    {
        triggered = false;
    }

    protected override void OnStay(Collider2D collision)
    {
        if (!triggered)
        {
            Instantiate(EnemyPrefab, Chamber.locations[0], Quaternion.identity);
            triggered = true;
        }
    }

}
