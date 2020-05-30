using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public GameObject HoneycombPrefab;
    public static Map StaticMap;
    public float Width = 10;
    public float Height = 10;
    public float VerticalSpacing = 0.27f;
    public float HorizontalSpacing = 0.45f;
    public List<GameObject> HoneycombPool = new List<GameObject>();
    public Transform HoneycombLayer_1;
    // Start is called before the first frame update
    void Start()
    {
        StaticMap = this;
        MapChunk chunk1 = new MapChunk(new Vector2(-2, 0), 5, 10, VerticalSpacing, HorizontalSpacing);
        MapChunk chunk2 = new MapChunk(new Vector2(3, 5), 5, 10, VerticalSpacing, HorizontalSpacing);
        chunk1.DisplayChunk(HoneycombPrefab);
        chunk2.DisplayChunk(HoneycombPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        
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

    public void DisplayChunk(GameObject HoneycombPrefab)
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
    private bool display;
    private Vector2 position;
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
