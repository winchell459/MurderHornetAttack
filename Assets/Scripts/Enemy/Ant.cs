﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ant : Insect
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
                    CurrentPointIndex = 0;
                    forwardMarch = true;
                }
                else if (CurrentPointIndex >= MarchingPoints.Length)
                {
                    CurrentPointIndex = MarchingPoints.Length - 1;
                    forwardMarch = false;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.tag;
        if (tag.Equals("Honeycomb"))
        {
            collision.GetComponent<Honeycomb>().DamageHoneycomb(float.PositiveInfinity);
        }
        else if (tag.Equals("Player"))
        {
            collision.GetComponent<HornetController>().TakeDamage(PlayerDamage);
        }

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
