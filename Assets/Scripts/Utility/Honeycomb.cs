using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public static class Honeycomb
    {
        //-------------------------------------------------Honeycomb.Utility------------------------------------------------------------
        public static Vector2 WorldPointToHoneycombPos(Vector2 point)
        {

            return HoneycombGridToWorldPostion(WorldPointToHoneycombGrid(point));
        }

        public static Vector2 HoneycombGridToWorldPostion(HoneycombPos honeyPos)
        {
            Map map = Map.StaticMap;
            float xPos = honeyPos.x * map.HorizontalSpacing;
            float yPos = yPos = honeyPos.y * map.VerticalSpacing * 2;
            if (honeyPos.x % 2 == 0) yPos += map.VerticalSpacing;

            return new Vector2(xPos, yPos) + new Vector2(map.MapOrigin.x * map.HorizontalSpacing, map.MapOrigin.y * map.VerticalSpacing);
        }

        public static HoneycombPos WorldPointToHoneycombGrid(Vector2 worldPos)
        {
            Map map = Map.StaticMap;
            int x = (int)((worldPos.x + map.HorizontalSpacing / 3) / map.HorizontalSpacing - map.MapOrigin.x);
            int y = (int)((worldPos.y + map.VerticalSpacing) / (2 * map.VerticalSpacing) - map.MapOrigin.y / 2);

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
            for (int i = xMin; i <= xMax; i += 1)
            {
                for (int j = yMin; j <= yMax; j += 1)
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
            int yChunk = (int)honeyIndex.y / (map.ChunkHeight / 2);
            //int chunkIndex = xChunk * yChunk + xChunk;
            return map.GetChunk(xChunk, yChunk);
        }
        public static List<MapHoneycomb> GetHoneycombPath(HoneycombPos start, HoneycombDir dir, int honeyDistance)
        {
            List<MapHoneycomb> path = new List<MapHoneycomb>();
            //start = WorldToHoneycomb(start);
            for (int i = 1; i <= honeyDistance; i += 1)
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
                end.y += (int)(Mathf.Sign(dir.y) * Mathf.Ceil((float)honeyDistance / 2));
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
            return Vector2.Distance(HoneycombGridToWorldPostion(hexOne), HoneycombGridToWorldPostion(hexTwo));
        }
    }
}

