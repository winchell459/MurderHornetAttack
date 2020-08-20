using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility 
{
    public static float GetTime()
    {
        return (System.DateTime.Now.Hour * 60 + System.DateTime.Now.Minute) * 60 + System.DateTime.Now.Second;
    }
    public static string FormatFloat(float value, int decimals)
    {
        
        int valueInt = (int)value;
        string valueStr = valueInt.ToString();
        if (decimals > 0) valueStr += ".";
        for(int i = 0; i < decimals; i += 1)
        {
            value -= valueInt;
            value *= 10;
            valueInt = (int)value;
            valueStr += valueInt.ToString();
        }
        return valueStr;
    }
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

    public static Vector2 WorldPointToHoneycombPos(Vector2 point)
    {
       
        return HoneycombGridToWorldPostion(WorldPointToHoneycombGrid(point));
    }

    public static Vector2 HoneycombGridToWorldPostion(HoneycombPos honeyPos)
    {
        Map map = Map.StaticMap;
        float xPos = honeyPos.x * map.HorizontalSpacing;
        float yPos = yPos = honeyPos.y * map.VerticalSpacing * 2;
        if (honeyPos.x % 2 == 0) yPos +=  map.VerticalSpacing;
        
        return new Vector2(xPos, yPos) + new Vector2(map.MapOrigin.x * map.HorizontalSpacing, map.MapOrigin.y * map.VerticalSpacing);
    }

    public static HoneycombPos WorldPointToHoneycombGrid(Vector2 worldPos)
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
                float check = Vector2.Distance(worldPos, HoneycombGridToWorldPostion(new HoneycombPos(i, j)));
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
        return new HoneycombPos(x, y);
    }


    public static MapChunk GetMapChunk(Vector2 worldPos)
    {
        HoneycombPos honeyIndex = WorldPointToHoneycombGrid(worldPos);
        Map map = Map.StaticMap;
        int xChunk = (int)honeyIndex.x / map.ChunkWidth;
        int yChunk = (int)honeyIndex.y / (map.ChunkHeight/2);
        //int chunkIndex = xChunk * yChunk + xChunk;
        return map.GetChunk(xChunk, yChunk);
    }
    public static List<MapHoneycomb> GetHoneycombPath(HoneycombPos start, HoneycombDir dir, int honeyDistance)
    {
        List<MapHoneycomb> path = new List<MapHoneycomb>();
        //start = WorldToHoneycomb(start);
        for(int i = 1; i <= honeyDistance; i += 1)
        {
            HoneycombPos honeyCell = GetHoneycombDirection(start, dir, i);
            //Debug.Log(honeyCell);
            path.Add(Map.StaticMap.GetHoneycomb((int)honeyCell.x, (int)honeyCell.y));
        }
        return path;
    }

    public static MapHoneycomb GetHoneycombFreePath(HoneycombPos startHex, HoneycombDir hexDir, int hexDistance)
    {
        List<MapHoneycomb> path = GetHoneycombPath(startHex, hexDir, hexDistance);
        MapHoneycomb newTarget = null;
        foreach (MapHoneycomb honeycomb in path)
        {
            //Debug.Log(honeycomb.position);
            if ((!honeycomb.display || honeycomb.isFloor) && honeycomb.LocationType == HoneycombTypes.Variety.Chamber)
            {
                newTarget = honeycomb;
                
            }
            else
            {
                //Debug.Log(honeycomb.LocationType);
                break;
            }
        }
        return newTarget;
    }
    
    /// <summary>
    /// Returns the coordinates of a target honeycomb starting from the coordinates of a honeycomb in a honeycomb vector (honeycomb direction and distance)
    /// </summary>
    /// <param name="start"></param>
    /// <param name="dir"></param>
    /// <param name="honeyDistance"></param>
    /// <returns></returns>
    public static HoneycombPos GetHoneycombDirection(HoneycombPos start, HoneycombDir dir, int honeyDistance)
    {
        //start = Utility.WorldPointToHoneycombGrid(start);
        HoneycombPos end = start;
        end.x += dir.x * honeyDistance;
        if (dir.x == 0) end.y += dir.y * honeyDistance;
        else if (start.x % 2 == 0 && dir.y > 0 || start.x % 2 != 0 && dir.y < 0)
        {
            //end.y += Mathf.Sign(dir.y) * Mathf.Ceil((float)honeyDistance / 2);
            end.y += (int)( Mathf.Sign(dir.y) * Mathf.Ceil((float)honeyDistance / 2));
        }
        else
        {
            //end.y += Mathf.Sign(dir.y) * Mathf.Ceil(((float)honeyDistance - 1) / 2);
            end.y += (int)(Mathf.Sign(dir.y) * Mathf.Ceil(((float)honeyDistance - 1) / 2));
        }

        return end;
    }

    public static HoneycombDir WorldDirToHoneycombDir(Vector2 worldDir)
    {
        HoneycombDir honeyDir = new HoneycombDir();
        if (worldDir.x > 0) honeyDir.x = 1;
        else if (worldDir.x < 0) honeyDir.x = 1;
        if (worldDir.y > 0) honeyDir.y = 1;
        else if (worldDir.y < 0) honeyDir.y = -1;
        return honeyDir;
    }

    public static float DistanceBetweenHoneycomb(HoneycombPos hexOne, HoneycombPos hexTwo)
    {
        return Vector2.Distance(HoneycombGridToWorldPostion( hexOne), HoneycombGridToWorldPostion( hexTwo));
    }
}
