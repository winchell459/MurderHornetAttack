using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : Insect
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
    

    public enum PathTypes
    {
        Random,
        Pattern
    }
    public PathTypes PathType;

    public List<HoneycompVectors> MyPath;
    // Start is called before the first frame update
    void Start()
    {
        
        if (PathType == PathTypes.Random)
        {
            Target = Utility.HoneycombGridToWorldPostion((Utility.WorldToHoneycomb(transform.position)));
            Path.Add(Target);
        }
        else
        {
            setPath(MyPath);
        }
        


        //Debug.Log(Target);
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

                Vector2 newTarget;
                if (PathType == PathTypes.Random)
                {
                    newTarget = getNewTarget();
                    Path.Add(newTarget);
                }
                else
                {
                    headIndex = (headIndex + 1) % Path.Count;
                    newTarget = Path[headIndex];
                }
                    
                

                transform.position = secondDistance * (newTarget - Target).normalized + Target;
                Target = newTarget;

                pointSnake();
                

                //Testing Spawn Tail
                testingSpawnTail();

            }

            for (int i = 1; i < Path.Count; i++)
            {
                Debug.DrawLine(Path[i - 1], Path[i], Color.red);
            }

            SetTailPosition();
        }

        
        
    }
    private void pointSnake()
    {
        transform.right = -(Target - (Vector2)transform.position);
    }
    private void testingSpawnTail()
    {
        lastSpawn++;
        if (!Head && lastSpawn >= SpawnRate)
        {
            lastSpawn = 0;
            spawnTail();
        }
    }
    private void spawnTail()
    {
        if (Tail) Tail.spawnTail();
        else
        {
            SnakeController tail = Instantiate(FindObjectOfType<LevelHandler>().SnakePrefab, transform.position - (-transform.right.normalized)*Time.deltaTime, Quaternion.identity).GetComponent<SnakeController>();
            Tail = tail;
            tail.Head = this;
           
            SetTailPosition();
        }
    }
    private int decrementHeadIndex(int index, int decrent)
    {
        index -= decrent;
        while (index < 0) index += Path.Count;
        return index;
    }
    public void SetTailPosition()
    {
        if(Tail)
        {
            if(Vector2.Distance(transform.position, GetNextHeadTarget(decrementHeadIndex(headIndex, 1))) > SnakeSpacing)
            {
                Vector2 dir = (GetNextHeadTarget(headIndex) - GetNextHeadTarget(decrementHeadIndex(headIndex, 1))).normalized;
                Tail.transform.position = -dir * SnakeSpacing + (Vector2)transform.position;
                Tail.transform.right = -dir;
                Tail.headIndex = headIndex;
            }
            else
            {
                Vector2 dir = (GetNextHeadTarget(decrementHeadIndex(headIndex, 1)) - GetNextHeadTarget(decrementHeadIndex(headIndex , 2))).normalized;
                Tail.transform.position = -dir * (SnakeSpacing - Vector2.Distance(transform.position, GetNextHeadTarget(decrementHeadIndex(headIndex , 1)))) + GetNextHeadTarget(decrementHeadIndex(headIndex , 1));
                Tail.transform.right = -dir;
                Tail.headIndex = decrementHeadIndex(headIndex, 1);
            }
            Tail.SetTailPosition();
        }
    }
    private void setPath(List<HoneycompVectors> path)
    {
        Path.Clear();
        Target = Utility.HoneycombGridToWorldPostion(Utility.GetHoneycombDirection(Utility.WorldToHoneycomb(transform.position), path[0].Direction, path[0].Distance));
        Path.Add(Target);
        for(int i = 1; i < path.Count; i += 1)
        {
            Path.Add(Utility.HoneycombGridToWorldPostion(Utility.GetHoneycombDirection(Utility.WorldToHoneycomb(Path[i-1]), path[i].Direction, path[i].Distance)));
        }
        pointSnake();
    }
    public List<Vector2> GetPath(int startIndex)
    {
        if (!Head)
        {
            List<Vector2> path = new List<Vector2>();
            for(int i = 0; i <= startIndex; i+= 1)
            {
                path.Add(Path[i]);
            }
            return path;
        }
        else return Head.GetPath(startIndex);
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

    int pathAttempts = 0;
    private Vector2 getRandomLoc()
    {
        int randDist = Random.Range(1, 4) * 3;
        int ranDir = Random.Range(0, 2);
        
        if (ranDir == 0) Direction = GetNewDirection(Direction, 1);
        else Direction = GetNewDirection(Direction, -1);
        //Debug.Log("HoneyDir: " + Direction + " HoneyDist: " + randDist + " HoneyPos: " + Utility.WorldToHoneycomb(Target));
        Debug.Log("--------------------------- new Path Start----------------------------");
        List<MapHoneycomb> path = Utility.GetHoneycombPath(Utility.WorldToHoneycomb(Target), Direction, randDist);
        //Debug.Log("--------------------------- new Path Honeycombs----------------------------");
        MapHoneycomb newTarget = null;
        foreach (MapHoneycomb honeycomb in path)
        {
            //Debug.Log(honeycomb.position);
            if (!honeycomb.display && honeycomb.LocationType == MapHoneycomb.LocationTypes.Chamber)
            {
                newTarget = honeycomb;
                pathAttempts = 0;
            }
            else
            {
                Debug.Log(honeycomb.LocationType);
                break;
            }
        }

        Debug.Log("--------------------------- new Path End----------------------------");
        //return Utility.HoneycombGridToWorldPostion( Utility.GetHoneycombDirection(Utility.WorldToHoneycomb(Target), Direction, randDist));
        if (newTarget != null) return newTarget.position;
        else
        {
            pathAttempts += 1;
            if (pathAttempts > 3) return Vector2.zero;
            return getRandomLoc();
        }
        
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

    public void DestroyLink()
    {
        if (Tail)
        {
            Tail.Path = GetPath(Tail.headIndex);
            Tail.Head = null;
            Tail.Target = Tail.Path[Tail.headIndex];
            Tail.Direction = Utility.WorldDirToHoneycombDir((Tail.Path[Tail.headIndex] - (Vector2)Tail.transform.position).normalized); // Tail.Path[Tail.headIndex - 1]).normalized;
        }

        Destroy(gameObject);
    }

    public void DestroySnake()
    {
        //if (Tail) Tail.DestroySnake();
        //if (Head) Head.DestroySnake();
        while (Tail)
        {
            SnakeController tail = Tail.Tail;
            Destroy(Tail.gameObject);
            Tail = tail;
        }
        while (Head)
        {
            SnakeController head = Head.Head;
            Destroy(Head.gameObject);
            Head = head;
        }
        Debug.Log("Destroy Snake");
        Destroy(gameObject);
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.transform.GetComponent<SnakeController>())
    //    {
    //        SnakeController link = collision.transform.GetComponent<SnakeController>();
    //        if (!link.Head) link.DestroySnake();
    //        else DestroyLink();
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<SnakeController>())
        {
            SnakeController link = collision.GetComponent<SnakeController>();
            if(link != Head && link != Tail)
            {
                if (!link.Head) link.DestroySnake();
                else DestroyLink();
            }
                //DestroySnake();
        }
    }

    public override void TakeDamage(float damage)
    {
        Health -= damage;
        takeDamageAnimation();
        if (Health <= 0) DestroyLink();
    }

    private void takeDamageAnimation()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        float colorPercent =  Health / MaxHealth;
        sprite.color = new Color(1, colorPercent, colorPercent);
    }
}
[System.Serializable]
public class HoneycompVectors
{
    public Vector2 Direction;
    public int Distance;
}
