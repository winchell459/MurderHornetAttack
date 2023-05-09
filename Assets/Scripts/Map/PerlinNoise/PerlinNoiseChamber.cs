using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseChamber /*: MapChamber*/
{
    List<HexDepth> nodes;
    public int nodeCount { get { return nodes.Count; } }
    HexDepth[,] hexDepths;
    int maxSumDepth = int.MinValue, minDepth = int.MinValue;
    List<PerlinNoiseArea> chamberAreas = new List<PerlinNoiseArea>();
    public List<PerlinNoiseArea> GetChamberAreas() { return chamberAreas; }

    int[,] chamberAreaID;
    List<int>[,] chamberAreaIDs;

    public void SetNodes(ref HexDepth[,] hexDepths, List<HexDepth> nodes)
    {
        this.hexDepths = hexDepths;
        this.nodes = nodes;
    }

    public HexDepth GetNeighbor(int x, int y)
    {
        if (x >= 0 && x < hexDepths.GetLength(0) && y >= 0 && y < hexDepths.GetLength(1)) return hexDepths[x, y];
        else return null;
    }

    
    public float GetChamberDepth(int x, int y)
    {
        return (float)hexDepths[x, y].depthSum / maxSumDepth;
    }
    public float GetChamberMinDepth(int x, int y)
    {
        return (float)hexDepths[x, y].minDepth / minDepth;
    }
    public bool GetSource(int x, int y)
    {
        return hexDepths[x, y].source;
    }
    public bool GetMaxSource(int x, int y)
    {
        return hexDepths[x, y].maxSource;
    }
    public float GetChamberPathRadiusHeatVal(int x, int y)
    {
        PerlinNoiseArea area = GetChamberArea(x, y);
        int maxRadius = PerlinNoiseChamber.maxRadius <= 0 ? MaxAreaRadius() + 1 : PerlinNoiseChamber.maxRadius;
        if (area == null || !area.HasHex(new HoneycombPos(x, y))) return 0;
        else
        {
            //float areaIndex = chamberAreas.IndexOf(area) + 1;
            return (float)area.maxRadius / maxRadius;

        }
    }
    public float GetChamberIndexHeatVal(int x, int y)
    {
        PerlinNoiseArea area = GetChamberArea(x, y);
       
        if (area == null || !area.HasHex(new HoneycombPos(x,y))) return 0;
        else
        {
            float areaIndex = chamberAreas.IndexOf(area) + 1;
            return areaIndex / chamberAreas.Count;

        }
    }
    public float GetChamberAreaMergedIndexHeatVal(int x, int y)
    {
        PerlinNoiseArea area = GetChamberArea(x, y);
        //int maxRadius = MaxAreaRadius() + 1;
        if (area == null) return 0;
        else
        {
            float areaIndex = chamberAreas.IndexOf(area.parentArea) + 1;
            return areaIndex / chamberAreas.Count;

        }
    }
    public static int maxRadius = -1; 
    public float GetChamberAreaRadiusHeatVal(int x, int y)
    {
        PerlinNoiseArea area = GetChamberArea(x, y);
        int maxRadius = PerlinNoiseChamber.maxRadius <= 0? MaxAreaRadius() + 1: PerlinNoiseChamber.maxRadius;
    
        if (area == null) return 0;
        else
        {
            //float areaIndex = chamberAreas.IndexOf(area)+1;
            //return areaIndex / chamberAreas.Count;
            float areaMaxRadius = area.maxParentRadius + 1;
            return areaMaxRadius / maxRadius;
        }
    }
    public PerlinNoiseArea GetChamberArea(HexDepth hex) { return GetChamberArea(hex.pos); }
    public PerlinNoiseArea GetChamberArea(HoneycombPos pos) { return GetChamberArea(pos.x, pos.y); }
    public PerlinNoiseArea GetChamberArea(int x, int y)
    {
        if (chamberAreaID[x, y] > 0)
        {
            return chamberAreas[chamberAreaID[x, y] - 1];
        }
        return null;
    }
    public void FindChamberAreas()
    {
        
        float startTime = Utility.Utility.GetTime();
        
        SetDepths();
        Debug.Log($"FindChamberAreas SetDepths() {Utility.Utility.GetTime() - startTime} seconds.");

        FindSourceHexDepthRadius();

        Debug.Log($"FindChamberAreas FindSourceHexDepthRadius() {Utility.Utility.GetTime() - startTime} seconds.");
        startTime = Utility.Utility.GetTime();

        chamberAreaID = new int[hexDepths.GetLength(0), hexDepths.GetLength(1)];
        chamberAreaIDs = new List<int>[hexDepths.GetLength(0), hexDepths.GetLength(1)];
        List<HexDepth> maxSourceOrdered = new List<HexDepth>();
        foreach (HexDepth hex in nodes)
        {
            int insertIndex = 0;
            if (hex.maxSource && hex.maxRadius > 0)
            {
                foreach (HexDepth maxSource in maxSourceOrdered)
                {
                    if (hex.maxRadius < maxSource.maxRadius) insertIndex++;
                    else break;
                }
                if (insertIndex < maxSourceOrdered.Count) maxSourceOrdered.Insert(insertIndex, hex);
                else maxSourceOrdered.Add(hex);
            }
        }

        int areaID = 0;
        foreach (HexDepth hex in maxSourceOrdered /*nodes*/)
        {
            if (hex.maxSource && hex.maxRadius > 0)
            {
                areaID += 1;

                List<HoneycombPos> honeycombPosInRadius = hex.GetHoneycombPosInRadius();
                PerlinNoiseArea myArea = new PerlinNoiseArea(this, areaID, hex.pos, honeycombPosInRadius);
                chamberAreas.Add(myArea);
                myArea.AddHex(hex);
                SetChamberAreaID(hex.pos, areaID, hex.maxRadius);
                
                foreach(HoneycombPos pos in honeycombPosInRadius)
                {
                    SetChamberAreaID(pos, areaID, hex.maxRadius);
                }

                
            }
        }

        Debug.Log($"ChamberArea Count: {chamberAreas.Count}");

        //---------------- merge chamber areas ---------------------------------------
        for(int i = 0; i < chamberAreas.Count; i+=1)
        {
            PerlinNoiseArea area = chamberAreas[i];
            int maxRadiusAreaIndex = i + 1;
            foreach(HoneycombPos pos in area.GetChamberHex())
            {
                //for(int j = 0; j < chamberAreas.Count; j++)
                foreach(int j in chamberAreaIDs[pos.x,pos.y])
                {
                    PerlinNoiseArea otherArea = chamberAreas[j-1];
                    if (otherArea != area && otherArea.HasHex(pos) && otherArea.maxParentRadius >= chamberAreas[maxRadiusAreaIndex-1].maxParentRadius)
                    {
                        maxRadiusAreaIndex = j/* + 1*/;
                    }
                }
            }

            if (maxRadiusAreaIndex != i + 1) area.parentArea = chamberAreas[maxRadiusAreaIndex - 1].GetParentArea();
        }

        List<int>[] mergedAreas = new List<int>[chamberAreas.Count];
        //find parents
        for(int i = 0; i < chamberAreas.Count; i += 1)
        {
            if(chamberAreas[i].parentArea == null || chamberAreas[i].parentArea == chamberAreas[i])
            {
                if (mergedAreas[i] == null) mergedAreas[i] = new List<int>();
                else Debug.LogWarning($"chamberID {i + 1} already created.");
                mergedAreas[i].Add(i + 1);
            }
        }

        //Add Children
        for (int i = 0; i < chamberAreas.Count; i += 1)
        {
            if (chamberAreas[i].parentArea != null && chamberAreas[i].parentArea != chamberAreas[i])
            {
                mergedAreas[chamberAreas[i].parentArea.areaID - 1].Add(i + 1);
            }
        }

        string mergedAreaString = "";
        foreach(List<int> areaList in mergedAreas)
        {
            if(areaList != null)
            {
                mergedAreaString += $"({areaList[0]} {chamberAreas[areaList[0]-1].maxRadius}) : ";
                for(int i = 1; i < areaList.Count; i += 1)
                {
                    mergedAreaString += $"({areaList[i]} {chamberAreas[areaList[i]-1].maxRadius}), ";
                }
                mergedAreaString += "\n";
            }
        }
        Debug.Log(mergedAreaString);

        // --------------------------- flood fill none areas with neighboring area ------------------------------
        for(int x = 0; x < chamberAreaID.GetLength(0); x += 1)
        {
            for(int y = 0; y < chamberAreaID.GetLength(1); y+= 1)
            {
                if(hexDepths[x,y] != null && chamberAreaID[x,y] == 0)
                {
                    FillHex(new HoneycombPos(x, y));
                }
            }
        }

        Debug.Log($"FindChamberAreas finised {Utility.Utility.GetTime() - startTime} seconds.");
        startTime = Utility.Utility.GetTime();
    }
    public static int[] randomHexSet= {0,1,2,3,4,5};
    public static System.Random random = new System.Random(0);
    public static void SetRandomHexSet()
    {
        for(int i = 0; i < 6; i++)
        {
            int index = random.Next(0, 6);
            int temp = randomHexSet[i];
            randomHexSet[i] = randomHexSet[index];
            randomHexSet[index] = temp;
        }
    }
    
    private void FillHex(HoneycombPos pos)
    {
        int areaID = -1;
        int emptyIndex = -1;
        List<HoneycombPos> neighbors = pos.GetAdjecentHoneycomb();
        SetRandomHexSet();

        for(int i = 0; i < neighbors.Count; i++)
        {
            HoneycombPos neighbor = neighbors[randomHexSet[i]];
            //Debug.Log(neighbor);
            if (areaID != -1 && emptyIndex != -1) break;
            if (areaID == -1 && chamberAreaID[neighbor.x, neighbor.y] != 0) areaID = chamberAreaID[neighbor.x, neighbor.y];
            else if (emptyIndex == -1 && hexDepths[neighbor.x,neighbor.y] != null && chamberAreaID[neighbor.x, neighbor.y] == 0) emptyIndex = i;
        }
        if (areaID != -1)
        {
            chamberAreaID[pos.x, pos.y] = areaID;
            if(emptyIndex != -1) FillHex(neighbors[emptyIndex]);
        }
        else
        {
            //Debug.Log($"missed hex {pos}");
        }
    }


    private void SetChamberAreaID(HoneycombPos pos, int areaID, int maxRadius)
    {
        int x = pos.x;
        int y = pos.y;
        if (chamberAreaIDs[x, y] == null) chamberAreaIDs[x, y] = new List<int>();
        if (!chamberAreaIDs[x, y].Contains(areaID)) chamberAreaIDs[x, y].Add(areaID);
        if (chamberAreaID[x, y] == 0 || chamberAreas[chamberAreaID[x,y]-1].maxParentRadius >= maxRadius)
        {
            chamberAreaID[x, y] = areaID;
        }
        
    }
    
    public void SetDepths()
    {
        foreach (HexDepth hexDepth in nodes)
        {

            int depth = hexDepth.depthSum;
            if (depth > maxSumDepth) maxSumDepth = depth;
            depth = hexDepth.minDepth;
            if (depth > minDepth) minDepth = depth;

            hexDepth.GetSource0();
            hexDepth.GetSource1();
            hexDepth.GetSource2();
            hexDepth.GetSource3();
            hexDepth.GetSource4();
            hexDepth.GetSource5();
        }

    }
    public void FindSourceHexDepthRadius()
    {
        int count = 1;
        foreach(HexDepth node in nodes)
        {
            if (node.source)
            {
                //Debug.Log($"{node.pos} {count} of {nodes.Count} has maxRadius {node.FindMaxRadius()}");
                node.FindMaxRadius();
            }
            count++;
        }
    }
    //finds the local maxRadius (non static)
    public int GetMaxAreaRadius() { return MaxAreaRadius(); }
    private int MaxAreaRadius()
    {
        int maxRadius = 0;
        foreach(PerlinNoiseArea area in chamberAreas)
        {
            if (area.maxParentRadius > maxRadius) maxRadius = area.maxParentRadius;
        }
        return maxRadius;
    }
}


