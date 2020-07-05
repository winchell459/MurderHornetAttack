using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Honeycomb : MonoBehaviour
{
    public MapHoneycomb honeyGrid;

    public MapHoneycomb.LocationTypes LocationType;

    public abstract void DamageHoneycomb(float damage);
    public abstract void DestroyHoneycomb();
    public abstract void HideHoneycomb();

   
}
