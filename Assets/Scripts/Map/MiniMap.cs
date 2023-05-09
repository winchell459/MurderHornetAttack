﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public static MiniMap singleton;
    private List<float[,]> heatMaps = new List<float[,]>();
    int displayingHeatMap = -1;
    int toDisplayHeatMap = -1;

    public int minimizedSize = 200;
    public int maximizedSize = 1000;
    private static int currentSize = 200;
    // Start is called before the first frame update
    void Awake()
    {
        if (singleton == null) singleton = this;
        else Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(toDisplayHeatMap != displayingHeatMap)
        {
            displayingHeatMap = toDisplayHeatMap;
            DisplayMap();
        }
    }
    private void DisplayMap()
    {
        GetComponent<MapDisplay>().DrawTexture(TextureGenerator.TextureFromHeightMap(heatMaps[displayingHeatMap]), currentSize);
    }

    public void AddHeatMap(float[,] heatMap)
    {
        heatMaps.Add(heatMap);
        toDisplayHeatMap = heatMaps.Count - 1;
    }

    public void NextHeatMap()
    {
        toDisplayHeatMap = (toDisplayHeatMap + 1) % heatMaps.Count;
    }

    public void BackHeatMap()
    {
        toDisplayHeatMap -= 1;
        if (toDisplayHeatMap < 0) toDisplayHeatMap = heatMaps.Count - 1;
    }

    public void ToggleSize()
    {
        if (currentSize == maximizedSize) currentSize = minimizedSize;
        else currentSize = maximizedSize;
        DisplayMap();
    }
}