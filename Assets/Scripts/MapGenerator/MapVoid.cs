using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapVoid 
{
    public abstract bool Check(MapHoneycomb honeycomb);
    public virtual void Setup(/*ChamberTrigger triggerPrefab, GameObject otherPrefab*/) { }

    private const int locationDepthThreshold = 4;
    private List<MapHoneycomb> voidWalls = new List<MapHoneycomb>();
    public HoneycombTypes.Variety VoidType;
    //public HoneycombTypes.Areas AreaType;

    //public Vector2 Location;//{ public get; private set; }
    //public bool Connected;

    protected void CheckDepth(float distance, MapHoneycomb honeycomb, HoneycombTypes.Variety locationType)
    {
        int depth = (int)Mathf.Ceil(distance / Map.StaticMap.HorizontalSpacing);
        if (depth < honeycomb.GetDepth()) honeycomb.SetDepth(depth);

        if(depth <= locationDepthThreshold)
        {
            SetLocationType(honeycomb, locationType);
            
        }
        else
        {
            
        }
        
    }

    protected void SetLocationType(MapHoneycomb honeycomb, HoneycombTypes.Variety locationType)
    {
        if (locationType > honeycomb.LocationType)
        {
            honeycomb.LocationType = locationType;
            voidWalls.Add(honeycomb);
            //Debug.Log(voidWalls.Count);
        }
    }

    public List<MapHoneycomb> GetVoidWalls()
    {
        
        foreach(MapHoneycomb honeycomb in voidWalls)
        {
            if (honeycomb.LocationType != VoidType) voidWalls.Remove(honeycomb);
        }
        return voidWalls;
    }
    
}
