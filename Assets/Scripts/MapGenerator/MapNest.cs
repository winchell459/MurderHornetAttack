using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNest : MapArea
{

    List<Circle> circles;

    public MapNest(Vector2 Location)
    {
        this.Location = Location;
        AreaSetup(HoneycombTypes.Areas.Nest);
    }

    

    public static MapNest CreateRandomNest(Vector2 pos, int nestCount, float radius, MapParameters mapParameters)
    {
        MapNest nest = new MapNest(pos);
        nest.locations.Add(pos);
        nest.widths.Add(radius * 2);
        float distributionAngle = 2 * Mathf.PI / nestCount;

        /*List<Circle>*/ nest.circles = new List<Circle>();
        for(int i = 0; i < nestCount; i+= 1)
        {
            Circle circle = new Circle();
            float minRadius = 0;
            float maxRadius = radius;
            do
            {
                float angle = i * distributionAngle - distributionAngle / 2;
                angle = Utility.Utility.Random(angle, angle + distributionAngle);
                float distance = Utility.Utility.Random(minRadius, maxRadius);
                float x = distance * Mathf.Cos(angle);
                float y = distance * Mathf.Sin(angle);
                Vector2 loc = pos + new Vector2(x, y);
                circle.r = radius / 2;
                circle.pos = loc;
                maxRadius += radius / 4;
            }
            while (Utility.Utility.ShapeOverlappped(circle, nest.circles));

            nest.circles.Add(circle);
        }

        foreach(Circle circle in nest.circles)
        {
           // GameObject.Instantiate(nestPrefab, circle.pos, Quaternion.identity);
            //Map.GetSpiderHole().transform.position = circle.pos;
            //nest.locations.Add(loc);

            MapChamber chamber = new MapChamber(circle.pos);
            chamber.VoidType = HoneycombTypes.Variety.Chamber;

            chamber.AddChamber(chamber.Location, radius);
            nest.chambers.Add(chamber);
            nest.AltPoints.Add(Utility.Honeycomb.WorldPointToHoneycombGrid(circle.pos,mapParameters));
        }

        

        return nest;
    }

    public override void Setup()
    {
        GameObject nestPrefab = MapManager.singleton.SpiderHole;
        foreach (Circle circle in circles)
        {
            GameObject.Instantiate(nestPrefab, circle.pos, Quaternion.identity);
            
        }
    }




}
