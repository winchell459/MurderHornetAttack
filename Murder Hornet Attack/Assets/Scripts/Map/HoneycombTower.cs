using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoneycombTower : Honeycomb
{
    public Transform[] TowerLayers;
    private Transform[] layers;

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
        for(int i = 1; i < TowerLayers.Length; i+= 1)
        {
            TowerLayers[i].parent = layers[i];
            TowerLayers[i].localPosition = TowerLayers[0].localPosition * layers[i].localScale.x;
        }
    }
}
