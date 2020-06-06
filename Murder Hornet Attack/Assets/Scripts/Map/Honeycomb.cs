using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Honeycomb : MonoBehaviour
{
    public MapHoneycomb honeyGrid;
    public GameObject Cap;
    private bool capped = true;
    

    public void DestroyHoneycomb()
    {
        if (!capped) honeyGrid.DestroyHoneycomb();
        else
        {
            int depth = honeyGrid.GetDepth() - 1;
            honeyGrid.SetDepth(depth);
            if(depth < 4) SetCapped(capped = false);
        }
        
    }
    public void SetCapped(bool capped)
    {
        this.capped = capped;
        if (capped && Cap) Cap.SetActive(true);
        else if (Cap) Cap.SetActive(false);
    }

   
}
