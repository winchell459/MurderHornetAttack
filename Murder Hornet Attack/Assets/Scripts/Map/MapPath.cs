using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPath : MapVoid
{
    private List<Vector2> Points = new List<Vector2>();
    private List<float> widths = new List<float>();
    public int Count = 0;
    //public List<Vector2> End = new List<Vector2>();

    public MapPath(Vector2 start, Vector2 end, float width)
    {
        Points.Add(start);
        Points.Add(end);
        widths.Add(width);
        Count += 1;
        VoidType = MapHoneycomb.LocationTypes.Path;
    }
    private MapPath(Vector2 start)
    {
        Points.Add(start);
        VoidType = MapHoneycomb.LocationTypes.Path;
    }
    public void Add(Vector2 point, float width)
    {
        Points.Add(point);
        widths.Add(width);
        Count += 1;
    }
    public Vector2 Start(int pathIndex)
    {
        return Points[pathIndex];
    }
    public Vector2 End(int pathIndex)
    {
        return Points[pathIndex + 1];
    }

    public override bool Check(MapHoneycomb honeycomb)
    {
        bool display = honeycomb.display;
        for (int i = 0; i < Count; i++)
        {
            //if (display)
            {
                Vector2 start = Start(i);
                Vector2 end = End(i);
                float distance = Utility.PointDistanceToPath(honeycomb.position, start, end);
                if (distance < widths[i] / 2 || distance < 0.45f)
                {
                    if(honeycomb.LocationType <= MapHoneycomb.LocationTypes.Path) display = false;
                    honeycomb.isFloor = true;
                    SetLocationType(honeycomb, MapHoneycomb.LocationTypes.Path);
                }
                else
                {
                    CheckDepth(distance - widths[i] / 2, honeycomb, MapHoneycomb.LocationTypes.Path);
                //if(distance < widths[i] / 2 + Map.StaticMap.HorizontalSpacing*2)
                //{
                //    honeycomb.SetCapped(false);
                //}
                }


            }

        }
        return display;
    }

    public static MapPath CreateJoggingPath(Vector2 start, Vector2 end, float jogWidthMin, float jogWidthMax, float pathLengthMin, float pathLengthMax, float pathWidthMin, float pathWidthMax)
    {
        MapPath path = new MapPath(start);
        //float minX = Map.StaticMap.MapOrigin.x;
        //float maxX = Map.StaticMap.MapOrigin.x + Map.StaticMap.MapWidth;
        //float minY = Map.StaticMap.MapOrigin.y;
        //float maxY = Map.StaticMap.MapOrigin.y + Map.StaticMap.MapHeight;

        Vector2 direction = (end - start).normalized; //vector direction towards end point
        Vector2 normal = Vector2.Perpendicular(direction).normalized;//normal vector to direction vector for jogging path
        Vector2 current = start; //current jog point
        Vector2 linePoint = start; //closest point on line to current jog point

        while (path.Count < 1 || path.Points[path.Count -1] != end)
        {
            
            Vector2 forward = Random.Range(pathLengthMin, pathWidthMax) * direction;
            Vector2 jog = Random.Range(jogWidthMin, jogWidthMax) * normal;
            float width = Random.Range(pathWidthMin, pathWidthMax);
            float nextX = forward.x + jog.x + linePoint.x;
            float nextY = forward.y + jog.y + linePoint.y;
            linePoint = Utility.ClosestPointOnLine(new Vector2(nextX, nextY), start, end);
            if (Vector2.Distance(current, end) < pathLengthMin || linePoint == end)
            {
                nextX = end.x;
                nextY = end.y;
            }
            current = new Vector2(nextX, nextY);
            path.Add(current, width); 
        }
        return path;
    }
}
