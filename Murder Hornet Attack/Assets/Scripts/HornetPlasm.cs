using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HornetPlasm : MonoBehaviour
{
    public float Damage = 1;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Honeycomb"))
        {
            //Destroy(collision.gameObject);
            collision.GetComponent<Honeycomb>().DestroyHoneycomb();
            Destroy(gameObject);
        }

        if (collision.transform.CompareTag("Enemy") && collision.transform.GetComponent<Insect>())
        {
            Insect collider = collision.transform.GetComponent<Insect>();

            
            collider.Collision(Damage);
        }
    }
}
