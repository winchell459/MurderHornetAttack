using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntSquad : MonoBehaviour
{
    public int AntNum = 0;
    public float speed = 5f;
    public float MarchDelay = 0.1f;

    public List<Ant> Squad = new List<Ant>();
    public ChamberAntFarmTrigger startMound;

    public GameObject AntPrefab;

    public float Radius = 5f;

    public int NumPoints = 5;
    public Vector2[] MarchingPoints;

    private bool marchStarted;
    public bool RandomMarch = false;

    public AntSquad waitingForSquad;

    private int deadCount = 0;
    public bool DeadWaiting { get { return deadCount > 0; } }
    public int DeadCount { get { return deadCount; } }

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

        if (RandomMarch)
        {
            MarchingPoints = getRandomMarchingPoints();
            StartMarch(AntNum);
        }
        

        

    }

    // Update is called once per frame
    void Update()
    {
        if(marchStarted && transform.childCount == 0 && !waitingForSquad && deadCount == 0)
        {
            //
            //  Create the red ant object to chase player
            //
            Debug.Log("AntSquad Destroyed");
            Destroy(gameObject);
        }

        for(int i = 1; i < MarchingPoints.Length; i++)
        {

            Debug.DrawLine(MarchingPoints[i - 1], MarchingPoints[i], Color.red);
        }
        
    }

    public void StartMarch(AntSquad waitingForSquad)
    {
        marchStarted = true;
        this.waitingForSquad = waitingForSquad;
    }

    public void StartMarch(int AntNum) 
    {
        marchStarted = true;
        Squad = new List<Ant>();
        for (int i = 0; i < AntNum; i++)
        {
            Squad.Add(Instantiate(AntPrefab, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<Ant>());
            Squad[i].transform.parent = this.transform;
            Ant ant = Squad[i].GetComponent<Ant>();
            ant.transform.position = MarchingPoints[0];
            ant.SetMarchingPoints(MarchingPoints);
            ant.March(MarchDelay * i);
            ant.speed = speed;

            Squad[i].mySquad = this;
        }

        for (int i = 0; i < AntNum; i++)
        {
            Map.StaticMap.AddTransientChunkObject(Squad[i]);
        }

        deadCount = 0;
    }
    public bool CheckCompletedSegments()
    {
        if(startMound.GetMoundID() == 0)
        {
            AntSquad nextSquad = startMound.GetNextAntSquad(this);
            if (nextSquad)
            {
                //merge squad
                MergeAntSquad(nextSquad);
                return true;
            }
        }
        return false;
    }
    public bool CheckCompletedSegments(Ant ant)
    {
        AntSquad nextSquad = startMound.GetNextAntSquad(this);
        if (nextSquad)
        {
            if(nextSquad == this)
            {
                ant.myState = Ant.States.exiting;
            }
            else
            {
                nextSquad.AddAnt(ant);
                RemoveAnt(ant);
            }
            
            return true;
        }
        return false;
    }

    // add another AntSquad to this AntSquad
    public void MergeAntSquad(AntSquad toAddSquad)
    {
        AddMarchingPoints(toAddSquad.MarchingPoints);
        for (int i = toAddSquad.Squad.Count - 1; i >= 0; i--)
        {
            Ant ant = toAddSquad.Squad[i];
            if (ant)
            {
                AddAnt(ant);
                toAddSquad.RemoveAnt(ant);
                
            }
        }
        toAddSquad.startMound.SetAntSquad(this);
    }
    

    public void AddAnt(Ant ant)
    {
        if(!Squad.Contains(ant))Squad.Add(ant);
        ant.transform.parent = transform;
        ant.MarchingPoints = MarchingPoints;
        ant.mySquad = this;
        ant.UpdateCurrentPointIndex();
    }

    public void RemoveAnt(Ant ant)
    {
        Squad.Remove(ant);
    }

    private Vector2[] getRandomMarchingPoints()
    {
        Vector2[] MarchingPoints = new Vector2[NumPoints];
        for (int i = 0; i < NumPoints; i++)
        {
            float x = Random.Range(Radius * -1, Radius) + transform.position.x;
            float y = Random.Range(Radius * -1, Radius) + transform.position.y;
            MarchingPoints[i] = new Vector2(x, y);
        }
        return MarchingPoints;
    }

    public void SetMarchingPoints(Vector2[] marchingPoints)
    {
        MarchingPoints = marchingPoints;
    }

    public void AddMarchingPoints(Vector2[] newPoints)
    {
        Vector2[] newMarchingPoints = new Vector2[MarchingPoints.Length + newPoints.Length];
        for (int i = 0; i < MarchingPoints.Length; i += 1)
        {
            newMarchingPoints[i + newPoints.Length] = MarchingPoints[i];
        }
        for (int i = 0; i < newPoints.Length; i += 1)
        {
            newMarchingPoints[i /* + MarchingPoints.Length*/] = newPoints[i];
        }

        MarchingPoints = newMarchingPoints;
        foreach (Ant ant in Squad)
        {
            if (ant)
            {
                //Ant ant = antObj.GetComponent<Ant>();
                //ant.March(1);
                ant.MarchingPoints = MarchingPoints;
                ant.CurrentPointIndex = ant.CurrentPointIndex + newPoints.Length;
            }
        }
    }

    public bool Alive()
    {
        foreach (Ant ant in Squad) if (ant) return true;
        return false;
    }

    public bool CheckDespawn()
    {
        foreach(Ant ant in Squad)
        {
            if (ant && (!ant.homerunComplete || Utility.Honeycomb.GetActiveMapChunk(ant.transform.position)))
            {
                //Debug.LogWarning("AntSquad still active");
                return false;
                
            }
        }
        //Debug.LogWarning("deactivate AntSquad");
        return true;
    }

    public void Respawn()
    {
        foreach (Ant ant in Squad)
        {
            if(ant)ant.gameObject.SetActive(true);
        }
    }
    public void Despawn()
    {
        foreach (Ant ant in Squad)
        {
            if (ant) ant.gameObject.SetActive(false);
        }
    }

    public void AntDeath()
    {
        deadCount++;
    }
    
}
