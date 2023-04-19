using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPerlinNoise : MonoBehaviour
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

    public bool randomSeed = false;

    public enum FalloffTypes { none, honecomb}
    public FalloffTypes falloffType;
    

    public float[,] GenerateNoiseMap(int mapWidth, int mapHeight)
    {
        seed = randomSeed ? Random.Range(0,1<<20) : seed;
        
        float[,] noiseMap = GenerateNoiseMap(mapWidth, mapHeight, seed, scale, octaves, persistance, lacunarity, offset, normalizeMode);

        if(falloffType == FalloffTypes.honecomb)
        {
            float[,] falloff = FalloffGenerator.GenerateFalloffMap(mapWidth, mapHeight);
            for(int i = 0; i < mapWidth; i++)
            {
                for(int j = 0; j<mapHeight; j++)
                {
                    noiseMap[i, j] = Mathf.Clamp01(noiseMap[i, j] + falloff[i, j]);
                }
            }

        }
        FindObjectOfType<MapDisplay>().DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        return noiseMap;
    }

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, NormalizeMode normalizeMode)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < octaves; i += 1)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }

        //clamp scale
        scale = scale <= 0 ? 0.0001f : scale;

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;


        for (int y = 0; y < mapHeight; y += 1)
        {
            for (int x = 0; x < mapWidth; x += 1)
            {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i += 1)
                {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                if (noiseHeight > maxLocalNoiseHeight) maxLocalNoiseHeight = noiseHeight;
                if (noiseHeight < minLocalNoiseHeight) minLocalNoiseHeight = noiseHeight;
                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y += 1)
        {
            for (int x = 0; x < mapWidth; x += 1)
            {
                if (normalizeMode == NormalizeMode.Local)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                }
                else
                {
                    float normalizedHeight = (noiseMap[x, y] + 1) / (2f * maxPossibleHeight / 1.75f);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }

            }
        }
        return noiseMap;
    }
}

public class PerlineNoiseVoid : MapVoid
{
    private MapPerlinNoise mapPerlinNoise;
    private int width, height;
    private float[,] map;
    private float threshold = 0.5f;
    
    public PerlineNoiseVoid(MapPerlinNoise mapPerlinNoise, int width, int height)
    {
        this.mapPerlinNoise = mapPerlinNoise;
        this.height = height;
        this.width = width;
        map = mapPerlinNoise.GenerateNoiseMap(width+1, height+1);
        VoidType = HoneycombTypes.Variety.Path;
    }
    int maxX = int.MinValue, minX = int.MaxValue, maxY = int.MinValue, minY = int.MaxValue;
    public override bool Check(MapHoneycomb honeycomb)
    {
        HoneycombPos honeycombPos = Utility.WorldPointToHoneycombGrid(honeycomb.position);

        //bool changed = false;
        //if (honeycombPos.x > maxX)
        //{
        //    maxX = honeycombPos.x;
        //    changed = true;
        //}
        //if (honeycombPos.y > maxY)
        //{
        //    maxY = honeycombPos.y;
        //    changed = true;
        //}
        //if (honeycombPos.x < minX)
        //{
        //    minX = honeycombPos.x;
        //    changed = true;
        //}
        //if (honeycombPos.y < minY)
        //{
        //    minY = honeycombPos.y;
        //    changed = true;
        //}

        //if (changed) Debug.Log($"(minX,minY):({minX},{minY}) | (maxX,maxY):({maxX},{maxY})");

        float perlinValue = map[honeycombPos.x/* % width*/, honeycombPos.y /*% height*/];
        //Debug.Log(honeycombPos);
        if (perlinValue > threshold)
        {
            float depth = mapPerlinNoise.depthCurve.Evaluate(perlinValue);
            //Debug.Log(depth);
            if (honeycomb.GetDepth() > depth) honeycomb.SetDepth((int)depth);
            CheckDepth((int)depth, honeycomb, VoidType);
            return true;
        }
        else
        {
            honeycomb.isFloor = true;
            return false;
        }

        
            
    }
}

