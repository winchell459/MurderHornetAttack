using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNest : MapArea
{
    
    

    public MapNest(Vector2 Location)
    {
        this.Location = Location;
        AreaType = HoneycombTypes.Areas.Nest;
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
            while (Utility.ShapeOverlappped(circle, circles));

            circles.Add(circle);
        }

        foreach(Circle circle in circles)
        {
            GameObject.Instantiate(nestPrefab, circle.pos, Quaternion.identity);
            //nest.locations.Add(loc);

            MapChamber chamber = new MapChamber(circle.pos);
            chamber.VoidType = HoneycombTypes.Variety.Chamber;

            chamber.AddChamber(chamber.Location, radius);
            nest.chambers.Add(chamber);
            nest.AltPoints.Add(Utility.WorldPointToHoneycombGrid(circle.pos));
        }

        

        return nest;
    }

    

    

    
}
