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

    public static MapFarm CreateRandomMaze(HoneycombPos startPoint, HoneycombPos endPoint, float edgeWidth, int nodeCount, GameObject AntSquadTriggerPrefab)
    {
        MapFarm farm = new MapFarm(startPoint.vector2);
        farm.locations.Add(farm.Location);
        farm.widths.Add(5);
        farm.startPoint = startPoint;
        farm.endPoint = endPoint;
        farm.generateMaze(nodeCount);
        foreach(HoneycombPos node in farm.maze)
        {
            MapChamber chamber = new MapChamber(node.worldPos);
            Debug.Log("node.pos " + node);
            chamber.VoidType = HoneycombTypes.Variety.Chamber;
            chamber.AddChamber(chamber.Location, 5);
            farm.chambers.Add(chamber);
        }

        

        return farm;
    }

    private void generateMaze(int nodeCount)
    {

        float edgeLength = 10;
        maze.Add(startPoint);
        maze.Add(endPoint);
        HoneycombPos minRangePoint = new HoneycombPos(startPoint.x - edgeLength -1 < endPoint.x - edgeLength - 1?  startPoint.x - (int)edgeLength - 1 :endPoint.x - (int)edgeLength -1 , startPoint.y - edgeLength - 1 < endPoint.y - edgeLength - 1 ? startPoint.y - (int)edgeLength - 1 : endPoint.y - (int)edgeLength - 1);
        HoneycombPos maxRangePoint = new HoneycombPos(startPoint.x + edgeLength + 1 > endPoint.x + edgeLength + 1 ? startPoint.x + (int)edgeLength + 1 : endPoint.x + (int)edgeLength + 1, startPoint.y + edgeLength + 1 > endPoint.y + edgeLength + 1 ? startPoint.y + (int)edgeLength + 1 : endPoint.y + (int)edgeLength + 1);

        Debug.Log(minRangePoint + " " + maxRangePoint);
        
        List<Circle> nodes = new List<Circle>();
        nodes.Add(new Circle(startPoint.worldPos, edgeLength));
        nodes.Add(new Circle(endPoint.worldPos, edgeLength));
        int fails = 0;
        int failsCoefficient = 50;
        while(nodes.Count < nodeCount && fails < failsCoefficient * nodeCount)
        {
            float randX = Random.Range(minRangePoint.worldPos.x, maxRangePoint.worldPos.x);
            float randY = Random.Range(minRangePoint.worldPos.y, maxRangePoint.worldPos.y);
            Circle newNode = new Circle(new Vector2(randX, randY),edgeLength/2);
            if(!Utility.ShapeOverlappped(newNode, nodes))
            {
                nodes.Add(newNode);
                maze.Add(endPoint);

                maze[maze.Count - 2] = Utility.WorldPointToHoneycombGrid(newNode.pos);
                
            }
            else
            {
                fails += 1;
                if (fails >= failsCoefficient * nodeCount) Debug.Log("Maze build fail: node.Count = " + nodes.Count);
            }
        }
        List<HoneycombPos> edges = new List<HoneycombPos>();
        //edges.Add(startPoint);
        maze = connectNodes(edges, startPoint);
    }

    private List<HoneycombPos> connectNodes(List<HoneycombPos> edges, HoneycombPos node)
    {
        if (edges.Contains(node) || (edges.Count > 0 && checkIntersecting(node, edges))) return edges;
        else edges.Add(node);
        for (int i = 1; i < maze.Count; i += 1)
        {
            Debug.Log(i + " edges.Count = " + edges.Count);
            if (edges.Count == maze.Count) break;
            if (!edges.Contains(maze[i])) edges = connectNodes(edges, maze[i]);
            
        }
        return edges;
    }
    private void generateBranches()
    {
        for(int i = 0; i < maze.Count -1; i += 1)
        {
            //Vector2 start = (Utility.HoneycombGridToWorldPostion(endPoint) - Utility.HoneycombGridToWorldPostion(startPoint)).normalized * 
            //MapPath.CreateJoggingPath()
        }
    }
    private bool checkIntersecting(HoneycombPos point, List<HoneycombPos> edges)
    {
        bool intersects = false;
        HoneycombPos point0 = edges[edges.Count - 1];
        for(int i = 1; i < edges.Count - 1; i += 1)
        {
            if(Utility.CheckIntersecting(point.worldPos,point0.worldPos, edges[i - 1].worldPos, edges[i].worldPos))
            {
                intersects = true;
            }
        }
        return intersects;
    }

    //struct edge {
    //    public HoneycombPos node1, node2;
    //}

}
