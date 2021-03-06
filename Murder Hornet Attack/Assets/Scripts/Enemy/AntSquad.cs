﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntSquad : MonoBehaviour
{
    public int AntNum = 0;
    public float speed = 5f;
    public float MarchDelay = 0.1f;

    public GameObject[] Squad;

    public GameObject AntPrefab;

    public float Radius = 5f;

    public int NumPoints = 5;
    public Vector2[] MarchingPoints;


    // Start is called before the first frame update
    void Start()
    {
        if (NumPoints <= 1)
        {
            NumPoints = 2;
        }

        if (AntNum <= 0)
        {
            AntNum = 1;
        }

        MarchingPoints = new Vector2[NumPoints];
        for (int i = 0; i < NumPoints; i++)
        {
            float x = Random.Range(Radius * -1, Radius) + transform.position.x;
            float y = Random.Range(Radius * -1, Radius) + transform.position.y;
            MarchingPoints[i] = new Vector2(x, y);
        }

        Squad = new GameObject[AntNum];
        for(int i = 0; i < AntNum; i++)
        {
            Squad[i] = Instantiate(AntPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            Squad[i].transform.parent = this.transform;
            Ant ant = Squad[i].GetComponent<Ant>();
            ant.transform.position = MarchingPoints[0];
            ant.SetMarchingPoints(MarchingPoints);
            ant.March(MarchDelay * i);
            ant.speed = speed;

        }

    }

    // Update is called once per frame
    void Update()
    {
        if(transform.childCount == 0)
        {
            //
            //  Create the red ant object to chase player
            //
            Destroy(gameObject);
        }
    }
}
