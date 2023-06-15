using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPedal : MapVoid
{
    HoneycombPos hexPos;
    public MapPedal(HoneycombPos hexPos)
    {
        this.hexPos = hexPos;
        
    }
    public override bool Check(MapHoneycomb honeycomb)
    {
        return true;
    }

    public override void Setup()
    {
        if (MapManager.singleton.UnplacedFlowerPetals())
        {
            MapManager.singleton.GetFlowerPetalDrop().transform.position = Utility.Honeycomb.HoneycombGridToWorldPostion(hexPos);
            MiniMap.singleton.SetFlower(hexPos, true);
        }
        else
        {
            Debug.LogWarning("Not enough pedals.");
        }
    }
}
