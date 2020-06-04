using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    
    public GameObject HoneycombPrefab;
    public static Map StaticMap;
    public float MapWidth = 10;
    public float MapHeight = 10;

    public Vector2 MapOrigin; // bottom left corner of map
    public float VerticalSpacing = 0.27f;
    public float HorizontalSpacing = 0.45f;
    public List<GameObject> HoneycombPool = new List<GameObject>();
    private List<MapChunk> honeycombChunks = new List<MapChunk>();
    public Transform HoneycombLayer_1;

    private int honeycombHeight = -20;
    public int ChunkHeight = 40;
    public int ChunkWidth = 14;
    
    private MapPath path;
    private List<MapVoid> voids = new List<MapVoid>();
    

    // Start is called before the first frame update
    void Awake()
    {
        StaticMap = this;
        
        createChunks();
        
    }
    
    public void RestartLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    private Vector2 honeycombToWorldPostion(Vector2 honeyPos)
    {
        return new Vector2(honeyPos.x * HorizontalSpacing, honeyPos.y * VerticalSpacing);
    }
    
    private void createChunks()
    {
        int xChunkCount = (int)Mathf.Ceil((MapWidth / HorizontalSpacing) / ChunkWidth);
        int yChunkCount = (int)Mathf.Ceil((MapHeight / VerticalSpacing) / ChunkHeight);
        
        for(int i = 0; i < xChunkCount; i += 1)
        {
            for(int j = 0; j < yChunkCount; j += 1)
            {

                Vector2 origin = MapOrigin + new Vector2(i * ChunkWidth, j * ChunkHeight);
                int width = (int)Mathf.Ceil((MapWidth - ChunkWidth * i*HorizontalSpacing) / HorizontalSpacing);
                int height = (int)Mathf.Ceil((MapHeight - ChunkHeight * j*VerticalSpacing) / VerticalSpacing);
                if (width > ChunkWidth) width = ChunkWidth;
                if (height > ChunkHeight) height = ChunkHeight;
                Debug.Log(width + " " + height);
                createChunk(origin, width, height);
                //honeycombChunks[honeycombChunks.Count - 1].DisplayChunk();
            }
        }
    }

    private void createChunk(Vector2 start, float width, float height)
    {
        MapChunk chunk = new MapChunk(start, width, height, VerticalSpacing, HorizontalSpacing);
        honeycombChunks.Add(chunk);
        
    }

    public void DisplayChunks()
    {
        foreach (MapChunk mp in honeycombChunks)
        {
            foreach (MapVoid v in voids)
            {
                mp.AddVoid(v);
            }
            mp.DisplayChunk();
        }
    }

    public void AddVoid(MapVoid newVoid)
    {
        voids.Add(newVoid);
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
            honeycomb = Instantiate(StaticMap.HoneycombPrefab, StaticMap.transform.position, Quaternion.identity);
            honeycomb.transform.parent = StaticMap.HoneycombLayer_1;
        }
        return honeycomb;
    }
}

public class MapChunk
{
    private Vector2 mapOffset;
    private float width;
    private float height;
    private float verticalSpacing;
    private float horizontalSpacing;
    private List<Honeycomb> honeycombs = new List<Honeycomb>();
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
                    
                    honeycombs.Add(new Honeycomb(true, new Vector2((i + mapOffset.x ) * horizontalSpacing, (j + mapOffset.y) * verticalSpacing)));
                }
            }
        }
    }

    public void AddVoid(MapVoid space) //, float pathWidth)
    {
       
        foreach (Honeycomb honeycomb in honeycombs)
        {
            space.Check(honeycomb);
        }
        
        
    }

    

    public void DisplayChunk()//GameObject HoneycombPrefab)
    {
        foreach(Honeycomb honeycomb in honeycombs)
        {
            honeycomb.DisplayHoneycomb();
        }
    }

    public void DestroyChunk()
    {

    }

}

public class Honeycomb
{
    public bool display;
    public Vector2 position;
    public Honeycomb(bool display, Vector2 position)
    {
        this.display = display;
        this.position = position;
    }

    public void DisplayHoneycomb()
    {
        if (display)
        {
            GameObject honeycomb = Map.GetHoneycomb();
            honeycomb.transform.position = position;
        }
    }
}


