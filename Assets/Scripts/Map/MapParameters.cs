using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Map Parameters", menuName = "MapParameters")]
public class MapParameters : ScriptableObject
{
    public float VerticalSpacing { get { return 0.27f; } }
    public float HorizontalSpacing { get { return 0.45f; } }

    public float MapWidth = 200;
    public float MapHeight = 160;
    public Vector2 MapOrigin;

    public int ChunkHeight = 12;
    public int ChunkWidth = 16;

    public int ChunkRadius = 7;
}
