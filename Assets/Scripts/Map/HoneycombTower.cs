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
    private LevelHandler lh;

    private void Start()
    {
        lastSpawn = Time.time;
        if (HiveCastle) SetupHoneycomb();

    }

    private void Update()
    {
        
        if (!lh) lh = LevelHandler.singleton;
        if (!player) player = LevelHandler.singleton.Player ? LevelHandler.singleton.Player.gameObject.GetComponent<HornetController>() : null;
        if (lastSpawn + spawnRate < Time.time && player && Vector2.Distance(player.transform.position, transform.position) <= spawnDistance)
        {
            if (lh.HoneycombTowerSpawnEnemy(mapHoneycomb))
            {
                SpawnBee(player.gameObject);
            }
            
        }
    }
    public EnemyPhysics SpawnBee(GameObject target)
    {

        if (enemyPrefab)
        {
            GameObject spawnedInsect = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            Map.StaticMap.AddEnemyToChunk(spawnedInsect.GetComponent<Insect>());
            
            Debug.Log("HoneycombTower Attacks");
            lastSpawn = Time.time;
            spawnedInsect.GetComponent<EnemyPhysics>().SetTarget(target);
            return spawnedInsect.GetComponent<EnemyPhysics>();
        }
        return null;
    }
    public override void DamageHoneycomb(float damage)
    {
        if (mapHoneycomb.health <= 0) return;
        mapHoneycomb.health -= damage;
        Debug.Log($"beeuildingHealth: {mapHoneycomb.health}");
        if(mapHoneycomb.health <= 0)
        {
            FindObjectOfType<LevelHandler>().BeeuildingDestroyed(transform.position);
            mapHoneycomb.HideHoneycomb();
            mapHoneycomb.SetDepth(0);
            mapHoneycomb.display = false;
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

    //public void SetupBeeTower()
    public override void SetupHoneycomb()
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
