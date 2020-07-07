using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility 
{
    /// <summary>
    /// returns the shortest distance from a point to a line
    /// </summary>
    /// <param name="point"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public static float PointDistanceToPath(Vector2 point, Vector2 start, Vector2 end)
    {
        float A = point.x - start.x;
        float B = point.y - start.y;
        float C = end.x - start.x;
        float D = end.y - start.y;

        float dot = A * C + B * D;
        float path = C * C + D * D;
        float check = -1;
        if (path != 0) check = dot / path;

        float xx, yy;

        if (check < 0)
        {
            xx = start.x;
            yy = start.y;
        }
        else if (check > 1)
        {
            xx = end.x;
            yy = end.y;
        }
        else
        {
            xx = start.x + check * C;
            yy = start.y + check * D;
        }
        float dx = point.x - xx;
        float dy = point.y - yy;
        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    /// <summary>
    /// returns the closets point on a line from another point not on at line
    /// </summary>
    /// <param name="point"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public static Vector2 ClosestPointOnLine(Vector2 point, Vector2 start, Vector2 end)
    {
        float A = point.x - start.x;
        float B = point.y - start.y;
        float C = end.x - start.x;
        float D = end.y - start.y;

        float dot = A * C + B * D;
        float path = C * C + D * D;
        float check = -1;
        if (path != 0) check = dot / path;

        Vector2 linePoint;
        if(check < 0)
        {
            linePoint = start;
        }else if(check > 1)
        {
            linePoint = end;
        }
        else
        {
            linePoint = start + (end - start) * check;
        }
        return linePoint;
    }

    public static Vector2 WorldPointToHoneycombGrid(Vector2 point)
    {
        Map map = Map.StaticMap;
        Vector2 gridPoint = point - map.MapOrigin;
        //gridPoint = new Vector2((int)gridPoint.x / map.HorizontalSpacing, (int)gridPoint.y / map.VerticalSpacing);
        float a = map.HorizontalSpacing / 3;//map.VerticalSpacing / Mathf.Sqrt(3);
        float xGrid = gridPoint.x + 2 * a;
        

        return gridPoint;
    }

    public static Vector2 HoneycombGridToWorldPostion(Vector2 honeyPos)
    {
        Map map = Map.StaticMap;
        float xPos = honeyPos.x * map.HorizontalSpacing;
        float yPos = yPos = honeyPos.y * map.VerticalSpacing * 2;
        if (honeyPos.x % 2 == 0) yPos +=  map.VerticalSpacing;
        
        return new Vector2(xPos, yPos) + new Vector2(map.MapOrigin.x * map.HorizontalSpacing, map.MapOrigin.y * map.VerticalSpacing);
    }

    public static Vector2 WorldToHoneycomb(Vector2 worldPos)
    {
        Map map = Map.StaticMap;
        int x = (int)((worldPos.x + map.HorizontalSpacing / 3) / map.HorizontalSpacing - map.MapOrigin.x );
        int y = (int)((worldPos.y + map.VerticalSpacing) / (2*map.VerticalSpacing) - map.MapOrigin.y/2 );

        List<Vector2> honeyCandidates = new List<Vector2>();
        int xMin = x;
        int xMax = x + 1;
        int yMin = y;
        int yMax = y + 1;
        //honeyCandidates.Add(HoneycombGridToWorldPostion(new Vector2(x, y)));
        if (x > 0) xMin -= 1;
        //if(x< map.Width) xMax += 1;
        if (y > 0) yMin -= 1;

        float distance = Mathf.Infinity;
        //string debugStr = "Checking Honeycomb:";
        for(int i = xMin; i <= xMax; i+=1)
        {
            for(int j = yMin; j <= yMax; j += 1)
            {
                //debugStr += " (" + i + ", " + j + ")";
                float check = Vector2.Distance(worldPos, HoneycombGridToWorldPostion(new Vector2(i, j)));
                if (check < distance)
                {
                    distance = check;
                    x = i;
                    y = j;
                }
            }
        }
        //debugStr += " Closest: ("  + x + ", " + y + ")";
        //Debug.Log(debugStr);
        return new Vector2(x, y);
    }

    public static MapChunk GetMapChunk(Vector2 worldPos)
    {
        Vector2 honeyIndex = WorldToHoneycomb(worldPos);
        Map map = Map.StaticMap;
        int xChunk = (int)honeyIndex.x / map.ChunkWidth;
        int yChunk = (int)honeyIndex.y / map.ChunkHeight;
        //int chunkIndex = xChunk * yChunk + xChunk;
        return map.GetChunk(xChunk, yChunk);
    }
}
