using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//-----------------------------------------------------MapHoneycomb------------------------------------------------------------------
public class MapHoneycomb
{
    public bool visited;
    public bool display;
    public Vector2 position;
    public GameObject honeycomb;
    private GameObject EnemyPrefab;

    public Color color = new Color(74/256, 156 / 256, 101 / 256);

    //private List<Transform> honeycombChildren = new List<Transform>();
    private bool capped = true;
    private int depth = int.MaxValue; //roughly the number of honeycombs away from a void
    private bool beeuilding;
    public bool isLargeLoc = false;
    private bool isLargeHoneycomb;
    public bool isFloor;

    public float health = 10;
    public float beeuildingHealth = 30;
    public HoneycombTypes.Variety LocationType;
    public HoneycombTypes.Areas AreaType = HoneycombTypes.Areas.Connection;

    
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

    bool ignoreLarge = false; //---------------------------------------------------------------ignoreLarge---------------------------------------------------------------
    public void DisplayHoneycomb()
    {
        visited = true;
        if (display)
        {
            
            if (isFloor)
            {
                if(Map.StaticMap.DisplayFloor)honeycomb = Map.GetHoneycombChamberFloor();
                honeycomb.GetComponent<HoneycombCell>().SetCapColor(color);
            }
            else if((depth < 5 || depth < 7 &&  !isLargeLoc ) || ignoreLarge && !beeuilding)
            {
                honeycomb = Map.GetHoneycomb();
                isLargeHoneycomb = false;
                beeuilding = false;
                if(EnemyPrefab) setupEnemyTrigger(true);
                
            }
            else if (beeuilding)
            {
                health = beeuildingHealth;
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
                if (isFloor) capped = true;
                else
                {
                    //if (depth <= 3) honeycomb.GetComponent<Collider2D>().enabled = true;
                    //else honeycomb.GetComponent<Collider2D>().enabled = false;
                }

                

                if (beeuilding && !isLargeHoneycomb && !isFloor) honeycomb.GetComponent<HoneycombTower>().SetupBeeTower();
                else 
                {
                    honeycomb.GetComponent<HoneycombCell>().SetCapped(capped);
                    //if (depth <= 3 && !isFloor)
                    //{
                    //    honeycomb.GetComponent<HoneycombCell>().HoneycombBase.SetActive(true);
                    //    honeycomb.GetComponent<HoneycombCell>().HoneycombBase.transform.parent = Map.StaticMap.HoneycombLayers[0];
                        
                    //}
                    //honeycomb.transform.parent = Map.StaticMap.HoneycombLayers[1];
                    //honeycomb.transform.localPosition = honeycomb.transform.localPosition * Map.StaticMap.HoneycombLayers[1].localScale.x;

                }
            }

        }

    }

    public void HideHoneycomb()
    {
        if (display && honeycomb)
        {
            if (EnemyPrefab) setupEnemyTrigger(false);

            honeycomb.GetComponent<Honeycomb>().HideHoneycomb();
            honeycomb.SetActive(false);
            if (isFloor)
            {
                Map.ReturnHoneycombChamberFloor(honeycomb);
            }
            else if (!isLargeHoneycomb && honeycomb && !beeuilding)
            {

                GameObject honeycombBase = honeycomb.GetComponent<HoneycombCell>().HoneycombBase;
                honeycombBase.SetActive(false);
                honeycombBase.transform.parent = honeycomb.transform ;
                honeycombBase.transform.localPosition = Vector2.zero;
                Map.ReturnHoneycomb(honeycomb);
                
            }
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

    private void setupEnemyTrigger(bool active)
    {
        //honeycomb.GetComponent<CircleCollider2D>().enabled = active;
        honeycomb.GetComponent<HoneycombCell>().EnemyTrigger.enabled = active;
    }

    public void DamageHoneycomb(int depth)
    {
        if (!beeuilding && display && depth < this.depth)
        {
            HideHoneycomb();
            SetDepth(depth);
            
            DisplayHoneycomb();
            if ((depth <= Map.StaticMap.TunnelDestructionDepth || (isLargeLoc || beeuilding)) && honeycomb) honeycomb.GetComponent<Honeycomb>().DamageAdjecentHoneycomb(depth + 1); 
        }
        
    }
}
