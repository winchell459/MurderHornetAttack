using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HornetController : Insect
{
    public float ForwardSpeed = 3;
    public float SideSpeed = 1f;
    public Transform HornetSprite;
    private Rigidbody2D rb;
    public GameObject HornetPlasmPrefab;
    
    public enum ControlTypes
    {
        MouseControl,
        DirectKeyboard
    }
    public ControlTypes Controls;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //rb.velocity = new Vector2(0, ForwardSpeed);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Controls == ControlTypes.MouseControl)
        {
            //float mouseWorldXPos = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
            //Vector2 mousePosition = new Vector2(mouseWorldXPos, transform.position.y);

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //transform.position = Vector3.MoveTowards(transform.position, mousePosition, SideSpeed * Time.deltaTime);
            Vector2 deltaMove = mousePosition - (Vector2)transform.position;
            rb.AddForce(deltaMove * ForwardSpeed);
            transform.up = Vector2.MoveTowards(transform.up, deltaMove, SideSpeed * Time.deltaTime);
            //if (transform.position.x < mouseWorldXPos) HornetSprite.localScale = new Vector3(-1, 1, 1);
            //else if(transform.position.x > mouseWorldXPos) HornetSprite.localScale = new Vector3(1, 1, 1);
        }else if(Controls == ControlTypes.DirectKeyboard)
        {
            float v = Input.GetAxis("Vertical");
            float h = Input.GetAxis("Horizontal");

            //rb.AddForce(v * ForwardSpeed * transform.up);
            //Debug.Log(rb.rotation);
            //if (h != 0) rb.AddTorque(-h * SideSpeed);
            //else rb.rotation = rb.rotation;

            rb.velocity = ForwardSpeed * v * transform.up;
            transform.Rotate(new Vector3(0, 0, -h * SideSpeed));
        }



    }

    private void Update()
    {
        if (Controls == ControlTypes.MouseControl)
        {
            if (Input.GetMouseButtonDown(0))
            {
                GameObject plasm = Instantiate(HornetPlasmPrefab, transform.position, Quaternion.identity);
                plasm.GetComponent<Rigidbody2D>().velocity = 40 * (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            }
        }
        else if (Controls == ControlTypes.DirectKeyboard)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameObject plasm = Instantiate(HornetPlasmPrefab, transform.position, Quaternion.identity);
                plasm.GetComponent<Rigidbody2D>().velocity = 10 * transform.up;
            }
        }
        
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Honeycomb"))
        {
            Destroy(collision.gameObject);
            hornetMurdered();
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.transform.name);
        if (collision.transform.CompareTag("Enemy") && collision.transform.GetComponent<Insect>())
        {
            Insect collider = collision.transform.GetComponent<Insect>();
            
            Collision(collider.CollisionDamage);
            collider.Collision(GetComponent<Insect>().CollisionDamage);
        }
    }

    public override void Collision(float Damage)
    {
        Health -= Damage;
        if (Health <= 0)
        {
            hornetMurdered();
        }
    }

    private void hornetMurdered()
    {
        FindObjectOfType<LevelHandler>().UpdatePlayerStats(0, 1);
        Destroy(gameObject);
    }
}
