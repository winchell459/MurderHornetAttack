using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HonecombEnemyTrigger : MonoBehaviour
{
    //public HoneycombCell honeyGrid;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Instantiate(transform.parent.GetComponent<HoneycombCell>().honeyGrid.GetEnemyPrefab(), transform.position, Quaternion.identity);
            transform.parent.GetComponent<HoneycombCell>().honeyGrid.DestroyHoneycomb();
        }
    }
}
