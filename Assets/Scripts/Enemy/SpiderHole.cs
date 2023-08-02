using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderHole : MonoBehaviour, IChunkObject
{
    public GameObject SpiderPrefab;
    public float SpawnCoolDown = 5f;
    private float lastSpawnTime = float.NegativeInfinity;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(lastSpawnTime + SpawnCoolDown < Time.fixedTime) 
            {
                Insect spider = Instantiate(SpiderPrefab, transform.position, Quaternion.identity).GetComponent<Insect>();
                spider.transform.up = transform.position - collision.transform.position;

                //Map.StaticMap.AddEnemyToChunk(spider);
                Map.StaticMap.AddTransientChunkObject(spider.GetComponent<IChunkObject>());

                lastSpawnTime = Time.fixedTime;
            } 
           

        }
    }

    void IChunkObject.Activate()
    {
        gameObject.SetActive(true);
    }

    void IChunkObject.Deactivate()
    {
        gameObject.SetActive(false);
    }

    GameObject IChunkObject.GameObject()
    {
        return gameObject;
    }
    
    void IChunkObject.SetMyChunk(MapChunk myChunk)
    {

    }
}
