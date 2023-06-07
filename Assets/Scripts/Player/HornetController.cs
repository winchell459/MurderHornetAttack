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



    ControlParameters cp;
    private PlayerHandler ph;

    public bool MobileControls;
    public bool ExitButtonPressed;

    public Retical retical;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ph = FindObjectOfType<PlayerHandler>();
        cp = FindObjectOfType<ControlParameters>();
        //rb.velocity = new Vector2(0, ForwardSpeed);
#if UNITY_EDITOR
        MobileControls = false;
#elif UNITY_IOS
        MobileControls = true;
#else
        SetMousePosition();
        MobileControls = false;
#endif

        if (!MobileControls) GetComponent<MobileController>().enabled = false;

    }
    //Vector2 mouseScreenPosition;
    float mousePosition;
    private void SetMousePosition() { mousePosition = /*Camera.main.ScreenToWorldPoint(*/Input.GetAxis("Mouse X")/*)*/; }

    private void OnDestroy()
    {
        if (ph.Controls == PlayerHandler.ControlTypes.MouseControl) Cursor.lockState = CursorLockMode.None;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(ph.Controls == PlayerHandler.ControlTypes.MouseControl)
        {
            MouseMovementControl();
        }
        else if(ph.Controls == PlayerHandler.ControlTypes.DirectKeyboard && !MobileControls)
        {
            float v = Input.GetAxis("Vertical");
            float h = Input.GetAxis("Horizontal");

            if (cp.InverseReverse && v < 0) MotionControl(v * cp.FlySensitivity, -h * cp.TurnSensitivity);
            else MotionControl(v * cp.FlySensitivity, h * cp.TurnSensitivity);
            //MotionControl(v, h);
        }

    }

    private void Update()
    {

        //Debug.Log($"Mouse X: {Input.GetAxis("Mouse X")}");
        if (ph.Controls == PlayerHandler.ControlTypes.MouseControl)
        {
            if (LevelHandler.singleton.paused)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                //MouseMovementControl();
                if (Input.GetMouseButtonDown(0))
                {
                    FirePlasma();
                }
            }

        }
        else if (ph.Controls == PlayerHandler.ControlTypes.DirectKeyboard)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                FirePlasma();
            }

        }

        if (!MobileControls)
        {
            if (Input.GetKey(KeyCode.E))
            {
                ExitButtonPressed = true;
            }
            else
            {
                ExitButtonPressed = false;
            }
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
            HandleFiring();
        }
        else
        {
            HandleEmptyFiring();
        }

    }
    private void HandleFiring()
    {
        if (!ph) ph = FindObjectOfType<PlayerHandler>();
        HornetPlasm.FirePlasma(HornetPlasmPrefab, transform.position, 10 * transform.up, ph.GetPlasmaPower());
        AudioHandler.singleton.PlayClip(AudioHandler.FXClipName.fire);
        ShotCount -= 1;
        LevelHandler.singleton.UpdatePlayerStats();
    }

    private void HandleEmptyFiring()
    {
        AudioHandler.singleton.PlayClip(AudioHandler.FXClipName.empty);
    }

    public void PlasmaCharged()
    {

    }

    private void MouseMovementControl()
    {
        bool rotation = true;
        float sensitivity = 1.1f;
        //float mouseWorldXPos = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        //Vector2 mousePosition = new Vector2(mouseWorldXPos, transform.position.y);

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);


        //transform.position = Vector3.MoveTowards(transform.position, mousePosition, SideSpeed * Time.deltaTime);
        Vector2 deltaMove = mousePosition - (Vector2)transform.position;
        //Debug.Log($"deltaMove = mousePosition - (Vector2)transform.position : {mousePosition - (Vector2)transform.position}");
        if (rotation)
        {
            Cursor.lockState = CursorLockMode.Locked;
            float deltaX = Input.GetAxis("Mouse X");
            //Debug.Log($"Mouse X: {Input.GetAxis("Mouse X")}");

            float rotate = /*Mathf.Sign(deltaX) * deltaX **/ deltaX * sensitivity ;
            //Debug.Log($"rotate: {rotate}");
            //MotionControl(Input.GetAxis("Vertical"), rotate);
            float v = Input.GetAxis("Vertical");
            if (cp.InverseReverse && v < 0) MotionControl(v * cp.FlySensitivity, -rotate * cp.TurnSensitivity);
            else MotionControl(v * cp.FlySensitivity, rotate * cp.TurnSensitivity);
        }
        else
        {
            rb.AddForce(deltaMove * ForwardSpeed);
            transform.up = Vector2.MoveTowards(transform.up, deltaMove, SideSpeed * Time.deltaTime);
        }


        SetMousePosition();
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

            //Destroy(collision.gameObject);
            collision.transform.GetComponent<Honeycomb>().DestroyHoneycomb();
        }else if (collision.transform.GetComponent<QueenController>())
        {
            Insect collider = collision.transform.GetComponent<Insect>();

            TakeDamage(collider.CollisionDamage, collision.transform.GetComponent<Insect>().GetCollisionVelocity(transform, rb.velocity));
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
        LevelHandler.singleton.UpdatePlayerStats();
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
            //Debug.Log("KickBackVelocity: " + KickBackVelocity);
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
