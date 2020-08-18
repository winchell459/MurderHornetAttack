using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNest : MapChamber
{
    public List<MapChamber> chambers = new List<MapChamber>();
    public List<MapPath> paths = new List<MapPath>();
    //public Vector2 Location;
     

    public MapNest(Vector2 Location)
    {
        this.Location = Location;
    }

    public override bool Check(MapHoneycomb honeycomb)
    {
        bool display = honeycomb.display;
        foreach(MapChamber chamber in chambers)
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

    public static MapNest CreateRandomNest(Vector2 pos, int nestCount, float radius, GameObject nestPrefab)
    {
        MapNest nest = new MapNest(pos);
        nest.locations.Add(pos);
        nest.widths.Add(radius * 2);
        float distributionAngle = 2 * Mathf.PI / nestCount;

        List<Circle> circles = new List<Circle>();
        for(int i = 0; i < nestCount; i+= 1)
        {
            Circle circle = new Circle();
            float minRadius = 0;
            float maxRadius = radius;
            do
            {
                float angle = i * distributionAngle - distributionAngle / 2;
                angle = Random.Range(angle, angle + distributionAngle);
                float distance = Random.Range(minRadius, maxRadius);
                float x = distance * Mathf.Cos(angle);
                float y = distance * Mathf.Sin(angle);
                Vector2 loc = pos + new Vector2(x, y);
                circle.r = radius / 2;
                circle.pos = loc;
                maxRadius += radius / 4;
            }
            while (ShapeOverlappped(circle, circles));

            circles.Add(circle);
        }

        foreach(Circle circle in circles)
        {
            GameObject.Instantiate(nestPrefab, circle.pos, Quaternion.identity);
            //nest.locations.Add(loc);

            MapChamber chamber = new MapChamber(circle.pos);
            chamber.VoidType = MapHoneycomb.LocationTypes.Garden;
            chamber.AddChamber(chamber.Location, radius);
            nest.chambers.Add(chamber);
        }

        return nest;
    }

    private struct Circle
    {
        public Vector2 pos;
        public float r;
    }

    private static bool ShapeOverlappped(Circle circle, List<Circle> circles)
    {
        bool overlap = false;
        foreach(Circle c in circles)
        {
            if (Vector2.Distance(c.pos, circle.pos) < c.r + circle.r) overlap = true;
        }
        return overlap;
    }
}
