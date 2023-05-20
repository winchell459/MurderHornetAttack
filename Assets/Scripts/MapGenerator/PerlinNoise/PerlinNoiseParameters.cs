using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PerlinNoiseParameters
{
    public enum NormalizeMode { Local, Global };
    public int seed;
    public float scale;
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;
    public Vector2 offset;
    public NormalizeMode normalizeMode;

    public AnimationCurve depthCurve;
    public float threshold = 0.5f;

    public bool randomSeed = false;

    public enum FalloffTypes { none, honecomb }
    public FalloffTypes falloffType;
}
