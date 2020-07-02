using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//-----------------------------------------------------MapHoneycomb------------------------------------------------------------------
public class MapHoneycomb
{
    public bool display;
    public Vector2 position;
    private GameObject honeycomb;
    private GameObject EnemyPrefab;
    //private List<Transform> honeycombChildren = new List<Transform>();
    private bool capped = true;
    private int depth = int.MaxValue; //roughly the number of honeycombs away from a void
    private bool beeuilding;
    public bool isLargeLoc = false;
    private bool isLargeHoneycomb;

    public enum LocationTypes
    {
        Embedded,
        Path,
        Chamber
    }
    public LocationTypes LocationType;

    //public MapHoneycomb(bool display, Vector2 position, bool capped, GameObject EnemyPrefab)
    //{
    //    this.display = display;
    //    this.position = position;
    //    this.capped = capped;
    //    this.EnemyPrefab = EnemyPrefab;
    //}
    public MapHoneycomb(bool display, Vector2 position, bool capped, bool isLargeLoc)
    {
        this.display = display;
        this.position = position;
        this.capped = capped;
        this.isLargeLoc = isLargeLoc;
        int rand = Random.Range(0, 100);
        if (rand < 5 && isLargeLoc) beeuilding = true;
    }
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
            if (depth < 5 || depth < 7 && !isLargeLoc)
            {
                honeycomb = Map.GetHoneycomb();
                isLargeHoneycomb = false;
                beeuilding = false;
                if(EnemyPrefab) honeycomb.GetComponent<CircleCollider2D>().enabled = true;
            }
            else if (beeuilding)
            {
                honeycomb = Map.GetBeeCity();
            }
            else if (isLargeLoc)
            {
                honeycomb = Map.GetHoneycombLarge();
                isLargeHoneycomb = true;
            }

            if (honeycomb)
            {
                honeycomb.transform.position = position;
                honeycomb.GetComponent<Honeycomb>().honeyGrid = this;
                honeycomb.SetActive(true);
                honeycomb.GetComponent<Honeycomb>().LocationType = LocationType;
                if (depth <= 2) capped = false;

                if (beeuilding && !isLargeHoneycomb) honeycomb.GetComponent<HoneycombTower>().SetupBeeTower();
                else honeycomb.GetComponent<HoneycombCell>().SetCapped(capped);
            }

        }

    }

    public void HideHoneycomb()
    {
        if (display && honeycomb)
        {
            if(EnemyPrefab) honeycomb.GetComponent<CircleCollider2D>().enabled = false;
            honeycomb.GetComponent<Honeycomb>().HideHoneycomb();
            honeycomb.SetActive(false);
            if (!isLargeHoneycomb && honeycomb && !beeuilding) Map.ReturnHoneycomb(honeycomb);
            else if (honeycomb && !beeuilding) Map.ReturnHoneycombLarge(honeycomb);
            else if (honeycomb)
            {
                Map.ReturnBeeCity(honeycomb);
            }
            honeycomb = null;
        }
    }

    public void DestroyHoneycomb()
    {
        HideHoneycomb();
        display = false;
    }

    public void AddEnemy(GameObject EnemyPrefab)
    {
        this.EnemyPrefab = EnemyPrefab;
        //honeycomb.GetComponent<Collider2D>().enabled = true;
    }
    
    public GameObject GetEnemyPrefab()
    {
        return EnemyPrefab;
    }
    
}
