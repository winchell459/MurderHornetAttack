using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Map Parameters", menuName = "MapParameters")]
public class MapParameters : ScriptableObject
{
    public float MapWidth = 200;
    public float MapHeight = 160;
    public float VerticalSpacing = 0.27f;
    public float HorizontalSpacing = 0.45f;

    public int ChunkHeight = 12;
    public int ChunkWidth = 16;

    public int ChunkRadius = 7;
}
