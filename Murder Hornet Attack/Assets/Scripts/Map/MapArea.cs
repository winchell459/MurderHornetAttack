using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapArea : MapChamber
{
    public List<MapChamber> chambers = new List<MapChamber>();
    public List<MapPath> paths = new List<MapPath>();

    public HoneycombTypes.Areas AreaType;

    public override bool Check(MapHoneycomb honeycomb)
    {
        bool display = honeycomb.display;
        foreach (MapChamber chamber in chambers)
        {
            //Debug.Log("MapNest.Check");
            if (!chamber.Check(honeycomb)) display = false;
        }

        foreach (MapPath path in paths)
        {
            if (!path.Check(honeycomb)) display = false;
        }
        //Debug.Log("MapNest.Check");

        return display;
    }
}
