using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoneycombCell : Honeycomb
{
    public GameObject Cap;
    public GameObject CapRing;
    private bool capped = true;
    private Transform Layer2;
    private float layer2Scale;


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
            int depth = honeyGrid.GetDepth();
            if (depth > 100)
            {

                if (depth > 7)
                {
                    Layer2 = Map.StaticMap.HoneycombLayers[4];
                    layer2Scale = Map.StaticMap.LayerScales[4];
                }
                else if (depth > 6)
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
                //SpriteRenderer sprite = Cap.GetComponent<SpriteRenderer>();
                //if (honeyGrid.isLargeLoc)
                //{
                //    sprite.color = Color.black;
                //    //Debug.Log("is black");
                //}
                //else sprite.color = Color.yellow;
                Cap.transform.localScale = new Vector3(1f, 1f, 1f);
                CapRing.SetActive(false);
                
            }
        }
        else if (Cap) Cap.SetActive(false);
    }

    public override void HideHoneycomb()
    {
        Cap.transform.parent = transform;
        Cap.transform.localPosition = Vector3.zero;
    }
}
