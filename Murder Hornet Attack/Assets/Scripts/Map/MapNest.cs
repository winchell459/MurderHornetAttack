using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNest : MapChamber
{
    List<MapChamber> chambers = new List<MapChamber>();
    List<MapPath> paths = new List<MapPath>();
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
            Debug.Log("MapNest.Check");
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
        float distributionAngle = Mathf.PI / nestCount;
        for(int i = 0; i < nestCount; i+= 1)
        {
            float angle = i * distributionAngle - distributionAngle / 2;
            angle = Random.Range(angle, angle + distributionAngle);
            float distance = Random.Range(radius, radius);
            float x = distance * Mathf.Cos(angle);
            float y = distance * Mathf.Sin(angle);
            Vector2 loc = pos + new Vector2(x, y);
            GameObject.Instantiate(nestPrefab, loc, Quaternion.identity);
            //nest.locations.Add(loc);
            
            MapChamber chamber = new MapChamber(loc);
            chamber.VoidType = MapHoneycomb.LocationTypes.Garden;
            chamber.AddChamber(chamber.Location, 10);
            nest.chambers.Add(chamber);
        }

        return nest;
    }
}
