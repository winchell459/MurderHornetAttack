using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapChamber : MapVoid
{
    private List<Vector2> locations = new List<Vector2>();
    private List<float> widths = new List<float>();

    public override bool Check(MapHoneycomb honeycomb)
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

    public Vector2 ClosestEntrancePoint(Vector2 point)
    {
        float distance = Mathf.Infinity;
        int index = 0;
        for(int i = 0; i < locations.Count; i+= 1)
        {
            float distanceCheck = Vector2.Distance(locations[i], point) - widths[i]/2;
            if(distanceCheck < distance)
            {
                distance = distanceCheck;
                index = i;
            }
        }
        return distance * (locations[index] - point).normalized + point;
    }

    public static MapChamber RandomChamber(Vector2 location, float radius)
    {
        int voidCount = Random.Range(2, (int)radius);
        MapChamber newChamber = new MapChamber();
        for(int i = 0; i < voidCount; i++)
        {
            float alpha = Random.Range(0, 360);
            float h = Random.Range(radius/6, radius); //may need to change the range do we don't get a void with zero width
            Vector2 loc = new Vector2(h * Mathf.Cos(alpha), h * Mathf.Sin(alpha));
            loc += location;
            float r = radius - Vector2.Distance(location, loc);
            newChamber.AddChamber(loc, r * 2);
            Debug.Log("new chamber section: loc " + loc + " radius: " + r);
        }
        return newChamber;
    }
}
