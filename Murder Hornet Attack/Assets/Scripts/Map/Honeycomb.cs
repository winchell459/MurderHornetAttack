using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Honeycomb : MonoBehaviour
{
    public MapHoneycomb honeyGrid;
    public GameObject Cap;
    public GameObject CapRing;
    private bool capped = true;
    private Transform Layer2;
    private float layer2Scale;
    

    public void DestroyHoneycomb()
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
            int depth = honeyGrid.GetDepth();
            if (depth > 4)
            {
                //Transform[] layers = Map.StaticMap.HoneycombLayers;
                //if (!Layer2)
                //{
                //    Layer2 = GameObject.Find("HoneycombLayer_2").transform;
                //    layer2Scale = Layer2.localScale.x;
                //}
                if(depth > 7)
                {
                    Layer2 = Map.StaticMap.HoneycombLayers[4];
                    layer2Scale = Map.StaticMap.LayerScales[4];
                }else if (depth > 6)
                {
                    Layer2 = Map.StaticMap.HoneycombLayers[3];
                    layer2Scale = Map.StaticMap.LayerScales[3];
                }
                else if (depth > 5)
                {
                    Layer2 = Map.StaticMap.HoneycombLayers[2];
                    layer2Scale = Map.StaticMap.LayerScales[2];
                
                }
                else 
                {
                    Layer2 = Map.StaticMap.HoneycombLayers[1];
                    layer2Scale = Map.StaticMap.LayerScales[1];
                }
                Cap.transform.parent = Layer2;
                Cap.transform.localPosition = transform.localPosition * layer2Scale;
                Cap.transform.localScale = new Vector3(layer2Scale, layer2Scale, 1f);
                CapRing.SetActive(true);
            }
            else
            {
                Cap.transform.localScale = new Vector3(1f, 1f, 1f);
                CapRing.SetActive(false);
            }
        }
        else if (Cap) Cap.SetActive(false);
    }

    public void HideHoneycomb()
    {
        Cap.transform.parent = transform;
        Cap.transform.localPosition = Vector3.zero;
    }

   
}
