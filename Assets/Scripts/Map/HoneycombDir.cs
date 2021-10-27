using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoneycombDir 
{
    public int x;
    public int y;
    public Vector2 vector2 { get { return new Vector2(x, y); } }
    public HoneycombDir() { }
    public HoneycombDir(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
