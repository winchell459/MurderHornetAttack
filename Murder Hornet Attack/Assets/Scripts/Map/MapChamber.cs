using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapChamber : MapVoid
{
    private List<Vector2> locations = new List<Vector2>();
    private List<float> widths = new List<float>();

    public override bool Check(Honeycomb honeycomb)
    {
        //bool display = true;
        for(int i = 0; i < locations.Count; i++)
        {
            if (Vector2.Distance(honeycomb.position, locations[i]) <= widths[i]/2) honeycomb.display = false;
        }
        return honeycomb.display;
    }

    public void AddChamber(Vector2 location, float width)
    {
        locations.Add(location);
        widths.Add(width);
    }
}
