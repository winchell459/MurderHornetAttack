using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public Transform Player;
    public GameObject HoneycombPrefab;
    public static Map StaticMap;
    public float Width = 10;
    public float Height = 10;
    public float VerticalSpacing = 0.27f;
    public float HorizontalSpacing = 0.45f;
    public List<GameObject> HoneycombPool = new List<GameObject>();
    private List<MapChunk> honeycombChunks = new List<MapChunk>();
    public Transform HoneycombLayer_1;

    private int honeycombHeight = -20;
    public int HoneycombStepHeight = 40;
    public int HoneycombWidth = 14;
    
    private Path path;
    public GameObject MurderPanel;

    // Start is called before the first frame update
    void Start()
    {
        StaticMap = this;
        MurderPanel.SetActive(false);
        
        createChunk(new Vector2(0, honeycombHeight), HoneycombWidth, HoneycombStepHeight);
        honeycombHeight += HoneycombStepHeight;
        path = new Path(new Vector2(0, -5), new Vector2(0, 10));
        honeycombChunks[0].AddPath(path, 3);
        honeycombChunks[0].DisplayChunk();
    }

    // Update is called once per frame
    void Update()
    {
        if (Player)
        {
            if (Player.transform.position.y > honeycombToWorldPostion(new Vector2(0, honeycombHeight - HoneycombStepHeight)))
            {
                createChunk(new Vector2(0, honeycombHeight), HoneycombWidth, HoneycombStepHeight);
                honeycombHeight += HoneycombStepHeight;
                float randX = Random.Range(-2.3f, 2.3f);
                path = new Path(path.Start(0), new Vector2(randX, path.End(0).y + 16));
                honeycombChunks[honeycombChunks.Count - 1].AddPath(path, 2);
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
    private float honeycombToWorldPostion(Vector2 honeyPos)
    {
        return honeyPos.y * VerticalSpacing;
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
                    //GameObject.Instantiate(HoneycombPrefab, new Vector2(i * horizontalSpacing - horizontalSpacing * width / 2, j * verticalSpacing), Quaternion.identity);

                    //honeycomb.transform.position = ;
                    honeycombs.Add(new Honeycomb(true, new Vector2((i + mapOffset.x - width / 2) * horizontalSpacing, (j + mapOffset.y) * verticalSpacing)));
                }
            }
        }
    }

    public void AddPath(Path path, float pathWidth)
    {
        for(int i = 0; i < path.Count; i++)
        {
            Vector2 start = path.Start(i);
            Vector2 end = path.End(i);
            foreach (Honeycomb hc in honeycombs)
            {
                float distance = pointDistanceToPath(hc.position, start, end);
                if (distance < pathWidth / 2 || distance < 0.45f)
                {
                    hc.display = false;
                }
            }
        }
        
        
    }

    private float pointDistanceToPath(Vector2 point, Vector2 start, Vector2 end)
    {
        float A = point.x - start.x;
        float B = point.y - start.y;
        float C = end.x - start.x;
        float D = end.y - start.y;

        float dot = A * C + B * D;
        float path = C * C + D * D;
        float check = -1;
        if (path != 0) check = dot / path;

        float xx, yy;

        if(check < 0)
        {
            xx = start.x;
            yy = start.y;
        }else if( check > 1)
        {
            xx = end.x;
            yy = end.y;
        }
        else
        {
            xx = start.x + check * C;
            yy = start.y + check * D;
        }
        float dx = point.x - xx;
        float dy = point.y - yy;
        return Mathf.Sqrt(dx * dx + dy * dy);
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

public class Path
{
    private List<Vector2> Points = new List<Vector2>();
    public int Count = 0;
    //public List<Vector2> End = new List<Vector2>();

    public Path(Vector2 start, Vector2 end)
    {
        Points.Add(start);
        Points.Add(end);
        Count += 1;
    }
    public void Add(Vector2 point)
    {
        Points.Add(point);
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
}
