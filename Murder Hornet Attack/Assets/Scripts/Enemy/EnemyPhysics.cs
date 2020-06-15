using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPhysics : MonoBehaviour
{
    public float speed = 1f;
    public float attackSpeed = 1.0f;
    public float attackRadius = 2.0f;

    public float moveRadius = 5.0f;

    public float resetRadius = 1.0f;


    public float maxSpeed = 10f;

    public Vector3 playerLocation;
    public float playerDistance;

    public bool dieOnCollision = false;

    private Rigidbody2D rb;


    private bool _enterAttackRadius = false;

    public bool EnterAttackRadius
    {
        get { return _enterAttackRadius; }

        set
        {
            if (_enterAttackRadius != value)
            {
                _enterAttackRadius = value;
                if (value)
                {
                    // Entered the radius
                    Debug.Log("Entered the radius");
                    rb.velocity = Vector2.zero;
                }
                else
                {
                    // Exit the radius
                    Debug.Log("Exited the radius");

                }
            }
        }
    }

    private bool _enterResetRadius = false;

    public bool EnterResetRadius
    {
        get { return _enterResetRadius; }

        set
        {
            if (_enterResetRadius != value)
            {
                _enterResetRadius = value;
                if (value)
                {
                    // Entered the radius
                    Debug.Log("Entered the reset radius");
                }
                else
                {
                    // Exit the radius
                    Debug.Log("Exited the reset radius");
                    rb.velocity = Vector2.zero;
                }
            }
        }
    }

    private bool _enterMoveRadius = false;

    public bool EnterMoveRadius
    {
        get { return _enterMoveRadius; }

        set
        {
            if (_enterMoveRadius != value)
            {
                _enterMoveRadius = value;
                if (value)
                {
                    // Entered the radius
                    Debug.Log("Entered the Move radius");
                    rb.velocity = Vector2.zero;
                }
                else
                {
                    // Exit the radius
                    Debug.Log("Exited the Move radius");
                    rb.velocity = Vector2.zero;
                }
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GetComponent<CircleCollider2D>().isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        MoveToPlayer();
        RadiusControls();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

    }

    private void RadiusControls()
    {
        if(playerDistance < moveRadius)
        {
            EnterMoveRadius = true;
            if (playerDistance < attackRadius)
            {
                EnterAttackRadius = true;
                if (playerDistance < resetRadius)
                {
                    EnterResetRadius = true;
                }
                else
                {
                    EnterResetRadius = false;
                }
            }
            else
            {
                EnterAttackRadius = false;
                EnterResetRadius = false;
            }
        }
        else
        {
            EnterMoveRadius = false;
            EnterAttackRadius = false;
            EnterResetRadius = false;
        }
    }

    private void MoveToPlayer()
    {
        playerLocation = GameObject.FindWithTag("Player").transform.position;
        playerDistance = Vector2.Distance(playerLocation, transform.position);
        

        if (playerDistance <= int.MaxValue)
        {
            Vector2 heading = (playerLocation - transform.position);
            heading.Normalize();

            if (playerDistance <= attackRadius)
            {


                heading *= attackSpeed;
                rb.AddForce(heading);
            }
            else
            {

                heading *= speed;
                rb.AddForce(heading);
                rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxSpeed);
            }
        }
    }


    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, moveRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        Gizmos.color = Color.blue;
        Vector3 velocity = new Vector3(rb.velocity.x, rb.velocity.y);
        Gizmos.DrawLine(transform.position, transform.position + velocity);
    }
}
