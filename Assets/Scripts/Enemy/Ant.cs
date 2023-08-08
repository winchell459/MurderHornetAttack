using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : InsectGroup
{
    public bool Marching = false;
    public float speed = 5f;

    public float PlayerDamage = 1f;

    public bool forwardMarch = true;

    private Rigidbody2D rb;

    public Vector2[] MarchingPoints;

    private float epsilon = 0.1f;

    public int CurrentPointIndex = 0;

    private Vector2 vector;
    private Vector2 position;

    [SerializeField] bool playerDamage = false;

    [SerializeField] bool digging;
    List<HoneycombPos> visitedHoneycomb = new List<HoneycombPos>();

    private AntSquad _mySquad;
    public AntSquad mySquad { get { return _mySquad; } set { _mySquad = value; } }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        position = new Vector2(transform.position.x, transform.position.y);
    }

    public bool homerunComplete = false;
    void FixedUpdate()
    {
        position.x = transform.position.x;
        position.y = transform.position.y;

        if (Marching)
        {
            if (Vector2.Distance(transform.position, MarchingPoints[CurrentPointIndex]) <= epsilon)
            {
                if (forwardMarch)
                {
                    CurrentPointIndex++;
                }
                else
                {
                    CurrentPointIndex--;
                }

                if (CurrentPointIndex < 0)
                {
                    if (mySquad.CheckCompletedSegments())
                    {
                        Debug.Log("-------------New segments added!---------------");
                    }
                    else
                    {
                        CurrentPointIndex = 0;
                        forwardMarch = true;
                        homerunComplete = true;
                    }
                    
                }
                else if (CurrentPointIndex >= MarchingPoints.Length)
                {
                    
                    CurrentPointIndex = MarchingPoints.Length - 1;
                    forwardMarch = false;
                    homerunComplete = true;

                }

                float angle = 0;

                Vector3 relative = transform.InverseTransformPoint(MarchingPoints[CurrentPointIndex]);
                angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
                transform.Rotate(0, 0, -angle);

            }
            vector = (MarchingPoints[CurrentPointIndex] - position).normalized;
            rb.MovePosition(rb.position + vector * speed * Time.fixedDeltaTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (Marching)
        //{
        //    float angle = 0;

        //    Vector3 relative = transform.InverseTransformPoint(MarchingPoints[CurrentPointIndex]);
        //    angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
        //    transform.Rotate(0, 0, -angle);
        //}
        if (digging)
        {
            HoneycombPos pos = Utility.Honeycomb.WorldPointToHoneycombGrid(transform.position);
            if (!visitedHoneycomb.Contains(pos))
            {
                MapHoneycomb honeycomb = Map.StaticMap.GetHoneycomb(pos);
                if(honeycomb.display && !honeycomb.isFloor)
                {
                    DamageHoneycomb(honeycomb.honeycomb.GetComponent<Honeycomb>());
                }
                visitedHoneycomb.Add(pos);
            }
        }
    }

    public void March(float MarchDelay)
    {
        StartCoroutine(StartMarch(MarchDelay));
    }

    private IEnumerator StartMarch(float MarchDelay)
    {
        // Delay before first damage
        yield return new WaitForSeconds(MarchDelay);
        Marching = true;
    }

    public void SetMarchingPoints(Vector2[] MarchingPoints)
    {
        this.MarchingPoints = MarchingPoints;
    }

    public void UpdateCurrentPointIndex()
    {
        int currentSegment = GetCurrentSegment(transform.position);
        CurrentPointIndex = forwardMarch ? currentSegment - 1 : currentSegment;
    }

    private int GetCurrentSegment(Vector2 currentPos)
    {
        
        for(int i = 1; i < MarchingPoints.Length; i++)
        {
            Vector2 inverseX = transform.InverseTransformPoint(MarchingPoints[i - 1]);
            Vector2 inverseY = transform.InverseTransformPoint(MarchingPoints[i]);
            float segmantDistance = Vector2.Distance(inverseX, inverseY);
            float xDistance = Vector2.Distance(inverseX, transform.InverseTransformPoint(transform.position));
            float yDistance = Vector2.Distance(inverseY, transform.InverseTransformPoint(transform.position));
            if (xDistance <= segmantDistance && yDistance <= segmantDistance) return i - 1;
        }

        Debug.LogWarning("Segment not found error.");
        return -1;
    }

   
    private void DamageHoneycomb(Honeycomb honeycomb)
    {
        honeycomb.DamageHoneycomb(float.PositiveInfinity, HoneycombTypes.Areas.Farm, HoneycombTypes.Variety.Path);
    }
    public override void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            HandleInsectGroupDeath();
            Destroy(gameObject);
        }
    }

    
    protected override bool CheckDespawn()
    {
        return _mySquad.CheckDespawn();
    }

    public override void Respawn()
    {
        _mySquad.Respawn();
    }

    public override void Despawn()
    {
        _mySquad.Despawn();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.tag;
        if (/*!digging && */tag.Equals("Honeycomb"))
        {
            DamageHoneycomb(collision.GetComponent<Honeycomb>());
        }
        else if (playerDamage && tag.Equals("Player"))
        {
            collision.GetComponent<HornetController>().TakeDamage(PlayerDamage);
        }
        else if (tag.Equals("Enemy") && collision.transform.GetComponent<Insect>())
        {
            Insect insect = collision.transform.GetComponent<Insect>();
            Vector2 velocity = rb.velocity;
            Vector2 collisionVelocity = insect.GetCollisionVelocity(transform, velocity);
            TakeDamage(insect.CollisionDamage, collisionVelocity);
            insect.TakeDamage(PlayerDamage);
        }

        //detect other ant squad and combine
        if (collision.GetComponent<Ant>())
        {
            Ant otherAnt = collision.GetComponent<Ant>();
            if (otherAnt.mySquad == mySquad) //check if squad had seperated
            {

            }
            else //combine ant squad
            {

            }
        }

    }
}
