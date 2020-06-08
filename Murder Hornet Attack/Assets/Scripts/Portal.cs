using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public bool inPortal;
    public GameObject CirclePrefab;
    public MapChamber Chamber;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<HornetController>())
        {
            inPortal = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<HornetController>())
        {
            inPortal = false;
        }
    }
}
