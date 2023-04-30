using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseChamber /*: MapChamber*/
{
    List<HexDepth> nodes;
    HexDepth[,] hexDepths;
    int maxDepth = int.MinValue, minDepth = int.MinValue;
    List<PerlinNoiseArea> chamberAreas = new List<PerlinNoiseArea>();

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
        return (float)hexDepths[x, y].maxDepth / maxDepth;
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
    public float GetChamberHeatVal(int x, int y)
    {
        PerlinNoiseArea area = GetChamberArea(new HoneycombPos(x, y));
        int maxRadius = MaxAreaRadius() + 1;
        if (area == null) return 0;
        else
        {
            float areaIndex = chamberAreas.IndexOf(area) + 1;
            return areaIndex / chamberAreas.Count;

        }
    }
    public static int maxRadius = -1; 
    public float GetChamberAreaRadiusVal(int x, int y)
    {
        PerlinNoiseArea area = GetChamberArea(new HoneycombPos(x, y));
        int maxRadius = PerlinNoiseChamber.maxRadius <= 0? MaxAreaRadius() + 1: PerlinNoiseChamber.maxRadius;
    
        if (area == null) return 0;
        else
        {
            //float areaIndex = chamberAreas.IndexOf(area)+1;
            //return areaIndex / chamberAreas.Count;
            float areaMaxRadius = area.maxRadius + 1;
            return areaMaxRadius / maxRadius;
        }
    }
    public PerlinNoiseArea GetChamberArea(HexDepth hex) { return GetChamberArea(hex.pos); }
    public PerlinNoiseArea GetChamberArea(HoneycombPos pos)
    {
        foreach(PerlinNoiseArea area in chamberAreas)
        {
            if (area.HasHex(pos)) return area;
        }
        return null;
    }
    public void FindChamberAreas()
    {
        SetDepths();
        FindSourceHexDepthRadius();

        foreach(HexDepth hex in nodes)
        {
            if (hex.maxSource && hex.maxRadius > 0)
            {
                PerlinNoiseArea myArea = GetChamberArea(hex);
                if(myArea == null)
                {
                    //check for overlapping areas
                    myArea = new PerlinNoiseArea(this, hex.GetHoneycombPosInRadius());
                    myArea.AddHex(hex);
                    chamberAreas.Add(myArea);
                }
                else
                {
                    foreach(HoneycombPos pos in hex.GetHoneycombPosInRadius())
                    {
                        PerlinNoiseArea posArea = GetChamberArea(pos);
                        if(posArea == null || posArea.maxRadius < myArea.maxRadius)
                        {
                            //remove pos from posArea
                            if(posArea != null) posArea.Remove(pos);
                            myArea.AddHex(pos);
                        }
                    }
                }
            }
        }

        Debug.Log($"ChamberArea Count: {chamberAreas.Count}");
    }
    
    public void SetDepths()
    {
        foreach (HexDepth hexDepth in nodes)
        {

            int depth = hexDepth.maxDepth;
            if (depth > maxDepth) maxDepth = depth;
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
            if (area.maxRadius > maxRadius) maxRadius = area.maxRadius;
        }
        return maxRadius;
    }
}

public class PerlinNoiseArea 
{
    public PerlinNoiseChamber myChamber;
    public int depth = 0;
    public int maxRadius = 0;
    List<HoneycombPos> chamberHex = new List<HoneycombPos>();
    public bool HasHex(HoneycombPos pos) { return chamberHex.Contains(pos); }
    public bool HasHex(HexDepth hex) { return chamberHex.Contains(hex.pos); }
    public void AddHex(HoneycombPos pos) { if (!HasHex(pos)) chamberHex.Add(pos); }
    public void AddHex(HexDepth hex) {
        if (!HasHex(hex)) {
            chamberHex.Add(hex.pos);
            if (hex.maxRadius > maxRadius) maxRadius = hex.maxRadius;
        }
    }
    public void Remove(HoneycombPos pos) { chamberHex.Remove(pos); }
    public PerlinNoiseArea(PerlinNoiseChamber myChamber)
    {
        this.myChamber = myChamber;
        chamberHex = new List<HoneycombPos>();
    }
    public PerlinNoiseArea(PerlinNoiseChamber myChamber, List<HoneycombPos> chamberHex)
    {
        this.myChamber = myChamber;
        this.chamberHex = chamberHex;
    }
}
