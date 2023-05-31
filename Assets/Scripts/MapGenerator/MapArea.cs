using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapArea : MapChamber
{
    public List<MapChamber> chambers = new List<MapChamber>();
    public List<MapPath> paths = new List<MapPath>();

    public List<HoneycombPos> StartPoints = new List<HoneycombPos>();
    public List<HoneycombPos> EndPoints = new List<HoneycombPos>();
    public List<HoneycombPos> AltPoints = new List<HoneycombPos>();

    public HoneycombTypes.Areas AreaType;

    public override bool Check(MapHoneycomb honeycomb)
    {
        bool display = honeycomb.display;
        if (display)
        {
            bool isFloorCheck = !honeycomb.isFloor;
            foreach (MapChamber chamber in chambers)
            {
                //Debug.Log("MapNest.Check");
                if (!chamber.Check(honeycomb))
                {
                    display = false;
                }

            }

            foreach (MapPath path in paths)
            {
                if (!path.Check(honeycomb))
                {
                    display = false;
                }
            }
            //Debug.Log("MapNest.Check");
            if (isFloorCheck && honeycomb.isFloor /*&& VoidType == HoneycombTypes.Variety.Chamber*/)
            {
                //Debug.Log(AreaType);
                honeycomb.color = ColorPalette.singleton.GetAreaColor(AreaType, 0);
                honeycomb.AreaType = AreaType;
            }
        }
        
        return display;
    }

    public HoneycombPos GetStartPoint(HoneycombPos sourcePos)
    {
        if(StartPoints.Count > 0)
        {
            HoneycombPos startPoint = StartPoints[0];
            foreach(HoneycombPos pos in StartPoints)
            {
                if(Utility.Honeycomb.DistanceBetweenHoneycomb(sourcePos, startPoint) > Utility.Honeycomb.DistanceBetweenHoneycomb(sourcePos, pos))
                {
                    startPoint = pos;
                }
            }
            return startPoint;
        }
        else
        {
            return GetAltPoint(sourcePos);
        }
    }

    public HoneycombPos GetEndPoint(HoneycombPos sourcePos)
    {
        if (EndPoints.Count > 0)
        {
            HoneycombPos endPoint = EndPoints[0];
            foreach (HoneycombPos pos in EndPoints)
            {
                if (Utility.Honeycomb.DistanceBetweenHoneycomb(sourcePos, endPoint) > Utility.Honeycomb.DistanceBetweenHoneycomb(sourcePos, pos))
                {
                    endPoint = pos;
                }
            }
            return endPoint;
        }
        else
        {
            return GetAltPoint(sourcePos);
        }
    }

    public HoneycombPos GetAltPoint(HoneycombPos sourcePos)
    {
        if(AltPoints.Count > 0)
        {
            HoneycombPos altPoint = AltPoints[0];
            foreach (HoneycombPos pos in AltPoints)
            {
                if (Utility.Honeycomb.DistanceBetweenHoneycomb(sourcePos, altPoint) > Utility.Honeycomb.DistanceBetweenHoneycomb(sourcePos, pos))
                {
                    altPoint = pos;
                }
            }
            return altPoint;
        }
        else
        {
            HoneycombPos altPoint = Utility.Honeycomb.WorldPointToHoneycombGrid(locations[0]);
            foreach(Vector2 pos in locations)
            {
                if(Utility.Honeycomb.DistanceBetweenHoneycomb(sourcePos,altPoint) > Utility.Honeycomb.DistanceBetweenHoneycomb(sourcePos, Utility.Honeycomb.WorldPointToHoneycombGrid(pos)))
                {
                    altPoint = Utility.Honeycomb.WorldPointToHoneycombGrid(pos);
                }
            }
            return altPoint;
        }
    }
}
