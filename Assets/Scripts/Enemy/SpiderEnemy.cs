using System.Collections;
using UnityEngine;

public class SpiderEnemy : Insect
{
    public float speed = 1f;
    public float attackRadius = 2.0f;

    public Vector3 playerLocation;
    public float playerDistance;

    public bool dieOnCollision = false;

    public Rigidbody2D rb;
    public GameObject Projectile;
    public float ProjectileForce = 5f;

    public float AttackCoolDown = 2f;

    private void OnEnable()
    {
        if (IsWaiting)
        {
            Debug.LogWarning("Spider was stuck waiting.");
            IsWaiting = false;
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
        UpdatePlayerLocation();
        UpdateAngle();
    }

    public enum State
    {
        Move, Rotate, Fire, Attacking
    }

    public State CurrentState = State.Move;
    public float WaitTimeStates = 0.5f;

    public void UpdateState()
    {
        State passState = CurrentState;
        if (InAttackRange())
        {
            CurrentState = State.Fire;
        }

        else if (InAttackAngle())
        {
            if (InAttackRange())
            {
                CurrentState = State.Fire;
            }
            else
            {
                CurrentState = State.Move;
            }
        }
        else if(!InAngleRange())
        {
            CurrentState = State.Rotate;
        }
        if (passState != CurrentState)
        {
            StartCoroutine(Wait(WaitTimeStates));
        }
    }

    public float DegreesPerSecond = 360f;

    public bool Clockwise = true;
    public float AngleToPlayer = 0f;
    public float AttackAngle = 3f;
    public float TargetAngle = 25f;

    public float VulnerableAngle = 0.5f; // 1 is direct hit behind, 0 is side hit, negative value is forward

    private void UpdateAngle()
    {
        Vector2 targetDir = playerLocation - transform.position;
        targetDir = targetDir.normalized;
        AngleToPlayer = Vector2.Angle(targetDir, transform.up);
        Clockwise = Vector3.Cross(transform.up, targetDir).z > 0;
    }

    private void UpdatePlayerLocation()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player)
        {
            playerLocation = GameObject.FindWithTag("Player").transform.position;
            playerDistance = Vector2.Distance(playerLocation, transform.position);
        }
        

    }

    private bool InAngleRange()
    {
        return AngleToPlayer <= TargetAngle;
    }

    private bool InAttackAngle()
    {
        return AngleToPlayer <= AttackAngle;
    }

    private bool InAttackRange()
    {
        return playerDistance <= attackRadius;
    }

    public float fireAngle = 1f;

    private bool InFireAngle()
    {
        return AngleToPlayer >= 180 - fireAngle;
    }

    private void FixedUpdate()
    {
        if (IsWaiting)
        {
            return;
        }

        if(CurrentState == State.Rotate)
        {
            FixedRotate();
        }
        else if(CurrentState == State.Fire)
        {
            Attack();
        }
        else
        {
            Move();
        }

        UpdateState();
    }

    public bool IsWaiting = false;

    IEnumerator Wait(float waittime)
    {
        IsWaiting = true;
        yield return new WaitForSecondsRealtime(waittime);
        IsWaiting = false;
    }

    private void Move()
    {
        rb.MovePosition(transform.position + transform.up * Time.fixedDeltaTime * speed);
        if (playerDistance <= attackRadius || AngleToPlayer >= AttackAngle)
        {
            rb.velocity = Vector2.zero;
        }
    }


    private void Attack()
    {
        // 180 rotate
        if (!InFireAngle())
        {
            float rotationAngle = Time.fixedDeltaTime * DegreesPerSecond;
            rotationAngle = rotationAngle < AngleToPlayer ? rotationAngle : AngleToPlayer;
            rotationAngle = Clockwise ? rotationAngle : -1 * rotationAngle;

            rb.MoveRotation(rb.rotation - rotationAngle);
        }
        // Fire!
        else 
        {
            GameObject web = GameObject.Instantiate(Projectile);
            web.transform.position = transform.position;
            web.GetComponent<Rigidbody2D>().AddForce(transform.up * -100f);
            StartCoroutine(Wait(AttackCoolDown));
        }
    }

    private void FixedRotate()
    {
        float rotationAngle = Time.fixedDeltaTime * DegreesPerSecond;
        rotationAngle = rotationAngle < AngleToPlayer ? rotationAngle : AngleToPlayer;
        rotationAngle = Clockwise ? rotationAngle : -1 * rotationAngle;

        rb.MoveRotation(rb.rotation + rotationAngle);
    }

    public override void TakeDamage(float Damage)
    {
        Health -= Damage;
        if (Health <= 0)
        {
            FindObjectOfType<LevelHandler>().EnemyDeath(gameObject);
            GameManager.SpiderKilled();
            Destroy(gameObject);
        }
    }

    public override void TakeDamage(float Damage, Vector2 KickBackVelocity)
    {
        Debug.Log($"spider attacked angle: {Vector2.Dot(transform.up, KickBackVelocity.normalized)}");
        if (Vector2.Dot(transform.up, KickBackVelocity.normalized) > VulnerableAngle) Damage *= 5;
        TakeDamage(Damage);
    }
}
