using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChamberTrigger : MonoBehaviour
{
    public bool inChamber;
    public GameObject CirclePrefab;
    public MapChamber Chamber;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<HornetController>())
        {
            inChamber = true;
            OnStay(collision);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<HornetController>())
        {
            inChamber = false;
            OnExit(collision);
        }
    }

    protected abstract void OnStay(Collider2D collision);
    protected abstract void OnExit(Collider2D collision);
}
