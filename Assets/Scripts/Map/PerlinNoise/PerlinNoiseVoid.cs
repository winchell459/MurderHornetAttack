using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class PerlineNoiseVoid : MapVoid
{
    private PerlinNoise mapPerlinNoise;
    private int width, height;
    private int[,] depthMap;

    int[,] chamberIDMap;
    List<PerlinNoiseChamber> chambers = new List<PerlinNoiseChamber>();
    HexDepth[,] hexDepths;
    
    int pathWidth = 1; //pathWidth: hexDepth delta min must me less than pathWidth to be on path
    public bool generating = true;

    public PerlineNoiseVoid(PerlinNoise mapPerlinNoise, int width, int height)
    {
        generating = true;
        this.mapPerlinNoise = mapPerlinNoise;
        this.height = height;
        this.width = width;
        
        depthMap = mapPerlinNoise.GenerateDepthMap(width + 1, height + 1);
        hexDepths = new HexDepth[depthMap.GetLength(0), depthMap.GetLength(1)];
        FindChambers(depthMap);
        VoidType = HoneycombTypes.Variety.Path;

    }


    bool floorIsPath = true;
    public override bool Check(MapHoneycomb honeycomb)
    {
        HoneycombPos honeycombPos = Utility.Honeycomb.WorldPointToHoneycombGrid(honeycomb.position);

        int honeycombDepth = depthMap[honeycombPos.x, honeycombPos.y];
        if (honeycombDepth > 0)
        {
            if (honeycomb.GetDepth() > honeycombDepth) honeycomb.SetDepth(honeycombDepth);
            CheckDepth(honeycombDepth, honeycomb, VoidType);
            return true;
        }
        else
        {
            honeycomb.isFloor = true;
            
            float grayScale = chambers[chamberIDMap[honeycombPos.x, honeycombPos.y] - 1].GetChamberAreaMergedIndexHeatVal(honeycombPos.x, honeycombPos.y);
            honeycomb.color = new Color(grayScale,grayScale,grayScale);
            return false;
            //if (hexDepths[honeycombPos.x, honeycombPos.y] != null && (hexDepths[honeycombPos.x, honeycombPos.y].delta0 <= pathWidth || hexDepths[honeycombPos.x, honeycombPos.y].delta1 <= pathWidth || hexDepths[honeycombPos.x, honeycombPos.y].delta2 <= pathWidth))
            //{
                
            //    return floorIsPath;
            //}else if(hexDepths[honeycombPos.x, honeycombPos.y] == null)
            //{
            //    Debug.LogWarning($"hexDepths[{honeycombPos.x}, {honeycombPos.y}] is null");
            //}
            
            //return !floorIsPath;
        }

    }

    PerlinNoiseChamber maxChamber = null;
    List<PerlinNoiseArea> usedAreas = new List<PerlinNoiseArea>();
    public HoneycombPos GetAreaPos(int minRadius)
    {
        if(maxChamber == null)
        {
            maxChamber = chambers[0];
            for(int i = 1; i < chambers.Count; i += 1)
            {
                if (maxChamber.nodeCount < chambers[i].nodeCount) maxChamber = chambers[i];
            }
        }

        List<PerlinNoiseArea> chamberAreas = new List<PerlinNoiseArea> (maxChamber.GetChamberAreas());
        while(chamberAreas.Count > 0)
        {
            int index = Random.Range(0, chamberAreas.Count);
            PerlinNoiseArea parent = maxChamber.GetChamberArea(chamberAreas[index].pos).parentArea;
            if (!usedAreas.Contains(parent) && chamberAreas[index].maxRadius >= minRadius && chamberAreas[index].maxRadius <= minRadius + 4)
            {
                usedAreas.Add(parent);
                return chamberAreas[index].pos;
            }
            chamberAreas.RemoveAt(index);
        }
        return new HoneycombPos(-1, -1);
    }

    int[,] map;
    
    void FindChambers(int[,] map)
    {
        this.map = map;
        Thread findChamberThread = new Thread(FindChambersThread);
        findChamberThread.Start();
    }

    //float threshold = 0.5f;
    bool saveHeatMap = false;
    bool saveHexDepthMap = false;
    bool displayHexDepthMap = true;
    void FindChambersThread()
    {
        float startTime = Utility.Utility.GetTime();
        int width = map.GetLength(0);
        int height = map.GetLength(1);
        int[,] heatMap = new int[width, height];

        int to_visit_count = width * height;
        bool[,] visited = new bool[width, height];
        bool[,] in_frontier = new bool[width, height];

        int chamberID = 1;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (to_visit_count <= 0)
                {
                    Debug.LogWarning($"to_visit_count break at ({x},{y})");
                    break;
                }
                if (!visited[x, y] && map[x, y] <= 0)
                {
                    //find connected tiles
                    List<HoneycombPos> frontier = new List<HoneycombPos>();
                    frontier.Add(new HoneycombPos(x, y));
                    in_frontier[x, y] = true;

                    int chamberSize = 0;
                    List<HexDepth> nodes = new List<HexDepth>();
                    PerlinNoiseChamber noiseChamber = new PerlinNoiseChamber();
                    chambers.Add(noiseChamber);

                    while (frontier.Count > 0)
                    {
                        HoneycombPos cur = frontier[0];
                        frontier.RemoveAt(0);
                        in_frontier[cur.x, cur.y] = false;
                        heatMap[cur.x, cur.y] = chamberID;
                        visited[cur.x, cur.y] = true;
                        to_visit_count--;

                        chamberSize++;
                        HexDepth hexDepth = new HexDepth(noiseChamber, cur);
                        nodes.Add(hexDepth);
                        hexDepths[hexDepth.pos.x, hexDepth.pos.y] = hexDepth;

                        foreach (HoneycombPos neighbor in cur.GetAdjecentHoneycomb())
                        {
                            if (neighbor.x >= 0 && neighbor.y >= 0 && neighbor.x < heatMap.GetLength(0) && neighbor.y < heatMap.GetLength(1))
                            {
                                if (!visited[neighbor.x, neighbor.y] && map[neighbor.x, neighbor.y] <= 0)
                                {
                                    if (!in_frontier[neighbor.x, neighbor.y])
                                    {
                                        frontier.Add(neighbor);
                                        in_frontier[neighbor.x, neighbor.y] = true;
                                    }
                                }
                                else if (!visited[neighbor.x, neighbor.y] && map[neighbor.x, neighbor.y] > 0)
                                {
                                    visited[neighbor.x, neighbor.y] = true;
                                    to_visit_count--;
                                    if (map[neighbor.x, neighbor.y] > 0) heatMap[neighbor.x, neighbor.y] = 0;
                                }
                            }

                        }
                    }
                    noiseChamber.SetNodes(ref hexDepths,nodes);
                    chamberID++;
                    Debug.Log($"ChamberID: {chamberID - 1} size: {chamberSize}");

                }
                else if(!visited[x, y])
                {
                    visited[x, y] = true;
                    to_visit_count--;
                    if (map[x, y] > mapPerlinNoise.threshold) heatMap[x, y] = 0;
                }
            }
            if (to_visit_count <= 0)
            {
                
                break;
            }
        }
        chamberIDMap = heatMap;

        Debug.Log($"FindChamber depthmap {Utility.Utility.GetTime() - startTime} seconds.");
        startTime = Utility.Utility.GetTime();

        int maxRadius = 0;
        foreach(PerlinNoiseChamber chamber in chambers)
        {
            chamber.FindChamberAreas();
            maxRadius = Mathf.Max(maxRadius, chamber.GetMaxAreaRadius());
        }
        PerlinNoiseChamber.maxRadius = maxRadius;

        Debug.Log($"FindChamber FindChamberAreas took {Utility.Utility.GetTime() - startTime} seconds.");
        startTime = Utility.Utility.GetTime();

        

        //--------------------------------------- display hex depth map as texture -------------------------------
        if (displayHexDepthMap)
        {
            float[,] pathHexDepthMap = new float[hexDepths.GetLength(0), hexDepths.GetLength(1)];
            //float[,] pathHexMinDeltaMap = new float[hexDepths.GetLength(0), hexDepths.GetLength(1)];
            //float[,] chamberHexDepthMap = new float[hexDepths.GetLength(0), hexDepths.GetLength(1)];
            float[,] chamberHexMinDepthMap = new float[hexDepths.GetLength(0), hexDepths.GetLength(1)];
            float[,] sourceHexMap = new float[hexDepths.GetLength(0), hexDepths.GetLength(1)];
            float[,] maxSourceHexMap = new float[hexDepths.GetLength(0), hexDepths.GetLength(1)];
            float[,] chamberAreaMap = new float[hexDepths.GetLength(0), hexDepths.GetLength(1)];
            float[,] chamberPathRadiusMap = new float[hexDepths.GetLength(0), hexDepths.GetLength(1)];
            float[,] chamberRadiusAreaMap = new float[hexDepths.GetLength(0), hexDepths.GetLength(1)];
            float[,] chamberAreaIndexMap = new float[hexDepths.GetLength(0), hexDepths.GetLength(1)];
            for (int y = hexDepths.GetLength(1) - 1; y >= 0; y--)
            {
                for (int x = 0; x < hexDepths.GetLength(0); x++)
                {
                    //setup pathHexDepthMap
                    if (depthMap[x, y] == 0 && hexDepths[x, y] != null)
                    {
                        
                        if (hexDepths[x, y].delta0 <= pathWidth || hexDepths[x, y].delta1 <= pathWidth || hexDepths[x, y].delta2 <= pathWidth) pathHexDepthMap[x, y] = 0f;
                        else pathHexDepthMap[x, y] = 1;

                        

                    }
                    else if (depthMap[x, y] <= 2) pathHexDepthMap[x, y] = 0.25f;
                    else if (depthMap[x, y] < 5) pathHexDepthMap[x, y] = 0.5f;
                    else pathHexDepthMap[x, y] = 0;
                    //noiseMap[x, y] = ((float)depthMap[x, y])/8;

                    if(depthMap[x,y] == 0)
                    {
                        //if (hexDepths[x, y].GetMinPathDelta(pathWidth)) pathHexMinDeltaMap[x, y] = 0f;
                        //else pathHexMinDeltaMap[x, y] = 1f;

                        if (chamberIDMap[x, y] == 0) Debug.LogWarning($"chamberIDMap error: ({x},{y}) visited: {visited[x,y]} in_frontier: {in_frontier[x,y]}");
                        else
                        {
                            //chamberHexDepthMap[x, y] = chambers[chamberIDMap[x, y] - 1].GetChamberDepth(x, y);
                            chamberHexMinDepthMap[x, y] = chambers[chamberIDMap[x, y] - 1].GetChamberMinDepth(x, y);
                            if (chambers[chamberIDMap[x, y] - 1].GetSource(x, y)) sourceHexMap[x, y] = chambers[chamberIDMap[x, y] - 1].GetChamberMinDepth(x, y);
                            else sourceHexMap[x, y] = 0;
                            if (chambers[chamberIDMap[x, y] - 1].GetMaxSource(x, y)) maxSourceHexMap[x, y] = chambers[chamberIDMap[x, y] - 1].GetChamberMinDepth(x, y);
                            else maxSourceHexMap[x, y] = 0;
                            chamberAreaMap[x, y] = chambers[chamberIDMap[x, y] - 1].GetChamberIndexHeatVal(x, y);
                            chamberPathRadiusMap[x, y] = chambers[chamberIDMap[x, y] - 1].GetChamberPathRadiusHeatVal(x, y);
                            chamberRadiusAreaMap[x, y] = chambers[chamberIDMap[x, y] - 1].GetChamberAreaRadiusHeatVal(x, y);
                            chamberAreaIndexMap[x, y] = chambers[chamberIDMap[x, y] - 1].GetChamberAreaMergedIndexHeatVal(x, y);
                        }
                        
                    }
                    else
                    {
                        //pathHexMinDeltaMap[x,y] = pathHexDepthMap[x, y];
                        //chamberHexDepthMap[x, y] = pathHexDepthMap[x, y];
                        chamberHexMinDepthMap[x, y] = pathHexDepthMap[x, y];
                        sourceHexMap[x,y] = pathHexDepthMap[x, y];
                        maxSourceHexMap[x, y] = pathHexDepthMap[x, y];
                        chamberAreaMap[x,y] = pathHexDepthMap[x, y];
                        chamberPathRadiusMap[x, y] = pathHexDepthMap[x, y];
                        chamberRadiusAreaMap[x, y] = pathHexDepthMap[x, y];
                        chamberAreaIndexMap[x, y] = pathHexDepthMap[x, y];
                    }
                }
                
            }
            MiniMap.singleton.AddHeatMap(pathHexDepthMap);
            //MiniMap.singleton.AddHeatMap(pathHexMinDeltaMap);
            //MiniMap.singleton.AddHeatMap(chamberHexDepthMap);
            MiniMap.singleton.AddHeatMap(chamberHexMinDepthMap);
            MiniMap.singleton.AddHeatMap(sourceHexMap);
            MiniMap.singleton.AddHeatMap(maxSourceHexMap);
            MiniMap.singleton.AddHeatMap(chamberAreaMap);
            MiniMap.singleton.AddHeatMap(chamberPathRadiusMap);
            MiniMap.singleton.AddHeatMap(chamberRadiusAreaMap);
            MiniMap.singleton.AddHeatMap(chamberAreaIndexMap);
        }

        Debug.Log($"FindChamber display hexDepth Path took {Utility.Utility.GetTime() - startTime} seconds.");
        startTime = Utility.Utility.GetTime();

        //-------------------------------------- save hex depth map as string ------------------------------------
        if (saveHexDepthMap)
        {
            string hexDepthString = "";
            for (int y = hexDepths.GetLength(1)-1; y >= 0 ; y--)
            {
                for (int x = 0; x < hexDepths.GetLength(0); x++)
                {
                    //setup displayMap
                    if (depthMap[x, y] == 0 && hexDepths[x, y] != null)
                    {
                        if (hexDepths[x, y].delta0 <= pathWidth || hexDepths[x, y].delta1 <= pathWidth || hexDepths[x, y].delta2 <= pathWidth) hexDepthString += "P";
                        else hexDepthString += "1";
                    }
                    else hexDepthString += "X";
                    //noiseMap[x, y] = ((float)depthMap[x, y])/8;
                }
                hexDepthString += "\n";
            }
            Utility.Save.CreateFile(hexDepthString);
        }

        Debug.Log($"FindChamber hexDepths string took {Utility.Utility.GetTime() - startTime} seconds.");
        startTime = Utility.Utility.GetTime();

        //------------------------------------- save chamberID map as string ----------------------------------------
        if (saveHeatMap)
        {
            string heatMapString = "";
            //print chamber ids to file
            for (int y = height - 1; y >= 0; y--)
            {

                for (int x = 0; x < width; x++)
                {
                    if (heatMap[x, y] < 10)
                        heatMapString += heatMap[x, y];
                    else
                        heatMapString += (char)((int)'A' + heatMap[x, y] - 10);
                }
                heatMapString += "\n";
            }
            Debug.Log($"FindChamber heatmap string {Utility.Utility.GetTime() - startTime} seconds.");
            startTime = Utility.Utility.GetTime();
            Utility.Save.CreateFile(heatMapString);
            Debug.Log($"FindChamber saving {Utility.Utility.GetTime() - startTime} seconds.");
        }

        generating = false;
    }
}

