﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapGeneratorParameters", menuName = "Map Generator Parameters")]
public class MapGeneratorParameters : ScriptableObject
{
    public enum GenerationTypes { randomVoids, perlinNoise, pillapillarPit }
    public GenerationTypes generationType;

    public int nestCount = 3, gardenCount = 3, antMoundCount = 5;
}
