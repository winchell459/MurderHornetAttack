using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HornetPlasm : MonoBehaviour
{
    public float Damage = 1;
    private bool honeycombBounce;
    private int bounceCount = 3;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Honeycomb"))
        {
            //Destroy(collision.gameObject);
            if (honeycombBounce)
            {
                Vector2 velocity = GetComponent<Rigidbody2D>().velocity;
                Vector2 normal = collision.transform.position - transform.position;
            }
            else
            {
                //HoneycombPos hexPos = Utility.Honeycomb.WorldPointToHoneycombGrid(collision.GetComponent<Honeycomb>().mapHoneycomb.position);
                Debug.Log($"plasm hit: {collision.name}");
                collision.GetComponent<Honeycomb>().DamageHoneycomb(Damage);

                GameManager.TerrainHit();
                Destroy(gameObject);
            }
           
        }

        if (collision.transform.CompareTag("Enemy") && collision.transform.GetComponent<Insect>())
        {
            Insect collider = collision.transform.GetComponent<Insect>();
            GameManager.EnemyHit(collider.type);
            
            collider.TakeDamage(Damage, GetComponent<Rigidbody2D>().velocity);
            FindObjectOfType<LevelHandler>().UpdatePlayerStats(1, 0);
            Destroy(gameObject);
        }

       
    }

    public static void FirePlasma(GameObject HornetPlasmPrefab, Vector2 location, Vector2 velocity, float power)
    {
        GameObject plasm = Instantiate(HornetPlasmPrefab, location, Quaternion.identity);
        plasm.GetComponent<Rigidbody2D>().velocity = velocity;
        plasm.GetComponent<HornetPlasm>().Damage = power;
        GameManager.ShotFired();
        Destroy(plasm, 15);
    }


}
