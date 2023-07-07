using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBeeCity : MapVoid
{
    private List<MapPath> paths = new List<MapPath>();
    private Vector2 queenChamberPos;
    private List<HoneycombPos> queenChamber;

    public MapBeeCity(Vector2 start, Vector2 end, float pathWidth, float spiralWidth, int queenChamberRadius, MapParameters mapParameters)
    {
        queenChamberPos = end;
        MapPath pathToQueen = MapPath.GetHexSpiralPath(pathWidth, spiralWidth, start, 60, end);//new MapPath(start, end, 4);
        paths.Add(pathToQueen);

        queenChamber = Utility.Honeycomb.WorldPointToHoneycombGrid(end, mapParameters).GetAdjecentHoneycomb(queenChamberRadius);
    }
    public override bool Check(MapHoneycomb honeycomb)
    {
        HoneycombPos honeycombPos = Utility.Honeycomb.WorldPointToHoneycombGrid(honeycomb.position);

        //if (Vector2.Distance(honeycomb.position, queenChamberPos) < 15) return false;

        foreach(MapPath path in paths)
        {
            if (!path.Check(honeycomb)) return false;
        }

        if (queenChamber.Contains(honeycombPos))
        {
            Debug.Log($"MapBeeCity.Check: {honeycomb.AreaType}");
            if(honeycomb.AreaType == HoneycombTypes.Areas.Connection)
                honeycomb.AreaType = HoneycombTypes.Areas.City;
            honeycomb.isFloor = true;
        }

        return true;
    }

    public override void Setup()
    {
        LevelHandler.singleton.queen.transform.position = queenChamberPos;
        LevelHandler.singleton.ExitTunnel.gameObject.SetActive(false);
    }

}
