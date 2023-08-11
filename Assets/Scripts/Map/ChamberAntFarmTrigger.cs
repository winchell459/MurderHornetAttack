using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChamberAntFarmTrigger : ChamberTrigger
{
    public AntSquad antSquadPrefab;
    private AntSquad antSquad;
    public ChamberAntFarmTrigger PreviousNode, NextNode;
    public bool Triggered = false;
    
    public MapPath AntPath;

    private bool inTrigger = false;

    [SerializeField] private GameObject spawnAntSquadButton;


    private void Update()
    {
        if (PreviousNode)
        {
            Color lineColor = Color.blue;
            if (!PreviousNode.PreviousNode) lineColor = Color.green;

            Debug.DrawLine(PreviousNode.transform.position, transform.position, lineColor);

            if(inTrigger)
            {
                spawnAntSquadButton.SetActive(true);
            }
            else
            {
                spawnAntSquadButton.SetActive(false);
            }
        }
        else
        {
            spawnAntSquadButton.SetActive(false);
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
                if (!NextNode)
                {

                }
                else if (PreviousNode /*&& PreviousNode.Triggered */&& PreviousNode.antSquad)
                {
                    Triggered = true;
                    
                    //antSquad = PreviousNode.antSquad;
                    //antSquad.AddMarchingPoints(getPathForSquad());

                    StartMarch(PreviousNode.antSquad);
                }
                else if (!PreviousNode)
                {
                    Triggered = true;
                    StartMarch();
                }
            }
            
            if(Triggered && antSquad && !antSquad.Alive() && antSquad.DeadWaiting)
            {
                inTrigger = true;
                //Debug.Log($"Drop Royal Jelly?");
            }
        }
    }

    private void StartMarch(AntSquad waitingForSquad)
    {
        if (!antSquad)
        {
            antSquad = Instantiate(antSquadPrefab, transform.position, Quaternion.identity);
            antSquad.SetMarchingPoints(getPathForSquad());
            antSquad.startMound = this;
            antSquad.StartMarch(waitingForSquad);
        }
        
    }

    private void StartMarch()
    {
        if (!antSquad && !PreviousNode)
        {
            antSquad = Instantiate(antSquadPrefab, transform.position, Quaternion.identity);
            antSquad.SetMarchingPoints(getPathForSquad());
            antSquad.startMound = this;
            antSquad.StartMarch(antSquad.AntNum);
        }
        else if(!PreviousNode)
        {
            antSquad.StartMarch(antSquad.DeadCount);
        }
        else
        {
            Debug.LogWarning("March not started Error");
        }

        
    }

    private void RestartMarch()
    {

    }

    public int GetMoundID()
    {
        return CountMound(0);
    }
    public int CountMound(int count)
    {
        if (!PreviousNode) return count;
        else return PreviousNode.CountMound(count + 1);
    }

    public AntSquad GetNextAntSquad(AntSquad current)
    {
        if (!NextNode) return current;                                                          // at end of path
        else if (antSquad && antSquad != current && !antSquad.DeadWaiting) return antSquad;     // found next completed path
        else if (!antSquad /*|| !NextNode*/) return null;                                       // next node not triggered
        else return NextNode.GetNextAntSquad(current);                                          // still looking for nextNode
    }

    public void SetAntSquad(AntSquad antSquad)
    {
        this.antSquad = antSquad;
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

    public void SpawnAntSquadButton()
    {
        if (LevelHandler.singleton.SpawnAntSqaud())
        {
            StartMarch();
            inTrigger = false;
        }
    }
}
