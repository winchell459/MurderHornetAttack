using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderHole : MonoBehaviour
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

                //set enemy in chunk for despawning when player leaves chunk
                MapChunk chunk = Utility.GetMapChunk(spider.transform.position);
                chunk.AddEnemyToChunk(spider);
                spider.InsectPrefab = transform.parent.GetComponent<HoneycombCell>().honeyGrid.GetEnemyPrefab();

                lastSpawnTime = Time.fixedTime;
            } 
           

        }
    }
}
