using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HoneycombDir 
{
    public override string ToString()
    {
        return $"({x},{y})";
    }
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
