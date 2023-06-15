using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpawn : MapChamber
{
    public MapSpawn(Vector2 Location)
    {
        this.Location = Location;
        ChamberSetup();
    }
    public override void Setup()
    {
        Portal PlayerSpawn = (Portal)ChamberTrigger.SetupChamberTrigger(MapManager.singleton.PortalPrefab, this, Color.clear);
        LevelHandler.singleton.PlayerSpawn = PlayerSpawn;
        Debug.Log("Player Spawn set");
    }
}
