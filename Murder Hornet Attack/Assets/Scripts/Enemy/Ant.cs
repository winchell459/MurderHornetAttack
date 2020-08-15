using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : Insect
{
    public bool Marching = false;
    public float speed = 5f;

    private Rigidbody2D rb;

    public Vector2[] MarchingPoints;

    private float epsilon = 0.1f;

    public int CurrentPointIndex = 0;

    private Vector2 velocity;
    private Vector2 position;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        position = new Vector2(transform.position.x, transform.position.y);
    }

    void FixedUpdate()
    {
        position.x = transform.position.x;
        position.y = transform.position.y;

        if (Marching)
        {
            if (Vector2.Distance(transform.position, MarchingPoints[CurrentPointIndex]) <= epsilon)
            {
                CurrentPointIndex++;
                CurrentPointIndex %= MarchingPoints.Length;
            }

            velocity = (MarchingPoints[CurrentPointIndex] - position).normalized;
            rb.MovePosition(rb.position + velocity * speed * Time.fixedDeltaTime);

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Marching)
        {
            float angle = 0;

            Vector3 relative = transform.InverseTransformPoint(MarchingPoints[CurrentPointIndex]);
            angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
            transform.Rotate(0, 0, -angle);

            //transform.up = rb.velocity.normalized;

            //if (Vector2.Distance(transform.position, MarchingPoints[CurrentPointIndex]) <= epsilon)
            //{
            //    CurrentPointIndex++;
            //    CurrentPointIndex %= MarchingPoints.Length;
            //}
            //float step = speed * Time.deltaTime;
            //transform.position = Vector2.MoveTowards(transform.position, MarchingPoints[CurrentPointIndex], step);


            //// Determine which direction to rotate towards
            //Vector3 targetDirection = MarchingPoints[CurrentPointIndex] - position;

            //// The step size is equal to speed times frame time.
            //float singleStep = speed * Time.deltaTime;

            //// Rotate the forward vector towards the target direction by one step
            //Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

            ////// Draw a ray pointing at our target in
            ////Debug.DrawRay(transform.position, newDirection, Color.red);

            //// Calculate a rotation a step closer to the target and applies rotation to this object
            //transform.rotation = Quaternion.LookRotation(newDirection);

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

    public override void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
