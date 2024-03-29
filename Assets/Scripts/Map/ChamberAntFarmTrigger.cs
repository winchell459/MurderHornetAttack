﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChamberAntFarmTrigger : ChamberTrigger
{
    public AntSquad antSquad;
    public ChamberAntFarmTrigger PreviousNode;
    public bool Triggered = false;
    public MapPath AntPath;

    protected override void OnExit(Collider2D collision)
    {
        
    }

    protected override void OnStay(Collider2D collision)
    {
        if (collision.GetComponent<HornetController>() && !Triggered)
        {
            if (PreviousNode && PreviousNode.Triggered)
            {
                Triggered = true;
                antSquad = PreviousNode.antSquad;
                antSquad.AddMarchingPoints(getPathForSquad());
            }
            else if(!PreviousNode)
            {
                Triggered = true;
                antSquad = Instantiate(antSquad, transform.position, Quaternion.identity);
                antSquad.SetMarchingPoints(getPathForSquad());
                antSquad.StartMarch();
            }
        }
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
