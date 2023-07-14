using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerPickup : MonoBehaviour
{
    public int flowerID;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            FindObjectOfType<LevelHandler>().CollectionsUpdated(0, 1, 0);
            FindObjectOfType<UIHandler>().FlowerPickup(flowerID);
            //FindObjectOfType<PlayerHandler>().flowersFound += 1;
            MiniMap.singleton.DisplayFlower(flowerID, false);
            Destroy(gameObject);
        }
    }
}
