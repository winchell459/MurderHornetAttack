using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexDepth
{
    PerlinNoiseChamber myChamber;
    public HoneycombPos pos;
    public int maxRadius = 0;

    //public static bool operator ==(HexDepth a, HexDepth b) { return a.pos == b.pos; }
    //public static bool operator !=(HexDepth a, HexDepth b) { return a.pos != b.pos; }
    public override bool Equals(object obj)
    {
        HexDepth other = obj as HexDepth;
        return other.pos.x == this.pos.x && other.pos.y == this.pos.y;
    }

    private int d0, d1, d2, d3, d4, d5; //depth values clockwise from directions 0,1 to -1,1
    public int depthSum { get { return GetDepth0() + GetDepth1() + GetDepth2() + GetDepth3() + GetDepth4() + GetDepth5(); } }
    //public int minDepth { get { return Mathf.Min(Mathf.Min(GetDepth0() + GetDepth1(), GetDepth2() + GetDepth3()),  GetDepth4() + GetDepth5()); } }
    public int minDepth
    {
        get
        {
            int[] parameters = { GetDepth0(), GetDepth1(), GetDepth2(), GetDepth3(), GetDepth4(), GetDepth5() };
            return Mathf.Min(parameters);
        }
    }

    // direction 1-1 = delta0, 1-0 = delta1, 1 1 = delta2
    public int delta0 { get { return GetDelta0(); } }
    public int delta1 { get { return GetDelta1(); } }
    public int delta2 { get { return GetDelta2(); } }

    public HexDepth hex0 = null, hex1 = null, hex2 = null, hex3 = null, hex4 = null, hex5 = null;
    private bool source0 = false, source1 = false, source2 = false, source3 = false, source4 = false, source5 = false;
    public bool source { get { return !source0 && !source1 && !source2 && !source3 && !source4 && !source5; } }

    public bool maxSource { get { return IsMaxSource(); } }

    private bool IsMaxSource()
    {
        if (!source) return false;
        foreach (HexDepth hex in hexInRadius)
        {
            if (hex.maxRadius > maxRadius) return false;
        }
        return true;
    }

    public HexDepth(PerlinNoiseChamber myChamber, HoneycombPos pos)
    {
        this.myChamber = myChamber;
        this.pos = pos;

    }

    private int GetDelta0()
    {
        //Debug.Log($"{GetDepth0()} - {GetDepth3()} = {Mathf.Abs(GetDepth0() - GetDepth3())}");
        return Mathf.Abs(GetDepth0() - GetDepth3());
    }
    private int GetDelta1()
    {
        return Mathf.Abs(GetDepth1() - GetDepth4());
    }
    private int GetDelta2()
    {
        return Mathf.Abs(GetDepth2() - GetDepth5());
    }

    public bool GetMinPathDelta(int deltaThreshold)
    {
        int d0 = GetDepth0();
        int d1 = GetDepth1();
        int d2 = GetDepth2();
        int d3 = GetDepth3();
        int d4 = GetDepth4();
        int d5 = GetDepth5();
        int[] depths = { d0, d1, d2, d3, d4, d5 };
        int maxDepth = Mathf.Max(depths);
        if (GetDelta0() < deltaThreshold && (d0 == maxDepth || d1 == maxDepth)) return true;
        else if (GetDelta1() <= deltaThreshold && (d2 == maxDepth || d3 == maxDepth)) return true;
        else if (GetDelta2() <= deltaThreshold && (d4 == maxDepth || d5 == maxDepth)) return true;
        else return false;
    }

    public int GetDepth0()
    {
        if (hex0 == null)
        {
            HoneycombPos neighborPos = pos.GetAdjecentHoneycomb(0, 1);
            hex0 = myChamber.GetNeighbor(neighborPos.x, neighborPos.y);
            if (hex0 == null)
            {
                d0 = 0;
                return d0;
            }
            else
            {
                d0 = hex0.GetDepth0() + 1;
                return d0;
            }
        }
        else return d0;

    }

    public int GetDepth1()
    {
        if (hex1 == null)
        {
            HoneycombPos neighborPos = pos.GetAdjecentHoneycomb(1, 1);
            hex1 = myChamber.GetNeighbor(neighborPos.x, neighborPos.y);
            if (hex1 == null)
            {
                d1 = 0;
                return 0;
            }
            else
            {
                d1 = hex1.GetDepth1() + 1;
                return d1;
            }
        }
        else return d1;

    }

    public int GetDepth2()
    {
        if (hex2 == null)
        {
            HoneycombPos neighborPos = pos.GetAdjecentHoneycomb(1, -1);
            hex2 = myChamber.GetNeighbor(neighborPos.x, neighborPos.y);
            if (hex2 == null)
            {
                d2 = 0;
                return 0;
            }
            else
            {
                d2 = hex2.GetDepth2() + 1;
                return d2;
            }
        }
        else return d2;

    }
    public int GetDepth3()
    {
        if (hex3 == null)
        {
            HoneycombPos neighborPos = pos.GetAdjecentHoneycomb(0, -1);
            hex3 = myChamber.GetNeighbor(neighborPos.x, neighborPos.y);
            if (hex3 == null)
            {
                d3 = 0;
                return 0;
            }
            else
            {
                d3 = hex3.GetDepth3() + 1;
                return d3;
            }
        }
        else return d3;

    }
    public int GetDepth4()
    {
        if (hex4 == null)
        {
            HoneycombPos neighborPos = pos.GetAdjecentHoneycomb(-1, -1);
            hex4 = myChamber.GetNeighbor(neighborPos.x, neighborPos.y);
            if (hex4 == null)
            {
                d4 = 0;
                return 0;
            }
            else
            {
                d4 = hex4.GetDepth4() + 1;
                return d4;
            }
        }
        else return d4;

    }
    public int GetDepth5()
    {
        if (hex5 == null)
        {
            HoneycombPos neighborPos = pos.GetAdjecentHoneycomb(-1, 1);
            hex5 = myChamber.GetNeighbor(neighborPos.x, neighborPos.y);
            if (hex5 == null)
            {
                d5 = 0;
                return 0;
            }
            else
            {
                d5 = hex5.GetDepth5() + 1;
                return d5;
            }
        }
        else return d5;

    }

    public bool GetSource0()
    {
        if (hex0 != null && hex0.minDepth > minDepth) source0 = true;
        return source0;
    }
    public bool GetSource1()
    {
        if (hex1 != null && hex1.minDepth > minDepth) source1 = true;
        return source1;
    }
    public bool GetSource2()
    {
        if (hex2 != null && hex2.minDepth > minDepth) source2 = true;
        return source2;
    }
    public bool GetSource3()
    {
        if (hex3 != null && hex3.minDepth > minDepth) source3 = true;
        return source3;
    }
    public bool GetSource4()
    {
        if (hex4 != null && hex4.minDepth > minDepth) source4 = true;
        return source4;
    }
    public bool GetSource5()
    {
        if (hex5 != null && hex5.minDepth > minDepth) source5 = true;
        return source5;
    }

    List<HexDepth> hexInRadius = new List<HexDepth>();
    public List<HoneycombPos> GetHoneycombPosInRadius()
    {
        List<HoneycombPos> hexPos = new List<HoneycombPos>();
        foreach (HexDepth hex in hexInRadius) hexPos.Add(hex.pos);
        return hexPos;
    }
    int radiusMax = 15;
    public int FindMaxRadius()
    {
        maxRadius = Mathf.Min(minDepth, radiusMax);
        List<HoneycombPos> neighbors = pos.GetAdjecentHoneycomb(maxRadius);
        if (neighbors.Count == 0)
        {
            maxRadius = 0;
            return maxRadius;
        }
        int count = 0;
        HexDepth neighbor = myChamber.GetNeighbor(neighbors[count].x, neighbors[count].y);
        while (count < neighbors.Count && neighbor != null)
        {
            hexInRadius.Add(neighbor);
            neighbor = myChamber.GetNeighbor(neighbors[count].x, neighbors[count].y);
            count += 1;
        }
        count -= 1;
        maxRadius = Utility.Honeycomb.GetHoneycombRadius(count);
        //Debug.Log($"{pos} has maxRadius {maxRadius}");
        return maxRadius;
    }
}
