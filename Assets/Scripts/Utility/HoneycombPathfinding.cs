
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public static class HoneycombPathfinding
    {
        //------------------------------------- path finding ---------------------------------------
        private static HoneycombDir[] Directions = {
        new HoneycombDir(0, 1),
        new HoneycombDir(1, 1),
        new HoneycombDir(1, -1),
        new HoneycombDir(0, -1),
        new HoneycombDir(-1, -1),
        new HoneycombDir(-1, 1)
    };

        public static List<MapHoneycomb> GetHoneycombPath(HoneycombPos start, HoneycombDir dir, int honeyDistance)
        {
            List<MapHoneycomb> path = new List<MapHoneycomb>();
            //start = WorldToHoneycomb(start);
            for (int i = 1; i <= honeyDistance; i += 1)
            {
                HoneycombPos honeyCell = GetHoneycombInDirection(start, dir, i);
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
                if ((!honeycomb.display || honeycomb.isFloor) && honeycomb.LocationType == HoneycombTypes.Variety.Chamber && !obstructions.Contains(Honeycomb.WorldPointToHoneycombGrid(honeycomb.position)))
                {
                    newTarget = honeycomb;

                }
                else
                {
                    if (obstructions.Contains(Honeycomb.WorldPointToHoneycombGrid(honeycomb.position))) Debug.Log("Collision with pillapillar avoided");
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
        public static HoneycombPos GetHoneycombInDirection(HoneycombPos start, HoneycombDir dir, int honeyDistance)
        {
            //start = Utility.WorldPointToHoneycombGrid(start);
            HoneycombPos end = new HoneycombPos(start.x, start.y);
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

        public static bool CheckValidNewTarget(Vector2 start, Vector2 end, HoneycombDir Direction)
        {
            if (end != Vector2.zero)
            {
                HoneycombDir newDirection = Honeycomb.WorldDirToHoneycombDir((end - start).normalized);
                return !CheckReverseDirection(newDirection, Direction);
            }
            else return false;

        }
        public static bool CheckReverseDirection(HoneycombDir newDirection, HoneycombDir Direction)
        {
            int newIndex = GetDirectionsIndex(newDirection);
            int curIndex = GetDirectionsIndex(Direction);
            Debug.Log($"newDirection: {newDirection} {newIndex} curDirection:{Direction} {curIndex}");
            return Mathf.Abs(newIndex - curIndex) == 3 || newIndex == 6;
        }

        private static int GetDirectionsIndex(HoneycombDir dir)
        {
            int index = 0;
            for (index = 0; index < Directions.Length; index += 1)
            {
                if (dir.x == Directions[index].x && dir.y == Directions[index].y)
                {
                    break;
                }
                if (index == Directions.Length - 1) Debug.LogWarning("Error finding new direction");
            }
            return index;
        }

        public static HoneycombDir GetNewDirection(HoneycombDir current, int turns) //turns + for counter clockwise
        {
            HoneycombDir newDir = new HoneycombDir(0, 0);
            int index = GetDirectionsIndex(current);

            if (turns > 0) index = (index + 1) % Directions.Length;
            else
            {
                index -= 1;
                if (index < 0) index = Directions.Length - 1;
            }

            newDir = Directions[index];

            turns = turns - (int)Mathf.Sign(turns);
            if (turns != 0) return GetNewDirection(newDir, turns);
            else
            {
                Debug.Log($"current: {current} newDir: {newDir}");
                return newDir;
            }
        }

        public static Vector2 FindPathToHoneycomb(HoneycombPos startHoneycomb, HoneycombPos targetHoneycomb, List<HoneycombPos> otherPos)
        {
            //Vector2 targetPos = Utility.HoneycombGridToWorldPostion(targetHoneycomb);
            //Vector2 startPos = Utility.HoneycombGridToWorldPostion(startHoneycomb);
            HoneycombPos closestHex = new HoneycombPos(startHoneycomb.x, startHoneycomb.y);

            if (startHoneycomb.x < targetHoneycomb.x && startHoneycomb.y > targetHoneycomb.y) //down and right
            {
                //(1,-1) || (0,-1)
                HoneycombDir dirOne = new HoneycombDir(1, 1);
                HoneycombDir dirTwo = new HoneycombDir(0, -1);
                closestHex = compareShortestPaths(dirOne, dirTwo, startHoneycomb, targetHoneycomb, otherPos);
            }
            else if (startHoneycomb.x < targetHoneycomb.x && startHoneycomb.y < targetHoneycomb.y) //right and up
            {
                //(1,1) || (0,1)
                HoneycombDir dirOne = new HoneycombDir(1, -1);
                HoneycombDir dirTwo = new HoneycombDir(0, 1);
                closestHex = compareShortestPaths(dirOne, dirTwo, startHoneycomb, targetHoneycomb, otherPos);

            }

            else if (startHoneycomb.x > targetHoneycomb.x && startHoneycomb.y > targetHoneycomb.y) // left and down
            {
                //(-1,-1) || (0,-1)
                HoneycombDir dirOne = new HoneycombDir(-1, 1);
                HoneycombDir dirTwo = new HoneycombDir(0, -1);
                closestHex = compareShortestPaths(dirOne, dirTwo, startHoneycomb, targetHoneycomb, otherPos);
            }
            else if (startHoneycomb.x > targetHoneycomb.x && startHoneycomb.y < targetHoneycomb.y) // left and up
            {
                //(-1,1) || (0,1)
                HoneycombDir dirOne = new HoneycombDir(-1, -1);
                HoneycombDir dirTwo = new HoneycombDir(0, 1);
                closestHex = compareShortestPaths(dirOne, dirTwo, startHoneycomb, targetHoneycomb, otherPos);
            }
            else if (startHoneycomb.x < targetHoneycomb.x && startHoneycomb.y == targetHoneycomb.y) //right and ( up 1 (x%2==0) or down 1 )
            {
                //(1,1) || (1,-1)
                HoneycombDir dirOne = new HoneycombDir(1, 1);
                HoneycombDir dirTwo = new HoneycombDir(1, -1);
                closestHex = compareShortestPaths(dirOne, dirTwo, startHoneycomb, targetHoneycomb, otherPos);
            }
            else if (startHoneycomb.x > targetHoneycomb.x && startHoneycomb.y == targetHoneycomb.y) // left and (down 1 x%2==0 or up 1)
            {
                //(-1,1) || (-1,-1)
                HoneycombDir dirOne = new HoneycombDir(-1, 1);
                HoneycombDir dirTwo = new HoneycombDir(-1, -1);
                closestHex = compareShortestPaths(dirOne, dirTwo, startHoneycomb, targetHoneycomb, otherPos);
            }
            //directly up
            else if (startHoneycomb.x == targetHoneycomb.x && startHoneycomb.y < targetHoneycomb.y) // up
            {
                // (0,1)
                HoneycombPos hexOne = FindShortestPath(startHoneycomb, new HoneycombDir(0, 1), targetHoneycomb, otherPos);
                if (Honeycomb.DistanceBetweenHoneycomb(closestHex, targetHoneycomb) > Honeycomb.DistanceBetweenHoneycomb(hexOne, targetHoneycomb)) closestHex = hexOne;
            }
            //directly below
            else if (startHoneycomb.x == targetHoneycomb.x && startHoneycomb.y > targetHoneycomb.y) // down
            {
                // (0,-1)
                HoneycombPos hexOne = FindShortestPath(startHoneycomb, new HoneycombDir(0, -1), targetHoneycomb, otherPos);
                if (Honeycomb.DistanceBetweenHoneycomb(closestHex, targetHoneycomb) > Honeycomb.DistanceBetweenHoneycomb(hexOne, targetHoneycomb)) closestHex = hexOne;
            }
            else
            {
                //(0,0)
                Debug.Log("Snake Follow Player Error");
            }
            if (closestHex.isNull) return Honeycomb.HoneycombGridToWorldPostion(startHoneycomb);
            return Honeycomb.HoneycombGridToWorldPostion(closestHex);
        }

        private static HoneycombPos compareShortestPaths(HoneycombDir dirOne, HoneycombDir dirTwo, HoneycombPos startHoneycomb, HoneycombPos targetHoneycomb, List<HoneycombPos> otherPos)
        {
            //Vector2 targetPos = Utility.HoneycombGridToWorldPostion(targetHoneycomb);
            //Vector2 startPos = Utility.HoneycombGridToWorldPostion(startHoneycomb);
            HoneycombPos closestHex = startHoneycomb;

            HoneycombPos hexOne = FindShortestPath(startHoneycomb, dirOne, targetHoneycomb, otherPos);
            HoneycombPos hexTwo = FindShortestPath(startHoneycomb, dirTwo, targetHoneycomb, otherPos);

            if (!hexOne.isNull && Honeycomb.DistanceBetweenHoneycomb(closestHex, targetHoneycomb) > Honeycomb.DistanceBetweenHoneycomb(hexOne, targetHoneycomb)) closestHex = hexOne;
            if (!hexTwo.isNull && Honeycomb.DistanceBetweenHoneycomb(closestHex, targetHoneycomb) > Honeycomb.DistanceBetweenHoneycomb(hexTwo, targetHoneycomb)) closestHex = hexTwo;
            return closestHex;
        }

        private static HoneycombPos FindShortestPath(HoneycombPos startHex, HoneycombDir hexDir, HoneycombPos targetHex, List<HoneycombPos> otherPos)
        {
            int distance = 1;
            HoneycombPos pathHex = startHex;
            //List<HoneycombPos> otherPos = GetPillapillarsPos();
            MapHoneycomb newTarget = GetHoneycombFreePath(startHex, hexDir, distance, otherPos);
            MapHoneycomb nextTarget = GetHoneycombFreePath(startHex, hexDir, distance + 1, otherPos);
            while (newTarget != null && nextTarget != null &&
                Honeycomb.DistanceBetweenHoneycomb(Honeycomb.WorldPointToHoneycombGrid(newTarget.position), targetHex)
                > Honeycomb.DistanceBetweenHoneycomb(Honeycomb.WorldPointToHoneycombGrid(nextTarget.position), targetHex))
            {
                newTarget = nextTarget;
                distance += 1;
                nextTarget = GetHoneycombFreePath(startHex, hexDir, distance + 1, otherPos);
            }
            //Debug.Log("Closets HexPos: " + Utility.WorldPointToHoneycombGrid(newTarget.position));
            if (newTarget == null) return HoneycombPos.nullHex;
            return Honeycomb.WorldPointToHoneycombGrid(newTarget.position);
        }
    }
}
