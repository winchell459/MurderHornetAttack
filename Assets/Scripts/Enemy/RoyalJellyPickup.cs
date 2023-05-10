using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoyalJellyPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            FindObjectOfType<LevelHandler>().CollectionsUpdated(0, 0, 1);
            Destroy(gameObject);
        }
    }
}
