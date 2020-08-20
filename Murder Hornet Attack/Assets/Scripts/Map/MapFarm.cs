using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapFarm : MapArea
{
    HoneycombPos startPoint;
    HoneycombPos endPoint;
    List<HoneycombPos> maze = new List<HoneycombPos>();
    
    public MapFarm(Vector2 Location)
    {
        this.Location = Location;
        AreaType = HoneycombTypes.Areas.Nest;
    }

    public static void CreateRandomMaze(HoneycombPos startPoint, HoneycombPos endPoint, float length, GameObject AntSquadTriggerPrefab)
    {
        MapFarm farm = new MapFarm(startPoint.vector2);
        farm.startPoint = startPoint;
        farm.endPoint = endPoint;

    }

    private void generateMaze()
    {
        maze.Add(startPoint);
        maze.Add(endPoint);
    }
    private void generateBranches()
    {
        for(int i = 0; i < maze.Count -1; i += 1)
        {
            //Vector2 start = (Utility.HoneycombGridToWorldPostion(endPoint) - Utility.HoneycombGridToWorldPostion(startPoint)).normalized * 
            //MapPath.CreateJoggingPath()
        }
    }

    
}
