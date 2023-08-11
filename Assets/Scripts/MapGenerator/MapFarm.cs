using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapFarm : MapArea
{
    Vector2 startPoint;
    Vector2 endPoint;
    //List<HoneycombPos> maze = new List<HoneycombPos>();
    List<Vector2> maze = new List<Vector2>();

    public MapFarm(Vector2 Location)
    {
        this.Location = Location;
        AreaSetup(HoneycombTypes.Areas.Farm);
    }

    //public static MapFarm CreateFlowerMaze(PerlineNoiseVoid noiseVoid, PerlinNoiseChamber startChamber, List<HoneycombPos> flowerPetals, PerlinNoiseChamber endChamber, GameObject AntSquadTriggerPrefab)
    //{

    //}

    public static MapFarm CreateRandomMazeWithPoints(Vector2 worldPos, HoneycombPos startPoint,  HoneycombPos endPoint, float edgeWidth, HoneycombPos[] moundLocations, MapParameters mapParameters)
    {
        MapFarm farm = new MapFarm(worldPos);
        farm.locations.Add(farm.Location);
        farm.widths.Add(5);
        //farm.startPoint = startPoint;
        //farm.endPoint = endPoint;

        farm.startPoint = worldPos;
        farm.endPoint = Utility.Honeycomb.HoneycombGridToWorldPostion(endPoint, mapParameters);

        farm.maze.Add(worldPos);

        for(int i = 1; i < moundLocations.Length; i++)
        {
            
            farm.locations.Add(Utility.Honeycomb.HoneycombGridToWorldPostion(moundLocations[i], mapParameters));
            farm.widths.Add(5);
            //farm.maze.Add(moundLocations[i]);
            farm.maze.Add(Utility.Honeycomb.HoneycombGridToWorldPostion(moundLocations[i], mapParameters));
        }
        

        farm.maze = farm.connectNodes(new List<Vector2>(), farm.startPoint);
        //farm.maze.Add(endPoint);
        //farm.maze.Add(farm.endPoint);

        //connect ant mounds
        List<MapPath> edges = farm.generateEdges(farm.maze);

        SetupMounds(farm, edges);

        farm.StartPoints.Add(Utility.Honeycomb.WorldPointToHoneycombGrid( farm.maze[0],mapParameters));
        farm.EndPoints.Add(Utility.Honeycomb.WorldPointToHoneycombGrid(farm.maze[farm.maze.Count - 1], mapParameters));

        return farm;
    }

    //public static MapFarm CreateRandomMaze(HoneycombPos startPoint, HoneycombPos endPoint, float edgeWidth, int nodeCount, MapParameters mapParameters)
    //{
    //    MapFarm farm = new MapFarm(startPoint.worldPos);
    //    farm.locations.Add(farm.Location);
    //    farm.widths.Add(5);
    //    farm.startPoint = Utility.Honeycomb.HoneycombGridToWorldPostion(startPoint, mapParameters);
    //    farm.endPoint = Utility.Honeycomb.HoneycombGridToWorldPostion(endPoint, mapParameters);

    //    //place ant mounds
    //    farm.generateMaze(nodeCount);

    //    //connect ant mounds
    //    List<MapPath> edges = farm.generateEdges(farm.maze);

        

    //    SetupMounds(farm/*, AntSquadTriggerPrefab*/, edges);

    //    //finish farm setup
    //    farm.StartPoints.Add(Utility.Honeycomb.WorldPointToHoneycombGrid(farm.maze[0], mapParameters));
    //    farm.EndPoints.Add(Utility.Honeycomb.WorldPointToHoneycombGrid(farm.maze[farm.maze.Count - 1], mapParameters));
    //    //farm.paths = edges;

    //    return farm;
    //}

    List<MapPath> edges;
    private static void SetupMounds(MapFarm farm, /*GameObject AntSquadTriggerPrefab,*/ List<MapPath> edges)
    {
        farm.edges = edges;
        //place triggers at each mound
        foreach (Vector2 node in farm.maze)
        {
            MapChamber chamber = new MapChamber(node);
            //Debug.Log("node.pos " + node);
            chamber.VoidType = HoneycombTypes.Variety.Chamber;
            chamber.AddChamber(chamber.Location, 5);
            //ChamberAntFarmTrigger trigger = (ChamberAntFarmTrigger)ChamberTrigger.SetupChamberTrigger(AntSquadTriggerPrefab, chamber, Color.black);

            //triggers.Add(trigger);
            farm.chambers.Add(chamber);
        }

        
    }
    public override void Setup()
    {
        //MapManager.singleton.AntSquad.position = Location;

        //place triggers at each mound
        List<ChamberAntFarmTrigger> triggers = new List<ChamberAntFarmTrigger>();

        for(int i = 0; i < maze.Count; i++) {

            Debug.Log($"chamber {i} is at {chambers[i].Location}");
            ChamberAntFarmTrigger trigger = (ChamberAntFarmTrigger)ChamberTrigger.SetupChamberTrigger(MapManager.singleton.ChamberAntFarmTriggerPrefab.gameObject, chambers[i], Color.black);

            triggers.Add(trigger);
            
        }
        //connect chamber ant farm triggers
        for (int i = 0; i < triggers.Count; i += 1)
        {
            if (i > 0)
            {
                triggers[i].PreviousNode = triggers[i - 1];
            }
            //if(i < triggers.Count)
            //{
            //    triggers[i].NextNode = triggers[i + 1];
            //}
            if (i < triggers.Count - 1)
            {
                triggers[i].AntPath = edges[i];
                triggers[i].NextNode = triggers[i + 1];
            }
        }
    }

    //private void generateMaze(int nodeCount/*, int seed*/)
    //{
    //    //System.Random random = new System.Random(seed);
    //    float edgeLength = 10;
    //    maze.Add(startPoint);
    //    maze.Add(endPoint);
    //    Vector2 minRangePoint = new Vector2(startPoint.x - edgeLength -1 < endPoint.x - edgeLength - 1?  startPoint.x - (int)edgeLength - 1 :endPoint.x - (int)edgeLength -1 , startPoint.y - edgeLength - 1 < endPoint.y - edgeLength - 1 ? startPoint.y - (int)edgeLength - 1 : endPoint.y - (int)edgeLength - 1);
    //    Vector2 maxRangePoint = new Vector2(startPoint.x + edgeLength + 1 > endPoint.x + edgeLength + 1 ? startPoint.x + (int)edgeLength + 1 : endPoint.x + (int)edgeLength + 1, startPoint.y + edgeLength + 1 > endPoint.y + edgeLength + 1 ? startPoint.y + (int)edgeLength + 1 : endPoint.y + (int)edgeLength + 1);

    //    Debug.Log(minRangePoint + " " + maxRangePoint);
        
    //    List<Circle> nodes = new List<Circle>();
    //    nodes.Add(new Circle(startPoint, edgeLength));
    //    nodes.Add(new Circle(endPoint, edgeLength));
    //    int fails = 0;
    //    int failsCoefficient = 50;
    //    while(nodes.Count < nodeCount && fails < failsCoefficient * nodeCount)
    //    {
    //        float randX = Utility.Utility.Random(minRangePoint.x, maxRangePoint.x);
    //        float randY = Utility.Utility.Random(minRangePoint.y, maxRangePoint.y);
    //        Circle newNode = new Circle(new Vector2(randX, randY),edgeLength/2);
    //        if(!Utility.Utility.ShapeOverlappped(newNode, nodes))
    //        {
    //            nodes.Add(newNode);
    //            maze.Add(endPoint);

    //            maze[maze.Count - 2] = newNode.pos;
                
    //        }
    //        else
    //        {
    //            fails += 1;
    //            if (fails >= failsCoefficient * nodeCount) Debug.Log("Maze build fail: node.Count = " + nodes.Count);
    //        }
    //    }

    //    List<Vector2> edges = new List<Vector2>();
    //    maze = connectNodes(edges, startPoint);
    //}

    private List<Vector2> connectNodes(List<Vector2> edges, Vector2 node)
    {
        List<Vector2> newEdges = new List<Vector2>();
        if (edges.Contains(node) || (edges.Count > 0 && checkIntersecting(node, edges))) return edges;
        else
        {
            foreach (Vector2 pos in edges)
            {
                newEdges.Add(pos);

            }
            newEdges.Add(node);
        }
        for (int i = 1; i < maze.Count; i += 1)
        {
            //Debug.Log(i + " edges.Count = " + newEdges.Count);
            if (newEdges.Count == maze.Count) break;
            if (!newEdges.Contains(maze[i]))
            {
                List<Vector2> tempEdges = connectNodes(newEdges, maze[i]);
                if (tempEdges.Count == maze.Count)
                {
                    newEdges.Clear();
                    foreach (Vector2 pos in tempEdges)
                    {
                        newEdges.Add(pos);
                    }
                }
            }
            
        }
        return newEdges;
    }
    //private void generateBranches()
    //{
    //    for(int i = 0; i < maze.Count -1; i += 1)
    //    {
    //        //Vector2 start = (Utility.HoneycombGridToWorldPostion(endPoint) - Utility.HoneycombGridToWorldPostion(startPoint)).normalized * 
    //        //MapPath.CreateJoggingPath()
    //    }
    //}

    private List<MapPath> generateEdges(List<Vector2> nodes)
    {
        List<MapPath> edges = new List<MapPath>();
        for(int i = 1; i < nodes.Count; i += 1)
        {
            edges.Add(MapPath.CreateJoggingPath(nodes[i], nodes[i - 1], 1, 2, 2, 3, 2, 3));
        }
        return edges;
    }
    private bool checkIntersecting(Vector2 point, List<Vector2> edges)
    {
        bool intersects = false;
        Vector2 point0 = edges[edges.Count - 1];
        for(int i = 1; i < edges.Count - 1; i += 1)
        {
            if(Utility.Utility.CheckIntersecting(point,point0, edges[i - 1], edges[i]))
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
