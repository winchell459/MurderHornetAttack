using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HornetController : Insect
{
    public float ForwardSpeed_Defualt = 2;
    public float ForwardSpeed = 2;
    public float SideSpeed = 1f;
    public float SideSpeed_Defualt = 1f;
    public Transform HornetSprite;
    private Rigidbody2D rb;
    public GameObject HornetPlasmPrefab;

    public int ShotCount;
    
    
    public enum ControlTypes
    {
        MouseControl,
        DirectKeyboard
    }
    public ControlTypes Controls;

    private PlayerHandler ph;

    public bool MobileControls;
    public bool ExitButtonPressed;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //rb.velocity = new Vector2(0, ForwardSpeed);
#if UNITY_IOS
        MobileControls = true;
#endif
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
        }else if(Controls == ControlTypes.DirectKeyboard && !MobileControls)
        {
            float v = Input.GetAxis("Vertical");
            float h = Input.GetAxis("Horizontal");
            
            MotionControl(v, h);
        }



    }
    private Vector2 CollisionVelocity = Vector2.zero;

    public void MotionControl(float vertical, float horizontal)
    {
        if (hadCollision && collisionControlTime + collisionTime > Time.fixedTime)
        {
            rb.velocity = CollisionVelocity;
            //Debug.Log("CollisionVelocity: " + CollisionVelocity);
            CollisionVelocity = Vector2.zero;
        } else if (hadCollision && collisionControlTime + collisionTime < Time.fixedTime)
        {
            hadCollision = false;
        }
        else
        {
            rb.velocity = ForwardSpeed * vertical * transform.up;
            hadCollision = false;
        }
        
        
        transform.Rotate(new Vector3(0, 0, -horizontal * SideSpeed));
    }

    public void FirePlasma()
    {

        if(ShotCount > 0)
        {
            if (!ph) ph = FindObjectOfType<PlayerHandler>();
            HornetPlasm.FirePlasma(HornetPlasmPrefab, transform.position, 10 * transform.up, ph.GetPlasmaPower());

            ShotCount -= 1;

            FindObjectOfType<LevelHandler>().UpdatePlayerStats();
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
                //GameObject plasm = Instantiate(HornetPlasmPrefab, transform.position, Quaternion.identity);
                //plasm.GetComponent<Rigidbody2D>().velocity = 10 * transform.up;
                FirePlasma();
            }

            if (!MobileControls)
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    ExitButtonPressed = true;
                }
                else
                {
                    ExitButtonPressed = false;
                }
            }
            
        }
        
    }


    public bool TakeHoneycombDamage = true;
    public float ImpulseCoefficient = 3;
    private bool hadCollision;
    private float collisionTime;
    private float collisionControlTime = 0.2f;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Honeycomb"))
        {
            
            //hornetMurdered();
            if(!TakeHoneycombDamage) TakeDamage(Health);
            else //if (!hadCollision)
            {
                //TakeDamage(5);
                Vector2 impulse = ImpulseCoefficient * (transform.position - collision.transform.position).normalized;
                //CollisionVelocity += impulse;
                TakeDamage(5, impulse);
                
            }
            
            Destroy(collision.gameObject);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.transform.name);
        if (collision.transform.CompareTag("Enemy") && collision.transform.GetComponent<Insect>())
        {
            Insect collider = collision.transform.GetComponent<Insect>();
            
            TakeDamage(collider.CollisionDamage, collision.GetComponent<Insect>().GetCollisionVelocity(transform, rb.velocity));
            collider.TakeDamage(GetComponent<Insect>().CollisionDamage);
        }
    }

    public override void TakeDamage(float Damage)
    {
        Health -= Damage;
        FindObjectOfType<LevelHandler>().UpdatePlayerStats();
        if (Health <= 0)
        {
            hornetMurdered();
        }
    }

    public override void TakeDamage(float Damage, Vector2 KickBackVelocity)
    {
        if (!hadCollision)
        {
            collisionTime = Time.fixedTime;
            hadCollision = true;
            CollisionVelocity += KickBackVelocity;
            Debug.Log("KickBackVelocity: " + KickBackVelocity);
        }
        
        TakeDamage(Damage);
    }

    public void AddedHealth(float Healing)
    {
        if (!ph) ph = FindObjectOfType<PlayerHandler>();
        Health = Mathf.Clamp(Health + Healing, 0, ph.GetMaxHealth());
        FindObjectOfType<LevelHandler>().UpdatePlayerStats();
    }

    public void ResetSpeed()
    {
        ForwardSpeed = ForwardSpeed_Defualt;
        SideSpeed = SideSpeed_Defualt;
    }

    private void hornetMurdered()
    {
        FindObjectOfType<LevelHandler>().UpdatePlayerStats(0, 1);
        Destroy(gameObject);
    }
}
