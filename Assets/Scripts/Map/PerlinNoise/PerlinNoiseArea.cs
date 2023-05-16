using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseArea
{
    public HoneycombPos pos;
    public int areaID;
    public PerlinNoiseArea parentArea
    {
        get { return GetParentArea(); }
        set { _parentArea = value; }
    }
    private PerlinNoiseArea _parentArea = null;
    public PerlinNoiseArea GetParentArea()
    {
        if (_parentArea == null || _parentArea == this) return this;
        else return _parentArea.GetParentArea();
    }

    public HoneycombTypes.Variety areaType = HoneycombTypes.Variety.Path;

    public PerlinNoiseChamber myChamber;
    public int depth = 0;

    public int maxParentRadius { get { return parentArea == null || parentArea == this ? _maxRadius : parentArea.maxParentRadius; } }
    private int _maxRadius = 0;
    public int maxRadius { get { return _maxRadius; } }

    List<HoneycombPos> chamberHex = new List<HoneycombPos>();
    public List<HoneycombPos> GetChamberHex() { return chamberHex; }
    public bool HasHex(HoneycombPos pos) { return chamberHex.Contains(pos); }
    public bool HasHex(HexDepth hex) { return chamberHex.Contains(hex.pos); }
    public void AddHex(HoneycombPos pos) { if (!HasHex(pos)) chamberHex.Add(pos); }
    public void AddHex(HexDepth hex)
    {
        if (!HasHex(hex))
        {
            chamberHex.Add(hex.pos);
            if (hex.maxRadius > _maxRadius) _maxRadius = hex.maxRadius;
        }
    }
    public void Remove(HoneycombPos pos) { chamberHex.Remove(pos); }
    public PerlinNoiseArea(PerlinNoiseChamber myChamber, int areaID, HoneycombPos pos)
    {
        this.pos = pos;
        this.areaID = areaID;
        this.myChamber = myChamber;
        chamberHex = new List<HoneycombPos>();
    }
    public PerlinNoiseArea(PerlinNoiseChamber myChamber, int areaID, HoneycombPos pos, List<HoneycombPos> chamberHex)
    {
        this.pos = pos;
        this.areaID = areaID;
        this.myChamber = myChamber;
        this.chamberHex = chamberHex;
    }
}
