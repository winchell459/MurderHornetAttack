using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoneycombTower : Honeycomb
{
    public Transform[] TowerLayers;
    private Transform[] layers;
    public bool HiveCastle = false;
    public GameObject enemyPrefab;
    public float spawnDistance = 10;
    public float spawnRate = 5; //spawn enemy every spawnRate seconds
    private float lastSpawn;

    private HornetController player;

    private void Start()
    {
        lastSpawn = Time.time;
        if (HiveCastle) SetupBeeTower();

    }

    private void Update()
    {
        if (!player) player = FindObjectOfType<HornetController>();
        if (lastSpawn + spawnRate < Time.time && player && Vector2.Distance(player.transform.position, transform.position) <= spawnDistance)
        {
            Debug.Log("HoneycombTower Attacks");
            Instantiate(/*honeyGrid.GetEnemyPrefab()*/enemyPrefab, transform.position, Quaternion.identity);
            lastSpawn = Time.time;
        }
    }
    public override void DamageHoneycomb(float damage)
    {
        if (honeyGrid.health <= 0) return;
        honeyGrid.health -= damage;
        if(honeyGrid.health <= 0)
        {
            FindObjectOfType<LevelHandler>().BeeuildingDestroyed(transform.position);
            honeyGrid.HideHoneycomb();
            honeyGrid.SetDepth(0);
        }
    }

    public override void DestroyHoneycomb()
    {
        throw new System.NotImplementedException();
    }

    public override void HideHoneycomb()
    {
        for (int i = 1; i < TowerLayers.Length; i += 1)
        {
            TowerLayers[i].parent = TowerLayers[0];
            TowerLayers[i].localPosition = Vector2.zero;
        }
    }

    public void SetupBeeTower()
    {
        if(layers == null) layers = Map.StaticMap.HoneycombLayers;
        if(HiveCastle)
        {
            placeHoneycombLayers(0, transform);

        }
        else
        {
            for (int i = 0; i < TowerLayers.Length; i += 1)
            {
                TowerLayers[i].parent = layers[i];
                TowerLayers[i].localPosition = TowerLayers[0].localPosition * layers[i].localScale.x;
            }
        }
        
    }

    private void placeHoneycombLayers(int mapLayer, Transform honeycombLayer)
    {
        Debug.Log("Placing " + honeycombLayer.name);
        if(mapLayer > layers.Length - 1)
        {
            Debug.Log("Tower layers too high");
        }
        else
        {
            honeycombLayer.parent = layers[mapLayer];
            honeycombLayer.localPosition = honeycombLayer.localPosition * layers[mapLayer].localScale.x; //(TowerLayers[0].position - honeycombLayer.position) * layers[mapLayer].localScale.x;
            for (int i = honeycombLayer.childCount - 1; i > -1 ; i -= 1)
            {
                placeHoneycombLayers(mapLayer + 1, honeycombLayer.GetChild(i));
            }
        }
    }
}
