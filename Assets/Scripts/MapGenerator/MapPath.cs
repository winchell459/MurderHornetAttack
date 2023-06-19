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
        VoidType = HoneycombTypes.Variety.Path;
    }
    private MapPath(Vector2 start)
    {
        Points.Add(start);
        VoidType = HoneycombTypes.Variety.Path;
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
    public Vector2 GetPoint(int index)
    {
        return Points[index];
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
                float distance = Utility.Utility.PointDistanceToPath(honeycomb.position, start, end);
                if (distance < widths[i] / 2 || distance < 0.45f)
                {
                    if(honeycomb.LocationType <= VoidType) display = false;
                    honeycomb.isFloor = true;
                    SetLocationType(honeycomb, VoidType);
                }
                else
                {
                    CheckDepth(distance - widths[i] / 2, honeycomb, VoidType);
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
            
            Vector2 forward = Utility.Utility.Random(pathLengthMin, pathWidthMax) * direction;
            Vector2 jog = Utility.Utility.Random(jogWidthMin, jogWidthMax) * normal;
            float width = Utility.Utility.Random(pathWidthMin, pathWidthMax);
            float nextX = forward.x + jog.x + linePoint.x;
            float nextY = forward.y + jog.y + linePoint.y;
            linePoint = Utility.Utility.ClosestPointOnLine(new Vector2(nextX, nextY), start, end);
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

    //Vector2Int[] spiralOrder = {new Vector2Int()}
    static int[] spiralOrder = { 0, 300, 240, 180, 120, 60 }; //clockwise default hex spiral order
    public static MapPath GetHexSpiralPath(float pathWidth, float spiralWidth, Vector2 start, int startDir, Vector2 end)
    {
        
        int spiralStart = 0;
        while(spiralStart < 6 && MapPath.spiralOrder[spiralStart] != startDir)
        {
            spiralStart++;
        }

        if(spiralStart >= 6)
        {
            Debug.LogWarning($"MapPath GetHexSpiralPath start angle: {startDir} not found");
            return null;
        }

        int[] spiralOrder = new int[6];
        for(int i = 0; i < 6; i++)
        {
            spiralOrder[i] = MapPath.spiralOrder[(i + spiralStart) % 6];
        }

        MapPath hexMapPath = new MapPath(start);
        float radius = Vector2.Distance(start, end);
        int loopBreaker = 100;
        while(Vector2.Distance(start, end) > spiralWidth && loopBreaker > 0)
        {
            Vector2 newPoint;
            for (int i = 0; i < 4; i++)
            {
                newPoint = new Vector2(start.x + radius * Mathf.Cos(spiralOrder[i] * Mathf.PI / 180), start.y + radius * Mathf.Sin(spiralOrder[i] * Mathf.PI / 180));

                hexMapPath.Add(newPoint, pathWidth);
                start = newPoint;

                if (Vector2.Distance(start, end) <= spiralWidth) break;
            }
            if (Vector2.Distance(start, end) <= spiralWidth) break;
            radius -= spiralWidth;
            newPoint = new Vector2(start.x + radius * Mathf.Cos(spiralOrder[4] * Mathf.PI / 180), start.y + radius * Mathf.Sin(spiralOrder[4] * Mathf.PI / 180));

            hexMapPath.Add(newPoint, pathWidth);
            start = newPoint;

            if (Vector2.Distance(start, end) <= spiralWidth) break;
            radius += spiralWidth;
            newPoint = new Vector2(start.x + radius * Mathf.Cos(spiralOrder[5] * Mathf.PI / 180), start.y + radius * Mathf.Sin(spiralOrder[5] * Mathf.PI / 180));

            hexMapPath.Add(newPoint, pathWidth);
            start = newPoint;

            radius -= spiralWidth;

            loopBreaker--;
        }
        
        

        return hexMapPath;
    }

    
}
