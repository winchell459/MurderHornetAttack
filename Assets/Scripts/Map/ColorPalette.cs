using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPalette : MonoBehaviour
{
    public static ColorPalette singleton;
    // Start is called before the first frame update
    void Start()
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
}
