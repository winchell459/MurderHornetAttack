﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChamberAntFarmTrigger : ChamberTrigger
{
    public AntSquad antSquadPrefab;
    private AntSquad antSquad;
    public ChamberAntFarmTrigger PreviousNode;
    public bool Triggered = false;
    public MapPath AntPath;

    private bool inTrigger = false;

    private void Update()
    {
        if (PreviousNode)
        {
            Color lineColor = Color.blue;
            if (!PreviousNode.PreviousNode) lineColor = Color.green;

            Debug.DrawLine(PreviousNode.transform.position, transform.position, lineColor);

            if(inTrigger && Input.GetKeyDown(KeyCode.J))
            {
                if (LevelHandler.singleton.SpawnAntSqaud())
                {
                    StartMarch();
                    inTrigger = false;
                }
            }
        }
    }
    protected override void OnExit(Collider2D collision)
    {
        if (collision.GetComponent<HornetController>())
        {
            inTrigger = false;
        }
    }

    protected override void OnStay(Collider2D collision)
    {
        if (collision.GetComponent<HornetController>())
        {
            
            if(!antSquad || (!antSquad.Alive() && !PreviousNode))
            {
                if (PreviousNode && PreviousNode.Triggered)
                {
                    Triggered = true;
                    antSquad = PreviousNode.antSquad;
                    antSquad.AddMarchingPoints(getPathForSquad());
                }
                else if (!PreviousNode)
                {
                    Triggered = true;
                    StartMarch();
                }
            }
            
            if(Triggered && !antSquad)
            {
                inTrigger = true;
                Debug.Log($"Drop Royal Jelly?");
            }
        }
    }

    private void StartMarch()
    {
        if (!antSquad)
        {
            antSquad = Instantiate(antSquadPrefab, transform.position, Quaternion.identity);
            antSquad.SetMarchingPoints(getPathForSquad());
        }

        antSquad.StartMarch();
    }

    private Vector2[] getPathForSquad()
    {
        Vector2[] path = new Vector2[AntPath.Count];
        for(int i = 0; i < path.Length; i+= 1)
        {
            path[i] = AntPath.GetPoint(i);
        }
        return path;
    }

    
}
