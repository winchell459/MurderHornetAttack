using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPalette : MonoBehaviour
{
    public static ColorPalette singleton;
    // Start is called before the first frame update
    void Awake()
    {
        singleton = this;
    }

    [System.Serializable]
    public struct Variety
    {
        public HoneycombTypes.Variety variety;
        public Color color;
    }
    public Variety[] colorVarieties;
    public Color GetVarietyColor(HoneycombTypes.Variety variety)
    {
        foreach(Variety cv in colorVarieties)
        {
            if (cv.variety == variety) return cv.color;
        }
        return Color.magenta;
    }

    [System.Serializable]
    public struct Area
    {
        public HoneycombTypes.Areas area;
        public Color floorColor, edgeColor, embeddedColor, largeColor;
        
    }
    public Area[] colorAreas;
    public Color GetAreaColor(HoneycombTypes.Areas area, int depth)
    {
        foreach(Area ca in colorAreas)
        {
            if(ca.area == area)
            {
                switch (depth)
                {
                    case 0: return ca.floorColor;
                    case 1: return ca.edgeColor;
                    case 2: return ca.embeddedColor;
                    case 3: return ca.largeColor;
                }
            }
         }
        return Color.magenta;
    }
}
