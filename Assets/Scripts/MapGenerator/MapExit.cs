using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapExit : MapChamber
{
    public MapExit(Vector2 Location)
    {
        this.Location = Location;
        ChamberSetup();
    }
    public override void Setup()
    {
        LevelHandler.singleton.Exit = (Portal)ChamberTrigger.SetupChamberTrigger(MapManager.singleton.PortalPrefab, this, Color.clear);
    }
}
