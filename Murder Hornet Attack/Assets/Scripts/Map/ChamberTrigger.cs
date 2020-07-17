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
            bool PlayerExit = true;
            foreach (Collider2D collider in GetComponents<Collider2D>())
            {
                if (collider.IsTouching(collision)) PlayerExit = false;
            }
            if (PlayerExit)
            {
                inChamber = false;
                OnExit(collision);
            }
            
        }
    }
    public static ChamberTrigger SetupChamberTrigger(GameObject prefab, MapChamber chamber)
    {
        ChamberTrigger trigger = Instantiate(prefab, chamber.Location, Quaternion.identity).GetComponent<ChamberTrigger>();
        return (trigger).Setup(chamber);
    }

    protected abstract void OnStay(Collider2D collision);
    protected abstract void OnExit(Collider2D collision);
    protected ChamberTrigger Setup(MapChamber chamber)
    {
        Chamber = chamber;

        //gameObject.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        //gameObject.AddComponent<CompositeCollider2D>().isTrigger = true;
        //GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;

        foreach (Vector2 loc in chamber.locations)
        {
            gameObject.AddComponent<CircleCollider2D>();//.usedByComposite = true ;

        }
        CircleCollider2D[] colliders = gameObject.GetComponents<CircleCollider2D>();
        for (int i = 0; i < colliders.Length; i += 1)
        {
            CircleCollider2D collider = colliders[i];
            collider.isTrigger = true;
            collider.radius = chamber.widths[i] / 2;
            collider.offset = chamber.locations[i] - (Vector2)transform.position;

            GameObject circle = Instantiate(CirclePrefab, chamber.locations[i], Quaternion.identity);
            circle.transform.parent = transform;
            circle.transform.localScale = new Vector2(chamber.widths[i], chamber.widths[i]);
            circle.GetComponent<SpriteRenderer>().color = Color.black;
            //Debug.Log(chamber.locations[i] + " " + chamber.widths[i]);
        }
        
        return this;
    }
}
