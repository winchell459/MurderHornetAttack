using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoneycombCell : Honeycomb
{
    public GameObject Cap;
    public GameObject CapRing;
    public Collider2D EnemyTrigger;
    
    //private bool capped = true;

    public GameObject HoneycombBase;

    public override void DestroyHoneycomb()
    {
        mapHoneycomb.DestroyHoneycomb();
        mapHoneycomb.SetHasEnemy(false);
    }
    //public void SetCapped(bool capped)
    public override void SetupHoneycomb()
    {
        //this.capped = capped;
        if (mapHoneycomb.GetCapped() && Cap)
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
        int depth = mapHoneycomb.GetDepth();
        if (!mapHoneycomb.GetCapped()) DestroyHoneycomb();
        else if (damage > depth)
        {
            //depth -= 1;
            //honeyGrid.SetDepth(depth);
            //if (depth < 4) SetCapped(capped = false);
            if (depth > 2)
            {
                mapHoneycomb.DamageAdjecentHoneycomb(1);
            }
            DestroyHoneycomb();
        }
    }



    public void SetCapColor(Color color)
    {
        Cap.GetComponent<SpriteRenderer>().color = color;
    }
    
}
