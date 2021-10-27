using UnityEngine;

public class EnemyPhysicsBoundry : MonoBehaviour
{

    public float repelForce = 5;
    public Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponentInParent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            //Debug.Log("Bee's near by!");
            Vector3 otherPosition = collision.transform.position;

            // This gives you the vector away from the other
            Vector2 heading = (transform.position - otherPosition);
            heading.Normalize();
            heading *= repelForce;
            rb.AddForce(heading);
        }
    }
}


