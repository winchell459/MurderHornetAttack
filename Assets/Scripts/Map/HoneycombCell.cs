using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoneycombCell : Honeycomb
{
    public GameObject Cap;
    public GameObject CapRing;
    public Collider2D EnemyTrigger;
    
    private bool capped = true;

    public GameObject HoneycombBase;

    public override void DestroyHoneycomb()
    {
        honeyGrid.DestroyHoneycomb();

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

    public override void DamageHoneycomb(float damage)
    {
        int depth = honeyGrid.GetDepth();
        if (!capped) DestroyHoneycomb();
        else if(damage > depth)
        {
            //depth -= 1;
            //honeyGrid.SetDepth(depth);
            //if (depth < 4) SetCapped(capped = false);
            if(depth > 2)
            {
                DamageAdjecentHoneycomb(1);
            }
            DestroyHoneycomb();
        }
    }

    public void SetCapColor(Color color)
    {
        Cap.GetComponent<SpriteRenderer>().color = color;
    }
    
}
