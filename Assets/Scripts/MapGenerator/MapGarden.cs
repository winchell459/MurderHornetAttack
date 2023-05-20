using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGarden : MapArea
{
    public MapGarden(Vector2 Location)
    {
        this.Location = Location;
        AreaType = HoneycombTypes.Areas.Garden;
    }

    public static MapGarden CreateRandomGarden(Vector2 pos, float radius, GameObject ChamberTriggerPrefab)
    {
        MapGarden garden = new MapGarden(pos);

        MapChamber chamber = MapChamber.RandomChamber(pos, radius);
        ChamberTrigger.SetupChamberTrigger(ChamberTriggerPrefab, chamber);
        garden.chambers.Add(chamber);
        for(int i = 0; i < chamber.locations.Count; i += 1)
        {
            garden.locations.Add(chamber.locations[i]);
            garden.widths.Add(chamber.widths[i]);
        }

        return garden;
    }

    
}
