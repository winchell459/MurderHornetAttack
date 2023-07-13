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
            Insect insect = Instantiate(transform.parent.GetComponent<HoneycombCell>().honeyGrid.GetEnemyPrefab(), transform.position, Quaternion.identity).GetComponent<Insect>();

            //set enemy in chunk for despawning when player leaves chunk
            //MapChunk chunk = Utility.Honeycomb.GetMapChunk(insect.transform.position);
            //chunk.AddEnemyToChunk(insect);
            //insect.InsectPrefab = transform.parent.GetComponent<HoneycombCell>().honeyGrid.GetEnemyPrefab();

            Map.StaticMap.AddEnemyToChunk(insect);

            transform.parent.GetComponent<HoneycombCell>().honeyGrid.DestroyHoneycomb();
        }
    }
}
