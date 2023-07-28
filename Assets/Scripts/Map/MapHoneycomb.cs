using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//-----------------------------------------------------MapHoneycomb------------------------------------------------------------------
public class MapHoneycomb
{
    public bool visible;
    public bool visited;
    public bool display;
    public Vector2 position;
    public GameObject honeycomb;

    private bool hasEnemy = false;

    public Color color = new Color(74/256, 156 / 256, 101 / 256);

    
    private int depth = int.MaxValue; //roughly the number of honeycombs away from a void

    private bool isLargeLoc = false;

    private bool isLargeHoneycomb;
    private bool beeuilding;
    private bool capped = true;
    public bool isFloor;

    public float health = 10;
    private float beeuildingHealth = 30;

    public HoneycombTypes.Variety LocationType;
    public HoneycombTypes.Areas AreaType = HoneycombTypes.Areas.Connection;

    
    public MapHoneycomb(bool display, Vector2 position, bool capped, bool isLargeLoc)
    {
        this.display = display;
        this.position = position;
        this.capped = capped;
        this.isLargeLoc = isLargeLoc;
        if (isLargeLoc)
        {
            int rand = Random.Range(0, 100);
            if (rand < 5)
            {
                beeuilding = true;
                health = beeuildingHealth;
                //Debug.Log("beeuilding!");
            }
        }
        
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
    public bool GetCapped() { return capped; }

    public void SetDepth(int depth){ this.depth = depth; }
    public int GetDepth() { return depth; }

    bool ignoreLarge = false; //---------------------------------------------------------------ignoreLarge---------------------------------------------------------------

    public void DisplayHoneycomb()
    {
        visited = true;
        if (display)
        {
            
            if (isFloor)
            {
                if(Map.StaticMap.DisplayFloor)
                    honeycomb = Map.GetHoneycombChamberFloor(AreaType);
            }
            else if((depth < 5 || depth < 7 &&  !isLargeLoc ) || ignoreLarge && !beeuilding)
            {
                honeycomb = Map.GetHoneycomb();
                isLargeHoneycomb = false;
                beeuilding = false;
                if(hasEnemy) setupEnemyTrigger(true);
                
            }
            else if (beeuilding)
            {
                //health = beeuildingHealth;
                honeycomb = Map.GetBeeCity();
            }
            else if (isLargeLoc)
            {
                honeycomb = Map.GetHoneycombLarge();
                isLargeHoneycomb = true;
            }

            if (honeycomb)
            {
                visible = true;
                honeycomb.transform.position = position;
                honeycomb.GetComponent<Honeycomb>().mapHoneycomb = this;
                honeycomb.SetActive(true);
                //honeycomb.GetComponent<Honeycomb>().LocationType = LocationType;
                if (depth <= 2) capped = false;
                if (isFloor) capped = true;
                else
                {
                    //if (depth <= 3) honeycomb.GetComponent<Collider2D>().enabled = true;
                    //else honeycomb.GetComponent<Collider2D>().enabled = false;
                }

                honeycomb.GetComponent<Honeycomb>().SetupHoneycomb();
            }

        }

    }

    public void HideHoneycomb()
    {
        //visible = false;
        if (display && honeycomb)
        {
            if (hasEnemy) setupEnemyTrigger(false);

            honeycomb.GetComponent<Honeycomb>().HideHoneycomb();
            honeycomb.SetActive(false);
            if (isFloor)
            {
                Map.ReturnHoneycombChamberFloor(honeycomb, AreaType);
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

    public void SetHasEnemy(bool hasEnemy)
    {
        this.hasEnemy = hasEnemy;
        //honeycomb.GetComponent<Collider2D>().enabled = true;
    }
    
    public GameObject GetEnemyPrefab()
    {
        return MapManager.singleton.EnemyPrefab;
    }

    private void setupEnemyTrigger(bool active)
    {
        //honeycomb.GetComponent<CircleCollider2D>().enabled = active;
        HoneycombCell hex = honeycomb.GetComponent<HoneycombCell>();
        if (hex.EnemyTrigger)
            hex.EnemyTrigger.enabled = active;
        else
            Debug.LogWarning($"MapHoneycomb.setupEnemyTrigger: EnemyTrigger missing");
    }

    //public void DamageHoneycomb(int depth, HoneycombTypes.Areas newArea, HoneycombTypes.Variety newVariety)
    //{
    //    if (!beeuilding && display && depth < this.depth)
    //    {
    //        HideHoneycomb();
    //        SetDepth(depth);

    //        if (visible)
    //        {
    //            //LocationType = newVariety;
    //            //isFloor = true;
    //            //AreaType = newArea;
    //            //display = true;

    //            DisplayHoneycomb();
    //        }
    //        if ((depth <= Map.StaticMap.TunnelDestructionDepth || (isLargeLoc || beeuilding)) && honeycomb)
    //            DamageAdjecentHoneycomb(depth + 1, newArea, newVariety);
    //    }

    //}

    public void DamageHoneycomb(int depth)
    {
        if (!beeuilding && display && depth < this.depth)
        {
            HideHoneycomb();
            SetDepth(depth);

            if (visible) DisplayHoneycomb();
            if ((depth <= Map.StaticMap.TunnelDestructionDepth || (isLargeLoc || beeuilding)) && honeycomb)
                DamageAdjecentHoneycomb(depth + 1);
        }

    }

    public void DamageAdjecentHoneycomb(int depth)
    {
        HoneycombPos hexPos = Utility.Honeycomb.WorldPointToHoneycombGrid(position);
        Map.StaticMap.GetHoneycomb(hexPos.GetAdjecentHoneycomb(0, 1)).DamageHoneycomb(depth);
        Map.StaticMap.GetHoneycomb(hexPos.GetAdjecentHoneycomb(1, 1)).DamageHoneycomb(depth);
        Map.StaticMap.GetHoneycomb(hexPos.GetAdjecentHoneycomb(1, -1)).DamageHoneycomb(depth);
        Map.StaticMap.GetHoneycomb(hexPos.GetAdjecentHoneycomb(0, -1)).DamageHoneycomb(depth);
        Map.StaticMap.GetHoneycomb(hexPos.GetAdjecentHoneycomb(-1, -1)).DamageHoneycomb(depth);
        Map.StaticMap.GetHoneycomb(hexPos.GetAdjecentHoneycomb(-1, 1)).DamageHoneycomb(depth);
    }

}
