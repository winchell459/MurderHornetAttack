using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGarden : MapArea
{
    
    public MapGarden(Vector2 Location)
    {
        this.Location = Location;
        AreaSetup(HoneycombTypes.Areas.Garden);
    }

    public static MapGarden CreateRandomGarden(Vector2 pos, float radius)
    {
        MapGarden garden = new MapGarden(pos);
        MapChamber chamber = RandomChamber(pos, radius);

        garden.chambers.Add(chamber);
        for(int i = 0; i < chamber.locations.Count; i += 1)
        {
            garden.locations.Add(chamber.locations[i]);
            garden.widths.Add(chamber.widths[i]);
        }

        return garden;
    }

    public override void Setup()
    {
        Transform newPit = GameObject.Instantiate(MapManager.singleton.SnakePit);
        newPit.transform.position = chambers[0].Location;
    }
}
