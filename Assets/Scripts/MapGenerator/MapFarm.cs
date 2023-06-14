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
        AreaType = HoneycombTypes.Areas.Farm;
    }

    //public static MapFarm CreateFlowerMaze(PerlineNoiseVoid noiseVoid, PerlinNoiseChamber startChamber, List<HoneycombPos> flowerPetals, PerlinNoiseChamber endChamber, GameObject AntSquadTriggerPrefab)
    //{

    //}

    public static MapFarm CreateRandomMazeWithPoints(HoneycombPos startPoint, HoneycombPos endPoint, float edgeWidth, HoneycombPos[] moundLocations/*, GameObject AntSquadTriggerPrefab*/)
    {
        MapFarm farm = new MapFarm(startPoint.worldPos);
        farm.locations.Add(farm.Location);
        farm.widths.Add(5);
        farm.startPoint = startPoint;
        farm.endPoint = endPoint;

        farm.maze.Add(startPoint);
        for(int i = 1; i < moundLocations.Length; i++)
        {
            farm.locations.Add(moundLocations[i].worldPos);
            farm.widths.Add(5);
            farm.maze.Add(moundLocations[i]);
        }
        

        farm.maze = farm.connectNodes(new List<HoneycombPos>(), startPoint);
        farm.maze.Add(endPoint);

        //connect ant mounds
        List<MapPath> edges = farm.generateEdges(farm.maze);



        SetupMounds(farm/*, AntSquadTriggerPrefab*/, edges);

        //finish farm setup
        farm.StartPoints.Add(farm.maze[0]);
        farm.EndPoints.Add(farm.maze[farm.maze.Count - 1]);

        return farm;
    }

    public static MapFarm CreateRandomMaze(HoneycombPos startPoint, HoneycombPos endPoint, float edgeWidth, int nodeCount/*, GameObject AntSquadTriggerPrefab*/)
    {
        MapFarm farm = new MapFarm(startPoint.worldPos);
        farm.locations.Add(farm.Location);
        farm.widths.Add(5);
        farm.startPoint = startPoint;
        farm.endPoint = endPoint;

        //place ant mounds
        farm.generateMaze(nodeCount);

        //connect ant mounds
        List<MapPath> edges = farm.generateEdges(farm.maze);

        

        SetupMounds(farm/*, AntSquadTriggerPrefab*/, edges);

        //finish farm setup
        farm.StartPoints.Add(farm.maze[0]);
        farm.EndPoints.Add(farm.maze[farm.maze.Count - 1]);
        //farm.paths = edges;

        return farm;
    }

    List<MapPath> edges;
    private static void SetupMounds(MapFarm farm, /*GameObject AntSquadTriggerPrefab,*/ List<MapPath> edges)
    {
        farm.edges = edges;
        //place triggers at each mound
        //List<ChamberAntFarmTrigger> triggers = new List<ChamberAntFarmTrigger>();
        //bool lastSet = false;
        foreach (HoneycombPos node in farm.maze)
        {
            MapChamber chamber = new MapChamber(node.worldPos);
            Debug.Log("node.pos " + node);
            chamber.VoidType = HoneycombTypes.Variety.Chamber;
            chamber.AddChamber(chamber.Location, 5);
            //ChamberAntFarmTrigger trigger = (ChamberAntFarmTrigger)ChamberTrigger.SetupChamberTrigger(AntSquadTriggerPrefab, chamber, Color.black);

            //triggers.Add(trigger);
            farm.chambers.Add(chamber);
        }

        
    }
    public override void Setup()
    {
        //place triggers at each mound
        List<ChamberAntFarmTrigger> triggers = new List<ChamberAntFarmTrigger>();
        //bool lastSet = false;
        foreach (HoneycombPos node in maze)
        {
            //MapChamber chamber = new MapChamber(node.worldPos);
            //Debug.Log("node.pos " + node);
            //chamber.VoidType = HoneycombTypes.Variety.Chamber;
            //chamber.AddChamber(chamber.Location, 5);
            ChamberAntFarmTrigger trigger = (ChamberAntFarmTrigger)ChamberTrigger.SetupChamberTrigger(MapManager.singleton.ChamberAntFarmTriggerPrefab.gameObject, chambers[0], Color.black);

            triggers.Add(trigger);
            //farm.chambers.Add(chamber);
        }
        //connect chamber ant farm triggers
        for (int i = 0; i < triggers.Count; i += 1)
        {
            if (i > 0) triggers[i].PreviousNode = triggers[i - 1];
            if (i < triggers.Count - 1) triggers[i].AntPath = edges[i];
        }
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
            if(!Utility.Utility.ShapeOverlappped(newNode, nodes))
            {
                nodes.Add(newNode);
                maze.Add(endPoint);

                maze[maze.Count - 2] = Utility.Honeycomb.WorldPointToHoneycombGrid(newNode.pos);
                
            }
            else
            {
                fails += 1;
                if (fails >= failsCoefficient * nodeCount) Debug.Log("Maze build fail: node.Count = " + nodes.Count);
            }
        }

        List<HoneycombPos> edges = new List<HoneycombPos>();
        maze = connectNodes(edges, startPoint);
    }

    private List<HoneycombPos> connectNodes(List<HoneycombPos> edges, HoneycombPos node)
    {
        List<HoneycombPos> newEdges = new List<HoneycombPos>();
        if (edges.Contains(node) || (edges.Count > 0 && checkIntersecting(node, edges))) return edges;
        else
        {
            foreach (HoneycombPos pos in edges)
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
                List<HoneycombPos> tempEdges = connectNodes(newEdges, maze[i]);
                if (tempEdges.Count == maze.Count)
                {
                    newEdges.Clear();
                    foreach (HoneycombPos pos in tempEdges)
                    {
                        newEdges.Add(pos);
                    }
                }
            }
            
        }
        return newEdges;
    }
    private void generateBranches()
    {
        for(int i = 0; i < maze.Count -1; i += 1)
        {
            //Vector2 start = (Utility.HoneycombGridToWorldPostion(endPoint) - Utility.HoneycombGridToWorldPostion(startPoint)).normalized * 
            //MapPath.CreateJoggingPath()
        }
    }

    private List<MapPath> generateEdges(List<HoneycombPos> nodes)
    {
        List<MapPath> edges = new List<MapPath>();
        for(int i = 1; i < nodes.Count; i += 1)
        {
            edges.Add(MapPath.CreateJoggingPath(nodes[i].worldPos, nodes[i - 1].worldPos, 1, 2, 2, 3, 2, 3));
        }
        return edges;
    }
    private bool checkIntersecting(HoneycombPos point, List<HoneycombPos> edges)
    {
        bool intersects = false;
        HoneycombPos point0 = edges[edges.Count - 1];
        for(int i = 1; i < edges.Count - 1; i += 1)
        {
            if(Utility.Utility.CheckIntersecting(point.worldPos,point0.worldPos, edges[i - 1].worldPos, edges[i].worldPos))
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
