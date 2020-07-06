using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    
    public GameObject HoneycombPrefab;
    public GameObject HoneycombCappedPrefab;
    public GameObject HoneycombLargePrefab;
    public GameObject BeeCityPrefab;
    public static Map StaticMap;
    public float MapWidth = 10;
    public float MapHeight = 10;

    public Vector2 MapOrigin; // bottom left corner of map
    public float VerticalSpacing = 0.27f;
    public float HorizontalSpacing = 0.45f;
    public List<GameObject> HoneycombPool = new List<GameObject>();
    public List<GameObject> HoneycombLargePool = new List<GameObject>();
    private List<MapChunk> honeycombChunks = new List<MapChunk>();
    private List<bool> displayChunks = new List<bool>();
    private List<GameObject> beeCityPool = new List<GameObject>();
    public Transform HoneycombLayer_1;

    public Transform[] HoneycombLayers;
    public List<float> LayerScales = new List<float>();

    private int honeycombHeight = -20;
    public int ChunkHeight = 40;
    public int ChunkWidth = 14;
    
    private MapPath path;
    private List<MapVoid> voids = new List<MapVoid>();

    public Camera cam;
    public bool Display;

    // Start is called before the first frame update
    void Awake()
    {
        StaticMap = this;
        
        createChunks();
        
        for(int i = 0; i < HoneycombLayers.Length; i += 1)
        {
            LayerScales.Add(HoneycombLayers[i].localScale.x);
        }
    }
    public int chunkRadius = 3;
    private void Update()
    {
        if (Display)
        {
            
            List<int> chunkLoc = new List<int>();
            for (int i = 0; i < honeycombChunks.Count; i+=1)
            {
                MapChunk chunk = honeycombChunks[i];
                if (chunk.CheckPointInChunk(cam.transform.position))
                {
                    displayChunks[i] = true;
                    chunkLoc.Add(i);
                }
                else displayChunks[i] = false;
            }

            int chunkRows = (int)Mathf.Ceil((MapHeight / VerticalSpacing) / (ChunkHeight));
            int chunkCols = (int)Mathf.Ceil((MapWidth / HorizontalSpacing) / (ChunkWidth));
            foreach(int index in chunkLoc)
            {
                int colCenter = index % chunkCols;
                int rowCenter = index/ chunkCols;
                //Debug.Log(col + " " + row);
                for(int j = 0; j < chunkRadius; j += 1)
                {
                    for(int i = 0; i <= chunkRadius - j; i += 1)
                    {
                        int col = colCenter + i;
                        int row = rowCenter + j;
                        if(col < chunkCols && col >=0 && row < chunkRows && row >= 0) displayChunks[col + row * chunkCols] = true;

                        col = colCenter - i;
                        row = rowCenter - j;
                        if (col < chunkCols && col >= 0 && row < chunkRows && row >= 0) displayChunks[col + row * chunkCols] = true;

                        col = colCenter + i;
                        row = rowCenter - j;
                        if (col < chunkCols && col >= 0 && row < chunkRows && row >= 0) displayChunks[col + row * chunkCols] = true;

                        col = colCenter - i;
                        row = rowCenter + j;
                        if (col < chunkCols && col >= 0 && row < chunkRows && row >= 0) displayChunks[col + row * chunkCols] = true;
                    }
                }
            }

            for(int i = 0; i < honeycombChunks.Count; i += 1)
            {
                if (displayChunks[i]) honeycombChunks[i].DisplayChunk();
                else honeycombChunks[i].DestroyChunk();
            }
        }
        
    }

    private MapChunk GetChunk(int col, int row)
    {
        int chunkCols = (int)Mathf.Ceil((MapWidth / HorizontalSpacing) / (ChunkWidth));
        return honeycombChunks[row * chunkCols + col];
    }

    
    
    private void createChunks()
    {
        int xChunkCount = (int)Mathf.Ceil((MapWidth / HorizontalSpacing) / ChunkWidth);
        int yChunkCount = (int)Mathf.Ceil((MapHeight / VerticalSpacing) / ChunkHeight);
        
        
        for (int j = 0; j < yChunkCount; j += 1)
        {
            for (int i = 0; i < xChunkCount; i += 1)
            {

                Vector2 origin = MapOrigin + new Vector2(i * ChunkWidth, j * ChunkHeight);
                int width = (int)Mathf.Ceil((MapWidth - ChunkWidth * i*HorizontalSpacing) / HorizontalSpacing);
                int height = (int)Mathf.Ceil((MapHeight - ChunkHeight * j*VerticalSpacing) / VerticalSpacing);
                if (width > ChunkWidth) width = ChunkWidth;
                if (height > ChunkHeight) height = ChunkHeight;
                //Debug.Log(width + " " + height);
                createChunk(origin, width, height);
                //honeycombChunks[honeycombChunks.Count - 1].DisplayChunk();
            }
        }
    }

    private void createChunk(Vector2 start, float width, float height)
    {
        MapChunk chunk = new MapChunk(start, width, height, VerticalSpacing, HorizontalSpacing);
        honeycombChunks.Add(chunk);
        displayChunks.Add(false);
        
    }

    public void DisplayChunks()
    {
        foreach (MapChunk mp in honeycombChunks)
        {
            foreach (MapVoid v in voids)
            {
                mp.AddVoid(v);
            }
            //mp.DisplayChunk();
        }
    }

    public void AddVoid(MapVoid newVoid)
    {
        voids.Add(newVoid);
    }
    public void AddVoid(List<MapVoid> newVoids)
    {
        foreach (MapVoid v in newVoids) AddVoid(v);
    }

    public static GameObject GetHoneycomb()
    {
        GameObject honeycomb;
        if(StaticMap.HoneycombPool.Count > 0)
        {
            honeycomb = StaticMap.HoneycombPool[0];
            StaticMap.HoneycombPool.RemoveAt(0);
        }
        else
        {
            int rand = Random.Range(0, 2);
            GameObject prefab = StaticMap.HoneycombCappedPrefab;
            //if (rand == 0) prefab = StaticMap.HoneycombCappedPrefab;
            //else prefab = StaticMap.HoneycombPrefab;
            honeycomb = Instantiate(prefab, StaticMap.transform.position, Quaternion.identity);
            honeycomb.transform.parent = StaticMap.HoneycombLayer_1;
        }
        return honeycomb;
    }

    public static void ReturnHoneycomb(GameObject honeycomb)
    {
        StaticMap.HoneycombPool.Add(honeycomb);
    }

    public static GameObject GetHoneycombLarge()
    {
        GameObject honeycomb;
        if (StaticMap.HoneycombLargePool.Count > 0)
        {
            honeycomb = StaticMap.HoneycombLargePool[0];
            StaticMap.HoneycombLargePool.RemoveAt(0);
        }
        else
        {
            int rand = Random.Range(0, 2);
            GameObject prefab = StaticMap.HoneycombLargePrefab;
            
            honeycomb = Instantiate(prefab, StaticMap.transform.position, Quaternion.identity);
            honeycomb.transform.parent = StaticMap.HoneycombLayer_1;
        }
        return honeycomb;
    }

    public static void ReturnHoneycombLarge(GameObject honeycomb)
    {
        StaticMap.HoneycombLargePool.Add(honeycomb);
    }

    public static GameObject GetBeeCity()
    {
        GameObject beeCity;
        if(StaticMap.beeCityPool.Count > 0)
        {
            beeCity = StaticMap.beeCityPool[0];
            StaticMap.beeCityPool.RemoveAt(0);
        }
        else
        {
            beeCity = Instantiate(StaticMap.BeeCityPrefab, StaticMap.transform.position, Quaternion.identity);

        }
        return beeCity;
    }

    public static void ReturnBeeCity(GameObject beeCity)
    {
        StaticMap.beeCityPool.Add(beeCity);
    }
}
