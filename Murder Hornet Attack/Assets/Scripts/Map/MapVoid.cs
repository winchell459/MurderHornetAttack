using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapVoid 
{
    public abstract bool Check(MapHoneycomb honeycomb);

    protected void CheckDepth(float distance, MapHoneycomb honeycomb)
    {
        int depth = (int)Mathf.Ceil(distance / Map.StaticMap.HorizontalSpacing);
        if (depth < honeycomb.GetDepth()) honeycomb.SetDepth(depth);
    }
}
