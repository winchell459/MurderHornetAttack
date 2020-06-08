﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapChamber : MapVoid
{
    public List<Vector2> locations = new List<Vector2>();
    public List<float> widths = new List<float>();
    public Vector2 Location;//{ public get; private set; }

    public MapChamber(Vector2 Location)
    {
        this.Location = Location;
    }
    public override bool Check(MapHoneycomb honeycomb)
    {
        bool display = honeycomb.display;
        
        for(int i = 0; i < locations.Count; i++)
        {
            if (!display) break;
            if (Vector2.Distance(honeycomb.position, locations[i]) <= widths[i]/2) display = false;
            else
            {
                CheckDepth(Vector2.Distance(honeycomb.position, locations[i]) - widths[i] / 2, honeycomb);
                //if (Vector2.Distance(honeycomb.position, locations[i]) < widths[i] / 2 + Map.StaticMap.HorizontalSpacing * 2)
                //{
                //    honeycomb.SetCapped(false);
                //}
            }
           
        }
        return display;
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
        
        return RandomChamber(location, radius, voidCount);
    }

    public static MapChamber RandomChamber(Vector2 location, float radius, int voidCount)
    {
        MapChamber newChamber = new MapChamber(location);
        //newChamber.Location = location;
        for (int i = 0; i < voidCount; i++)
        {
            float alpha = Random.Range(0, 360);
            float h = Random.Range(radius / 4, radius); //may need to change the range do we don't get a void with zero width
            Vector2 loc = new Vector2(h * Mathf.Cos(alpha), h * Mathf.Sin(alpha));
            loc += location;
            float r = radius - Vector2.Distance(location, loc);
            newChamber.AddChamber(loc, r * 2);
            //Debug.Log("new chamber section: loc " + loc + " radius: " + r);


        }
        return newChamber;
    }

    //public static MapChamber RandomPortalChamber
}
