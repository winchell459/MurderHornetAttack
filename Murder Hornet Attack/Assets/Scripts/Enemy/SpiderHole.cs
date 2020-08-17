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
                GameObject spider = Instantiate(SpiderPrefab, transform.position, Quaternion.identity);
                spider.transform.up = transform.position - collision.transform.position;

                lastSpawnTime = Time.fixedTime;
            } 
           

        }
    }
}
