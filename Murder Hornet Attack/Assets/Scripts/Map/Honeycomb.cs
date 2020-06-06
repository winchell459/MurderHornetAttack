using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Honeycomb : MonoBehaviour
{
    public MapHoneycomb honeyGrid;

    public void DestroyHoneycomb()
    {
        honeyGrid.DestroyHoneycomb();
        //honeyGrid.display = false;
        //Map.ReturnHoneycomb(gameObject);
        //gameObject.SetActive(false);
    }
}
