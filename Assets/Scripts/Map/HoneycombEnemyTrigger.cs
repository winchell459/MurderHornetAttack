using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoneycombEnemyTrigger : MonoBehaviour
{
    //public HoneycombCell honeyGrid;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            IChunkObject insect = Instantiate(transform.parent.GetComponent<HoneycombCell>().mapHoneycomb.GetEnemyPrefab(), transform.position, Quaternion.identity).GetComponent<IChunkObject>();

            //Map.StaticMap.AddEnemyToChunk(insect);
            Map.StaticMap.AddTransientChunkObject(insect);

            transform.parent.GetComponent<HoneycombCell>().mapHoneycomb.DestroyHoneycomb();
        }
    }
}
