using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoneycombCell : Honeycomb
{
    public GameObject Cap;
    public GameObject CapRing;
    public Collider2D EnemyTrigger;
    //public GameObject EnemyPrefab;
    private bool capped = true;
    //private Transform Layer2;
    //private float layer2Scale;


    public override void DestroyHoneycomb()
    {
        if (!capped) honeyGrid.DestroyHoneycomb();
        else
        {
            int depth = honeyGrid.GetDepth() - 1;
            honeyGrid.SetDepth(depth);
            if (depth < 4) SetCapped(capped = false);
        }

    }
    public void SetCapped(bool capped)
    {
        this.capped = capped;
        if (capped && Cap)
        {
            Cap.SetActive(true);
            
        }
        else if (Cap) Cap.SetActive(false);
    }

    public override void HideHoneycomb()
    {
        Cap.transform.parent = transform;
        Cap.transform.localPosition = Vector3.zero;
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.CompareTag("Player"))
    //    {
    //        Instantiate(honeyGrid.GetEnemyPrefab(), transform.position, Quaternion.identity);
    //        honeyGrid.DestroyHoneycomb();
    //    }
    //}
}
