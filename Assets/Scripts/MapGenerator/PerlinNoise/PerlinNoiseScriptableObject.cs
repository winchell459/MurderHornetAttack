using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PerlinNoise", menuName = "PerlinNoise")]
public class PerlinNoiseScriptableObject : ScriptableObject
{
    public PerlinNoiseParameters parameters;
}
