using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoneycombTower : Honeycomb
{
    public Transform[] TowerLayers;
    private Transform[] layers;
    public bool HiveCastle = false;

    private void Start()
    {
        if (HiveCastle) SetupBeeTower();
    }
    public override void DamageHoneycomb(float damage)
    {
        throw new System.NotImplementedException();
    }

    public override void DestroyHoneycomb()
    {
        throw new System.NotImplementedException();
    }

    public override void HideHoneycomb()
    {
        for (int i = 1; i < TowerLayers.Length; i += 1)
        {
            TowerLayers[i].parent = TowerLayers[0];
            TowerLayers[i].localPosition = Vector2.zero;
        }
    }

    public void SetupBeeTower()
    {
        if(layers == null) layers = Map.StaticMap.HoneycombLayers;
        if(HiveCastle)
        {
            placeHoneycombLayers(0, transform);

        }
        else
        {
            for (int i = 0; i < TowerLayers.Length; i += 1)
            {
                TowerLayers[i].parent = layers[i];
                TowerLayers[i].localPosition = TowerLayers[i].localPosition * layers[i].localScale.x;
            }
        }
        
    }

    private void placeHoneycombLayers(int mapLayer, Transform honeycombLayer)
    {
        Debug.Log("Placing " + honeycombLayer.name);
        if(mapLayer > layers.Length - 1)
        {
            Debug.Log("Tower layers too high");
        }
        else
        {
            honeycombLayer.parent = layers[mapLayer];
            honeycombLayer.localPosition = honeycombLayer.localPosition * layers[mapLayer].localScale.x;
            for(int i = honeycombLayer.childCount - 1; i > -1 ; i -= 1)
            {
                placeHoneycombLayers(mapLayer + 1, honeycombLayer.GetChild(i));
            }
        }
    }
}
