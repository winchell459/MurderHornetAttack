using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
    public SnakeController Head, Tail;
    public Vector2 Target;
    public List<Vector2> Path = new List<Vector2>();
    public float TargetThreshold = 0.01f;
    public float Velocity = 5;
    public Vector2 Direction = new Vector2(-1,1);
    private Vector2[] Directions = { new Vector2(-1, -1), new Vector2(0, -1), new Vector2(1, 1), new Vector2(1,-1), new Vector2(0,1), new Vector2(-1,1)};

    public int headIndex = 0;
    public GameObject SnakeLinkPrefab;

    public int SpawnRate = 3;
    private int lastSpawn = 0;

    public float SnakeSpacing = 1f;
    // Start is called before the first frame update
    void Start()
    {
        Target = Utility.HoneycombGridToWorldPostion((Utility.WorldToHoneycomb(transform.position)));
        Path.Add(Target);
        Debug.Log(Target);
    }

    // Update is called once per frame
    void Update()
    {

        if (!Head)
        {
            if (Vector2.Distance(transform.position, Target) > Velocity * Time.deltaTime)
            {
                transform.position = Vector2.MoveTowards(transform.position, Target, Velocity * Time.deltaTime);
            }
            else
            {
                float secondDistance = Velocity * Time.deltaTime - Vector2.Distance(transform.position, Target);

                Vector2 newTarget = getNewTarget();
                Path.Add(newTarget);
                transform.position = secondDistance * (newTarget - Target).normalized + Target;

                Target = newTarget;
                transform.right = -(Target - (Vector2)transform.position);
                lastSpawn++;
                if (!Head && lastSpawn >= SpawnRate)
                {
                    lastSpawn = 0;
                    spawnTail();
                }
            }

            for (int i = 1; i < Path.Count; i++)
            {
                Debug.DrawLine(Path[i - 1], Path[i], Color.red);
            }

            SetTailPosition();
        }

        
        
    }
    private void spawnTail()
    {
        if (Tail) Tail.spawnTail();
        else
        {
            SnakeController tail = Instantiate(SnakeLinkPrefab, transform.position - (-transform.right.normalized)*Time.deltaTime, Quaternion.identity).GetComponent<SnakeController>();
            Tail = tail;
            tail.Head = this;
            //tail.SnakeLinkPrefab = SnakeLinkPrefab;
            //tail.headIndex = headIndex;
            SetTailPosition();
        }
    }
    public void SetTailPosition()
    {
        if(Tail)
        {
            if(Vector2.Distance(transform.position, GetNextHeadTarget(headIndex - 1)) > SnakeSpacing)
            {
                Vector2 dir = (GetNextHeadTarget(headIndex) - GetNextHeadTarget(headIndex - 1)).normalized;
                Tail.transform.position = -dir * SnakeSpacing + (Vector2)transform.position;
                Tail.transform.right = -dir;
                Tail.headIndex = headIndex;
            }
            else
            {
                Vector2 dir = (GetNextHeadTarget(headIndex-1) - GetNextHeadTarget(headIndex - 2)).normalized;
                Tail.transform.position = -dir * (SnakeSpacing - Vector2.Distance(transform.position, GetNextHeadTarget(headIndex - 1))) + GetNextHeadTarget(headIndex - 1);
                Tail.transform.right = -dir;
                Tail.headIndex = headIndex - 1;
            }
            Tail.SetTailPosition();
        }
    }
    public Vector2 GetNextHeadTarget(int headIndex)
    {
        if (!Head) return Path[headIndex];
        else return Head.GetNextHeadTarget(headIndex);
    }
    private Vector2 getNewTarget()
    {
        headIndex += 1;
        if (!Head) return getRandomLoc();
        else
        {
            // headIndex += 1;
            return Head.GetNextHeadTarget(headIndex);
        }
    }

    private Vector2 getRandomLoc()
    {
        int randDist = Random.Range(1, 4) * 3;
        int ranDir = Random.Range(0, 2);
        
        if (ranDir == 0) Direction = GetNewDirection(Direction, 1);
        else Direction = GetNewDirection(Direction, -1);
        //Debug.Log("HoneyDir: " + Direction + " HoneyDist: " + randDist + " HoneyPos: " + Utility.WorldToHoneycomb(Target));
        
        return Utility.HoneycombGridToWorldPostion( Utility.GetHoneycombDirection(Utility.WorldToHoneycomb(Target), Direction, randDist));
        
    }

    private Vector2 GetNewDirection(Vector2 current, int turns) //turns + for counter clockwise
    {
        Vector2 newDir = new Vector2(0, 0);
        int index = 0;
        for(index = 0; index < Directions.Length; index += 1)
        {
            if(current.x == Directions[index].x && current.y == Directions[index].y)
            {
                break;
            }
        }
        if (turns > 0) index = (index + 1) % Directions.Length;
        else
        {
            index -= 1;
            if (index < 0) index = Directions.Length - 1;
        }
        newDir = Directions[index];

        turns = turns - (int)Mathf.Sign(turns) *1;
        if (turns != 0) return GetNewDirection(newDir, turns);
        else return newDir;
    }

    
}
