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
            MapChunk chunk = Utility.GetMapChunk(insect.transform.position);
            chunk.AddEnemyToChunk(insect);
            insect.InsectPrefab = transform.parent.GetComponent<HoneycombCell>().honeyGrid.GetEnemyPrefab();
            transform.parent.GetComponent<HoneycombCell>().honeyGrid.DestroyHoneycomb();
        }
    }
}
