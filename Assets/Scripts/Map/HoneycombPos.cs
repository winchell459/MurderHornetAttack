using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoneycombPos 
{
    public int x;
    public int y;
    public Vector2 vector2 { get { return new Vector2(x, y); } }
    public Vector2 worldPos { get { return Utility.Honeycomb.HoneycombGridToWorldPostion(this); } }
    public HoneycombPos() { }
    public HoneycombPos(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public static HoneycombPos operator +(HoneycombPos a, HoneycombPos b) => new HoneycombPos(a.x + b.x, a.y + b.y);
    public static HoneycombPos operator -(HoneycombPos a, HoneycombPos b) => new HoneycombPos(a.x - b.x, a.y - b.y);
    public static bool operator ==(HoneycombPos a, HoneycombPos b) { return a.x == b.x && a.y == b.y; }
    public static bool operator !=(HoneycombPos a, HoneycombPos b) { return a.x != b.x || a.y != b.y; }
    public override bool Equals(object obj)
    {
        HoneycombPos other = obj as HoneycombPos;
        return other.x == x && other.y == y;
    }
    public List<HoneycombPos> GetAdjecentHoneycomb(int radius)
    {
        if (radius == 0) return new List<HoneycombPos>();
        List<HoneycombPos> neighbors = GetAdjecentHoneycomb();
        neighbors.Insert(0, this);
        radius -= 1;
        int current = 1;
        
        while(radius > 0)
        {
            int end = neighbors.Count - 1;
            while (current <= end)
            {
                foreach(HoneycombPos pos in neighbors[current].GetAdjecentHoneycomb())
                {
                    if (!neighbors.Contains(pos)) neighbors.Add(pos);
                }
                current += 1;
            }
            radius-=1;
        }

        neighbors.RemoveAt(0);
        return neighbors;
    }

    public List<HoneycombPos> GetAdjecentHoneycomb()
    {
        List<HoneycombPos> neighbors = new List<HoneycombPos>();
        neighbors.Add(GetAdjecentHoneycomb(0, 1));
        neighbors.Add(GetAdjecentHoneycomb(1, 1));
        neighbors.Add(GetAdjecentHoneycomb(1, -1));
        neighbors.Add(GetAdjecentHoneycomb(0, -1));
        neighbors.Add(GetAdjecentHoneycomb(-1, -1));
        neighbors.Add(GetAdjecentHoneycomb(-1, 1));
        return neighbors;
    }

    

    public HoneycombPos GetAdjecentHoneycomb (HoneycombDir dir)
    {
        HoneycombPos end = new HoneycombPos(x,y);
        end.x += dir.x;
        if (dir.x == 0) end.y += dir.y;
        else if (x % 2 == 0 && dir.y > 0 || x % 2 != 0 && dir.y < 0)
        {
            //end.y += Mathf.Sign(dir.y) * Mathf.Ceil((float)honeyDistance / 2);
            end.y += (int)(Mathf.Sign(dir.y));// * Mathf.Ceil((float)honeyDistance / 2));
        }
        else
        {
            //end.y += Mathf.Sign(dir.y) * Mathf.Ceil(((float)honeyDistance - 1) / 2);
            //end.y += (int)(Mathf.Sign(dir.y) * Mathf.Ceil(((float)honeyDistance - 1) / 2));
        }

        return end;
    }

    public HoneycombPos GetAdjecentHoneycomb(int x, int y)
    {
        return GetAdjecentHoneycomb(new HoneycombDir(x, y));
    }

    public override string ToString()
    {
        return "[" + x + ", " + y + "]";
    }


}
