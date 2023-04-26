using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Honeycomb : MonoBehaviour
{
    public MapHoneycomb honeyGrid;

    public HoneycombTypes.Variety LocationType;

    public abstract void DamageHoneycomb(float damage);
    public abstract void DestroyHoneycomb();
    public abstract void HideHoneycomb();

    public void DamageAdjecentHoneycomb(int depth)
    {
        HoneycombPos hexPos = Utility.Honeycomb.WorldPointToHoneycombGrid(honeyGrid.position);
        Map.StaticMap.GetHoneycomb(hexPos.GetAdjecentHoneycomb(0, 1)).DamageHoneycomb(depth);
        Map.StaticMap.GetHoneycomb(hexPos.GetAdjecentHoneycomb(1, 1)).DamageHoneycomb(depth);
        Map.StaticMap.GetHoneycomb(hexPos.GetAdjecentHoneycomb(1, -1)).DamageHoneycomb(depth);
        Map.StaticMap.GetHoneycomb(hexPos.GetAdjecentHoneycomb(0, -1)).DamageHoneycomb(depth);
        Map.StaticMap.GetHoneycomb(hexPos.GetAdjecentHoneycomb(-1, -1)).DamageHoneycomb(depth);
        Map.StaticMap.GetHoneycomb(hexPos.GetAdjecentHoneycomb(-1, 1)).DamageHoneycomb(depth);
    }
}
