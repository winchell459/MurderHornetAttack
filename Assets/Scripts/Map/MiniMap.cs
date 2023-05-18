using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
    public MapDisplay mapDisplay;

    //public bool displayMiniMap;
    //private int[,] depthMap;
    private MapHoneycomb[,] honeycombMap;
    Color[] colorMap;
    public Map map;
    private int currentChunkID = -1;

    HoneycombPos playerPos;

    public Color emptyColor = Color.black;

    // Start is called before the first frame update
    void Awake()
    {
        if (singleton == null) singleton = this;
        else Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(toDisplayHeatMap == displayingHeatMap && displayingHeatMap >= heatMaps.Count )
        {
            if(currentChunkID != map.currentChunk && !waitingColorMap)
            {
                currentChunkID = map.currentChunk;
                Thread thread = new Thread(GenerateColorMap);
                generatingColorMap = true;
                waitingColorMap = true;
                thread.Start();
                
            }

            if (!generatingColorMap && waitingColorMap)
            {
                waitingColorMap = false;
                DisplayMiniMap();
            }
        }
        else if (toDisplayHeatMap != displayingHeatMap)
        {
            
            displayingHeatMap = toDisplayHeatMap;
            if(toDisplayHeatMap >= heatMaps.Count)
            {
                //depthMap = FindObjectOfType<Map>().GetDepthMap();
                if(honeycombMap == null) honeycombMap = map.GetMapHoneycombMap();
                DisplayMiniMap();
            }
            else
            {
                DisplayMap();
            }
            
        }
    }

    bool generatingColorMap = false;
    bool waitingColorMap = false;
    public int zoom = 2;

    //find start point
    int zoomWidth { get { return honeycombMap.GetLength(0) / zoom; } }
    int zoomHeight { get { return honeycombMap.GetLength(1) / zoom; } }
    int xStart { get { return Mathf.Min(honeycombMap.GetLength(0) - zoomWidth, Mathf.Max(0, playerPos.x - zoomWidth / 2)); } }
    int yStart { get { return Mathf.Min(honeycombMap.GetLength(1) - zoomHeight, Mathf.Max(0, playerPos.y - zoomHeight / 2)); } }

    private void GenerateColorMap()
    {
        int width = honeycombMap.GetLength(0);
        int height = honeycombMap.GetLength(1);
        //float[,] heatMap = new float[depthMap.GetLength(0), depthMap.GetLength(1)];
        if(colorMap == null)colorMap = new Color[width * height];

        

        Debug.Log($"width: {width} height: {height} zoomWidth:{zoomWidth} zoomHeight: {zoomHeight} xStart: {xStart} yStart: {yStart}");

        for (int x = xStart; x < xStart + zoomWidth; x += 1)
        {
            for (int y = yStart; y < yStart + zoomHeight; y += 1)
            {
                MapHoneycomb honeycomb = honeycombMap[x, y];
                if (!honeycomb.visited)
                {
                    //colorMap[x + y * width] = emptyColor;
                    FillPixel(x - xStart, y - yStart, width, zoom, emptyColor);
                }
                else if (!honeycomb.display || honeycomb.GetDepth() == 0 || honeycomb.isFloor)
                {
                    //heatMap[x, y] = 0;
                    //colorMap[x + y * width] = ColorPalette.singleton.GetAreaColor(honeycomb.AreaType, 0);
                    FillPixel(x - xStart, y - yStart, width, zoom, ColorPalette.singleton.GetAreaColor(honeycomb.AreaType, 0));
                }
                else if (honeycomb.GetDepth() <= 2)
                {
                    //heatMap[x, y] = 0.25f;
                    //colorMap[x + y * width] = ColorPalette.singleton.GetAreaColor(HoneycombTypes.Areas.Connection, 1);
                    FillPixel(x - xStart, y - yStart, width, zoom, ColorPalette.singleton.GetAreaColor(honeycomb.AreaType, 1));
                }
                else if (honeycomb.GetDepth() < 5)
                {
                    //heatMap[x, y] = 0.5f;
                    //colorMap[x + y * width] = ColorPalette.singleton.GetAreaColor(HoneycombTypes.Areas.Connection, 2);
                    FillPixel(x - xStart, y - yStart, width, zoom, ColorPalette.singleton.GetAreaColor(honeycomb.AreaType, 2));
                }
                else
                {
                    //heatMap[x, y] = 1f;
                    //colorMap[x + y * width] = ColorPalette.singleton.GetAreaColor(HoneycombTypes.Areas.Connection, 3);
                    FillPixel(x - xStart, y - yStart, width, zoom, ColorPalette.singleton.GetAreaColor(honeycomb.AreaType, 3));
                }
            }
        }
        //this.colorMap = colorMap;
        generatingColorMap = false;
    }
    void FillPixel(int x, int y, int width, int zoom, Color color)
    {
        for(int i = x*zoom; i < (x+1)*zoom; i++)
        {
            for(int j = y*zoom; j < (y+1)*zoom; j++)
            {
                //Debug.Log($"x: {i} y: {j} index:{i + j * width} colorMap.Length:{colorMap.Length}");
                colorMap[i + j * width] = color;
            }
        }
    }

    Texture2D texture;
    private void DisplayMiniMap()
    {
        if (texture == null) texture = TextureGenerator.TextureFromColorMap(colorMap, honeycombMap.GetLength(0), honeycombMap.GetLength(1));
        else texture = TextureGenerator.TextureFromColorMap(texture, colorMap, honeycombMap.GetLength(0), honeycombMap.GetLength(1));
        mapDisplay.DrawTexture(texture, currentSize);
        
}
    private void DisplayMap()
    {
        mapDisplay.DrawTexture(TextureGenerator.TextureFromHeightMap(heatMaps[displayingHeatMap]), currentSize);
    }

    public void AddHeatMap(float[,] heatMap)
    {
        heatMaps.Add(heatMap);
        toDisplayHeatMap = heatMaps.Count - 1;
    }

    public void NextHeatMap()
    {
        toDisplayHeatMap = (toDisplayHeatMap + 1) % (heatMaps.Count + 3);
        CheckZoom(); 
    }

    public void BackHeatMap()
    {
        toDisplayHeatMap -= 1;
        if (toDisplayHeatMap < 0) toDisplayHeatMap = heatMaps.Count + 2;
        CheckZoom();
    }
    private void CheckZoom()
    {
        if (toDisplayHeatMap == heatMaps.Count) zoom = 1;
        else if (toDisplayHeatMap == heatMaps.Count + 1) zoom = 2;
        else if (toDisplayHeatMap == heatMaps.Count + 2) zoom = 3;
    }

    public void ToggleSize()
    {
        if (currentSize == maximizedSize) currentSize = minimizedSize;
        else currentSize = maximizedSize;
        if (displayingHeatMap < heatMaps.Count) DisplayMap();
        else DisplayMiniMap();
    }

    public Transform playerMarker;
    public void SetPlayerMarker(HoneycombPos playerPos)
    {
        this.playerPos = playerPos;

        int width = displayingHeatMap >= heatMaps.Count ? honeycombMap.GetLength(0) : heatMaps[displayingHeatMap].GetLength(0);
        int height = displayingHeatMap >= heatMaps.Count ? honeycombMap.GetLength(1) : heatMaps[displayingHeatMap].GetLength(1);
        if(displayingHeatMap >= heatMaps.Count)
        {
            float step = zoom * currentSize / (float)Mathf.Max(width, height);
            Vector2 markerPos = new Vector2((playerPos.x - xStart - zoomWidth) * step + zoom / 2, (playerPos.y - yStart - zoomHeight) * step + zoom / 2);
            playerMarker.localPosition = markerPos;
        }
        else
        {
            float step = currentSize / (float)Mathf.Max(width, height);
            Vector2 markerPos = new Vector2((playerPos.x - width) * step, (playerPos.y - height) * step);
            playerMarker.localPosition = markerPos;
        }
        

        //Debug.Log($"playerPos: {playerPos}  step: {step}  markerPos: {markerPos}  playerMarker.position: {playerMarker.position}");
    }
}
