using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                    //if(i%6==0 && j%6==0 || i % 6 == 0 && j % 6 == 0)
                    if (i % 3 == 0 && j % 3 == 0)
                    {
                        honeycombs.Add(new MapHoneycomb(true, new Vector2((i + mapOffset.x) * horizontalSpacing, (j + mapOffset.y) * verticalSpacing), true, true));

                        /*Debug.Log("isLarge");*/
                    }
                    else honeycombs.Add(new MapHoneycomb(true, new Vector2((i + mapOffset.x) * horizontalSpacing, (j + mapOffset.y) * verticalSpacing)));

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





