using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    
    public GameObject HoneycombPrefab;
    public GameObject HoneycombCappedPrefab;
    public static Map StaticMap;
    public float MapWidth = 10;
    public float MapHeight = 10;

    public Vector2 MapOrigin; // bottom left corner of map
    public float VerticalSpacing = 0.27f;
    public float HorizontalSpacing = 0.45f;
    public List<GameObject> HoneycombPool = new List<GameObject>();
    private List<MapChunk> honeycombChunks = new List<MapChunk>();
    private List<bool> displayChunks = new List<bool>();
    public Transform HoneycombLayer_1;

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
        
    }

    private void Update()
    {
        if (Display)
        {
            int chunkRadius = 3;
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

    private Vector2 honeycombToWorldPostion(Vector2 honeyPos)
    {
        return new Vector2(honeyPos.x * HorizontalSpacing, honeyPos.y * VerticalSpacing);
    }

    public Vector2 WorldToHoneycomb(Vector2 worldPos)
    {
        float x = worldPos.x / HorizontalSpacing - MapOrigin.x;
        float y = worldPos.y / VerticalSpacing - MapOrigin.y;
        return new Vector2(x, y);
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
}
//-------------------------------------MapChunk------------------------------------------------------------------------
public class MapChunk
{
    private Vector2 mapOffset;
    private float width;
    private float height;
    private float verticalSpacing;
    private float horizontalSpacing;
    private List<MapHoneycomb> honeycombs = new List<MapHoneycomb>();
    private bool display = false;
    //private List<GameObject> displayhoneycombs;
    
    public MapChunk(Vector2 mapOffset, float width, float height, float verticalSpacing, float horizontalSpacing)
    {
        this.width = width;
        this.height = height;
        this.verticalSpacing = verticalSpacing;
        this.horizontalSpacing = horizontalSpacing;
        this.mapOffset = mapOffset;

        honeycombSetup();
    }

    private void honeycombSetup()
    {
        int wSteps = (int)width; // (int)( Width / HorizontalSpacing) + 1;
        int hSteps = (int)height; // (int)(Height / VerticalSpacing) + 1;

        for (int i = 0; i < wSteps; i++)
        {
            for (int j = 0; j < hSteps; j++)
            {
                if (j % 2 != i % 2)
                {
                    
                    honeycombs.Add(new MapHoneycomb(true, new Vector2((i + mapOffset.x ) * horizontalSpacing, (j + mapOffset.y) * verticalSpacing)));
                }
            }
        }
    }

    public void AddVoid(MapVoid space) //, float pathWidth)
    {
       
        foreach (MapHoneycomb honeycomb in honeycombs)
        {
            honeycomb.display = space.Check(honeycomb);
        }
        
        
    }

    

    public void DisplayChunk()//GameObject HoneycombPrefab)
    {
        if (!display)
        {
            display = true;
            foreach (MapHoneycomb honeycomb in honeycombs)
            {
                honeycomb.DisplayHoneycomb();
            }
        }
        
    }

    public void DestroyChunk()
    {
        if (display)
        {
            foreach (MapHoneycomb honeycomb in honeycombs)
            {
                honeycomb.HideHoneycomb();
            }
        }
        display = false;
        
    }

    public bool CheckPointInChunk(Vector2 point)
    {
        float xMin = mapOffset.x * Map.StaticMap.HorizontalSpacing;
        float yMin = mapOffset.y * Map.StaticMap.VerticalSpacing;
        float xMax = (mapOffset.x + width) * Map.StaticMap.HorizontalSpacing;
        float yMax = (mapOffset.y + height) * Map.StaticMap.VerticalSpacing;

        return (point.x >= xMin && point.x <= xMax && point.y >= yMin && point.y <= yMax);
    }

}

//-----------------------------------------------------MapHoneycomb------------------------------------------------------------------
public class MapHoneycomb
{
    public bool display;
    public Vector2 position;
    private GameObject honeycomb;
    private bool capped = true;
    private int depth = int.MaxValue; //roughly the number of honeycombs away from a void


    public MapHoneycomb(bool display, Vector2 position, bool capped)
    {
        this.display = display;
        this.position = position;
        this.capped = capped;
    }
    public MapHoneycomb(bool display, Vector2 position)
    {
        this.display = display;
        this.position = position;
        
    }

    public void SetCapped(bool capped) { this.capped = capped; }

    public void SetDepth(int depth)
    {
        this.depth = depth;
    }
    public int GetDepth() { return depth; }

    public void DisplayHoneycomb()
    {
        if (display)
        {
            honeycomb = Map.GetHoneycomb();
            honeycomb.transform.position = position;
            honeycomb.GetComponent<Honeycomb>().honeyGrid = this;
            honeycomb.SetActive(true);
            if (depth <= 2) capped = false;
            honeycomb.GetComponent<Honeycomb>().SetCapped(capped);
        }
    }

    public void HideHoneycomb()
    {
        if (display && honeycomb)
        {
            honeycomb.SetActive(false);
            Map.ReturnHoneycomb(honeycomb);

        }
    }

    public void DestroyHoneycomb()
    {
        HideHoneycomb();
        display = false;   
    }
}


