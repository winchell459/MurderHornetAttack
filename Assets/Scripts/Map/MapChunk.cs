using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//-------------------------------------MapChunk------------------------------------------------------------------------
public class MapChunk 
{
    
    private int width;
    private int height;
    private float verticalSpacing;
    private float horizontalSpacing;
    private List<MapHoneycomb> honeycombs = new List<MapHoneycomb>();
    private bool display = false;

    //private List<Insect> enemiesInChunk = new List<Insect>();
    private List<IChunkObject> chunkObjects = new List<IChunkObject>();
    private List<IChunkObject> transientChunkObjects = new List<IChunkObject>();

    public Vector2 mapOffset;
    public Vector2 ChunkIndex;
    public bool Visible { get { return display; } }

    public MapChunk(Vector2 mapOffset, int width, int height, float verticalSpacing, float horizontalSpacing)
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

        //hSteps /= 2;
        for (int j = 0; j < hSteps; j++)
            
        {
            for (int i = 0; i < wSteps; i++)
            {
                if (j % 2 != i % 2)
                {
                    //if(i%6==0 && j%6==0 || i % 6 == 0 && j % 6 == 0)
                    if (i % 3 == 0 && j % 3 == 0)
                    {
                        //is large honeycomb location
                        honeycombs.Add(new MapHoneycomb(true, new Vector2((i + mapOffset.x) * horizontalSpacing, (j + mapOffset.y) * (verticalSpacing)), true, true));

                        /*Debug.Log("isLarge");*/
                    }
                    else honeycombs.Add(new MapHoneycomb(true, new Vector2((i + mapOffset.x) * horizontalSpacing, (j + mapOffset.y) * (verticalSpacing))));
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
                honeycomb.visible = true;
            }

            
            
            foreach(IChunkObject chunkObject in chunkObjects)
            {
                chunkObject.Activate();
            }

            CheckTransientObjects(true);
        }

    }

    public void CheckTransientObjects(bool active)
    {
        //for (int i = enemiesInChunk.Count - 1; i >= 0; i -= 1)
        //{
        //    if (enemiesInChunk[i])
        //    {
        //        MapChunk chunk = Utility.Honeycomb.GetMapChunk(enemiesInChunk[i].transform.position);
        //        if (chunk == this)
        //        {
        //            enemiesInChunk[i].gameObject.SetActive(active);
        //        }
        //        else
        //        {

        //            chunk.AddEnemyToChunk(enemiesInChunk[i]);
        //            enemiesInChunk.RemoveAt(i);
        //        }
        //    }
        //    else enemiesInChunk.RemoveAt(i);

        //}

        for (int i = transientChunkObjects.Count - 1; i >= 0; i -= 1)
        {
            IChunkObject tco = transientChunkObjects[i];
            if (tco.GameObject())
            {
                MapChunk chunk = Utility.Honeycomb.GetMapChunk(tco.GameObject().transform.position);
                if (chunk == this)
                {
                    if (active)
                        tco.Activate();
                    else
                        tco.Deactivate();
                }
                else
                {
                    chunk.AddTransientChunkObject(tco);
                    transientChunkObjects.RemoveAt(i);
                }
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
                honeycomb.visible = false;
            }

            foreach (IChunkObject chunkObject in chunkObjects)
            {
                chunkObject.Deactivate();
            }

            CheckTransientObjects(false);
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

    //public void AddEnemyToChunk(Insect insect)
    //{
    //    if (!enemiesInChunk.Contains(insect)) enemiesInChunk.Add(insect);
    //    if (!display) insect.gameObject.SetActive(false);
    //}

    public void AddChunkObject(IChunkObject chunkObject)
    {
        chunkObjects.Add(chunkObject);
        if (Visible) chunkObject.Activate();
        else chunkObject.Deactivate();
    }

    public void AddTransientChunkObject(IChunkObject chunkObject)
    {
        transientChunkObjects.Add(chunkObject);
        chunkObject.SetMyChunk(this);
        //if (Visible) chunkObject.Activate();
        //else chunkObject.Deactivate();
    }

    public void RemoveTransientChunkObject(IChunkObject chunkObject)
    {
        transientChunkObjects.Remove(chunkObject);
    }

    public MapHoneycomb GetMapHoneycomb(int col, int row)
    {
        if (col % 2 == 0) col = (int)(col / 2) + width / 2;
        else col = (int)(col / 2);
        return honeycombs[col + row * width];
    }

    public List<MapHoneycomb> GetAllMapHoneycombs() { return honeycombs; }
}





