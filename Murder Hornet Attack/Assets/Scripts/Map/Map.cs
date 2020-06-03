using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public Transform Player;
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
    
    private Path path;
    private List<Void> voids = new List<Void>();
    public GameObject MurderPanel;

    // Start is called before the first frame update
    void Start()
    {
        StaticMap = this;
        MurderPanel.SetActive(false);

        //createChunk(new Vector2(0, honeycombHeight), ChunkWidth, ChunckHeight);
        //createChunk(MapOrigin, ChunkWidth, ChunckHeight);
        //honeycombHeight += ChunckHeight;
        //path = new Path(new Vector2(0, -5), new Vector2(0, 10));
        //honeycombChunks[0].AddPath(path, 3);
        //honeycombChunks[0].DisplayChunk();

        createChunks();
        createVoids();
        
        foreach(MapChunk mp in honeycombChunks)
        {
            foreach(Void v in voids)
            {
                mp.AddVoid(v);
            }
            mp.DisplayChunk();
        }
    }

    // Update is called once per frame
    void Update()
    {
       
        
    }
    private void infiniteLevel()
    {
        if (Player)
        {
            if (Player.transform.position.y > honeycombToWorldPostion(new Vector2(0, honeycombHeight - ChunkHeight)).y)
            {
                createChunk(new Vector2(0, honeycombHeight), ChunkWidth, ChunkHeight);
                honeycombHeight += ChunkHeight;
                float randX = Random.Range(-2.3f, 2.3f);
                path = new Path(path.Start(0), new Vector2(randX, path.End(0).y + 16), 2);
                honeycombChunks[honeycombChunks.Count - 1].AddVoid(path);
                honeycombChunks[honeycombChunks.Count - 1].DisplayChunk();
            }
        }
        else
        {
            MurderPanel.SetActive(true);
        }
    }
    public void RestartLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
    private Vector2 honeycombToWorldPostion(Vector2 honeyPos)
    {
        return new Vector2(honeyPos.x * HorizontalSpacing, honeyPos.y * VerticalSpacing);
    }
    private void createVoids()
    {
        //simple path
        Vector2 origin = new Vector2(MapOrigin.x * HorizontalSpacing, MapOrigin.y * VerticalSpacing);
        float minX = origin.x;
        float maxX = origin.x + MapWidth;
        float minY = origin.y;
        float maxY = origin.y + MapHeight;

        float pathWidth = 3;

        float rangeX = 3;
        float rangeY = 3;

        float pathX = Player.position.x;
        float pathY = Player.position.y;

        float endX = Random.Range(0, rangeX) + pathX;
        endX = Mathf.Clamp(endX, minX, maxX);
        float endY = Random.Range(0, rangeY) + pathY;
        endY = Mathf.Clamp(endY, minY, maxY);
        Path newPath = new Path(new Vector2(pathX, pathY), new Vector2(endX, endY), pathWidth);
        pathX = endX;
        pathY = endY;
        while(pathX < origin.x + MapWidth - 1)
        {
            pathWidth = Random.Range(2, 8);
            endX = Random.Range(0, rangeX) + pathX;
            endX = Mathf.Clamp(endX, minX + pathWidth*HorizontalSpacing/2, maxX - pathWidth * HorizontalSpacing / 2);
            endY = Random.Range(-rangeY, rangeY) + pathY;
            endY = Mathf.Clamp(endY, minY + (4 + pathWidth / 2) * VerticalSpacing, maxY - (3 + pathWidth / 2) * HorizontalSpacing );
            newPath.Add(new Vector2(endX, endY), pathWidth);
            pathX = endX;
            pathY = endY;
        }
        voids.Add(newPath);
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

    public void AddVoid(Void space) //, float pathWidth)
    {
        //for(int i = 0; i < path.Count; i++)
        //{
        //    Vector2 start = path.Start(i);
        //    Vector2 end = path.End(i);
        //    foreach (Honeycomb hc in honeycombs)
        //    {
        //        float distance = Utility.PointDistanceToPath(hc.position, start, end);
        //        if (distance < pathWidth / 2 || distance < 0.45f)
        //        {
        //            hc.display = false;
        //        }
        //    }
        //}
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

public abstract class Void
{
    public abstract bool Check(Honeycomb honeycomb);
}

public class Path : Void
{
    private List<Vector2> Points = new List<Vector2>();
    private List<float> widths = new List<float>();
    public int Count = 0;
    //public List<Vector2> End = new List<Vector2>();

    public Path(Vector2 start, Vector2 end, float width)
    {
        Points.Add(start);
        Points.Add(end);
        widths.Add(width);
        Count += 1;
    }
    public void Add(Vector2 point, float width)
    {
        Points.Add(point);
        widths.Add(width);
        Count += 1;
    }
    public Vector2 Start(int pathIndex)
    {
        return Points[pathIndex];
    }
    public Vector2 End(int pathIndex)
    {
        return Points[pathIndex + 1];
    }

    public override bool Check(Honeycomb honeycomb)
    {
        for (int i = 0; i < Count; i++)
        {
            if (honeycomb.display)
            {
                Vector2 start = Start(i);
                Vector2 end = End(i);
                float distance = Utility.PointDistanceToPath(honeycomb.position, start, end);
                if (distance < widths[i] / 2 || distance < 0.45f)
                {
                    honeycomb.display = false;
                }
            }
            
        }
        return honeycomb.display;
    }
}
