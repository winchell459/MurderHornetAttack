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
        public static Vector2 WorldPointToHoneycombPos(Vector2 point, MapParameters map)
        {

            return HoneycombGridToWorldPostion(WorldPointToHoneycombGrid(point, map), map.HorizontalSpacing, map.VerticalSpacing, map.MapOrigin);
        }
        public static Vector2 HoneycombGridToWorldPostion(HoneycombPos honeyPos)
        {
            Map map = Map.StaticMap;
            return HoneycombGridToWorldPostion(honeyPos, map.HorizontalSpacing, map.VerticalSpacing, map.MapOrigin);
        }
        public static Vector2 HoneycombGridToWorldPostion(HoneycombPos honeyPos, MapParameters map)
        {
           
            return HoneycombGridToWorldPostion(honeyPos, map.HorizontalSpacing, map.VerticalSpacing, map.MapOrigin);
        }

        public static Vector2 HoneycombGridToWorldPostion(HoneycombPos honeyPos, float HorizontalSpacing, float VerticalSpacing, Vector2 MapOrigin)
        {
            
            float xPos = honeyPos.x * HorizontalSpacing;
            float yPos = yPos = honeyPos.y * VerticalSpacing * 2;
            if (honeyPos.x % 2 == 0) yPos += VerticalSpacing;

            return new Vector2(xPos, yPos) + new Vector2(MapOrigin.x * HorizontalSpacing, MapOrigin.y * VerticalSpacing);
        }
        public static HoneycombPos WorldPointToHoneycombGrid(Vector2 worldPos, MapParameters map)
        {
            return WorldPointToHoneycombGrid(worldPos, map.HorizontalSpacing, map.VerticalSpacing, map.MapOrigin);
        }
        public static HoneycombPos WorldPointToHoneycombGrid(Vector2 worldPos)
        {
            Map map = Map.StaticMap;
            return WorldPointToHoneycombGrid(worldPos, map.HorizontalSpacing, map.VerticalSpacing, map.MapOrigin);
        }
        public static HoneycombPos WorldPointToHoneycombGrid(Vector2 worldPos, float HorizontalSpacing, float VerticalSpacing, Vector2 MapOrigin)
        {
            
            int x = (int)((worldPos.x + HorizontalSpacing / 3) / HorizontalSpacing - MapOrigin.x);
            int y = (int)((worldPos.y + VerticalSpacing) / (2 * VerticalSpacing) - MapOrigin.y / 2);

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
                    float check = Vector2.Distance(worldPos, HoneycombGridToWorldPostion(new HoneycombPos(i, j), HorizontalSpacing, VerticalSpacing, MapOrigin));
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


        public static MapChunk GetMapChunk(Vector2 worldPos/*, MapParameters map*/)
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
       

        public static MapHoneycomb GetHoneycombFreePath(HoneycombPos startHex, HoneycombDir hexDir, int hexDistance, List<HoneycombPos> obstructions)
        {
            List<MapHoneycomb> path = GetHoneycombPath(startHex, hexDir, hexDistance);
            MapHoneycomb newTarget = null;
            foreach (MapHoneycomb honeycomb in path)
            {
                //Debug.Log(honeycomb.position);
                if ((!honeycomb.display || honeycomb.isFloor) && honeycomb.LocationType == HoneycombTypes.Variety.Chamber && !obstructions.Contains(WorldPointToHoneycombGrid(honeycomb.position)))
                {
                    newTarget = honeycomb;

                }
                else
                {
                    if (obstructions.Contains(WorldPointToHoneycombGrid(honeycomb.position))) Debug.Log("Collision with pillapillar avoided");
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
        /// <param name="honeyDistance">random distance to travel</param>
        /// <returns></returns>
        public static HoneycombPos GetHoneycombDirection(HoneycombPos start, HoneycombDir dir, int honeyDistance)
        {
            //start = Utility.WorldPointToHoneycombGrid(start);
            HoneycombPos end = start;
            end.x += dir.x * honeyDistance;
            if (dir.x == 0) end.y += dir.y * honeyDistance;
            else if (start.x % 2 == 0 && dir.y > 0 || start.x % 2 != 0 && dir.y < 0) //moving right
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
            else if (worldDir.x < 0) honeyDir.x = -1;
            if (worldDir.y > 0) honeyDir.y = 1;
            else if (worldDir.y < 0) honeyDir.y = -1;
            return honeyDir;
        }

        public static float DistanceBetweenHoneycomb(HoneycombPos hexOne, HoneycombPos hexTwo)
        {
            return Vector2.Distance(HoneycombGridToWorldPostion(hexOne), HoneycombGridToWorldPostion(hexTwo));
        }

        public static float DistanceBetweenHoneycomb(HoneycombPos hexOne, HoneycombPos hexTwo, MapParameters map)
        {
            return Vector2.Distance(HoneycombGridToWorldPostion(hexOne,map), HoneycombGridToWorldPostion(hexTwo,map));
        }

        public static int GetHoneycombRadius(int size)
        {
            int ringSize = 0;
            int totalSize = 0;
            int radius = 0;
            while(size >= totalSize)
            {
                radius += 1;
                ringSize += 6;
                if (size != totalSize)
                {
                    totalSize += ringSize;
                }
                else break;
                
            }
            
            //Debug.Log(totalSize);
            return radius - 1;
        }
    }
}

