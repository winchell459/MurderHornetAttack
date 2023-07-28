using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Honeycomb : MonoBehaviour
{
    public MapHoneycomb mapHoneycomb;

    public abstract void SetupHoneycomb();
    public abstract void DamageHoneycomb(float damage);
    public virtual void DamageHoneycomb(float damage, HoneycombTypes.Areas newArea, HoneycombTypes.Variety newVariety)
    {
        DamageHoneycomb(damage);
        //Debug.Log($"New Honeycomb depth {mapHoneycomb.GetDepth()}");
        if (mapHoneycomb.GetDepth() <= Map.StaticMap.TunnelDestructionDepth)
        {
            mapHoneycomb.LocationType = newVariety;
            mapHoneycomb.isFloor = true;
            mapHoneycomb.AreaType = newArea;
            mapHoneycomb.display = true;

            mapHoneycomb.SetHasEnemy(false);

            mapHoneycomb.DisplayHoneycomb();
        }

    }
    public abstract void DestroyHoneycomb();
    public abstract void HideHoneycomb();

    
}
