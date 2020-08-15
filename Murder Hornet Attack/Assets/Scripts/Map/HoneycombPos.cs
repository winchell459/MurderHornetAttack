using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoneycombPos 
{
    public int x;
    public int y;
    public Vector2 vector2 { get { return new Vector2(x, y); } }
    public HoneycombPos() { }
    public HoneycombPos(int x, int y)
    {
        this.x = x;
        this.y = y;
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
}
