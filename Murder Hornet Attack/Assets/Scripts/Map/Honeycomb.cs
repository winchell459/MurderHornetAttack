using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Honeycomb : MonoBehaviour
{
    public MapHoneycomb honeyGrid;

    public abstract void DestroyHoneycomb();
    public abstract void HideHoneycomb();

   
}
