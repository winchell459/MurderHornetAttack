using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillapillarController : Insect
{
    public PillapillarController Head, Tail;
    public PillapillarEgg egg;
    public Vector2 Target;
    public List<Vector2> Path = new List<Vector2>();
    public float TargetThreshold = 0.01f;
    public float Velocity = 5;
    public HoneycombDir Direction = new HoneycombDir(-1,1);
    private HoneycombDir[] Directions = { new HoneycombDir(-1, -1), new HoneycombDir(0, -1), new HoneycombDir(1, 1), new HoneycombDir(1,-1), new HoneycombDir(0,1), new HoneycombDir(-1,1)};

    public int headIndex = 0;
    public GameObject SnakeLinkPrefab;

    public int SpawnRate = 3;
    private int lastSpawn = 0;

    public float SnakeSpacing = 1f;

    public float AttackRadius = 5f;

    public enum PathTypes
    {
        Random,
        Pattern
    }
    public PathTypes PathType;

    public List<HoneycombVectors> MyPath;
    // Start is called before the first frame update
    void Start()
    {
        
        if (PathType == PathTypes.Random)
        {
            Target = Utility.Honeycomb.HoneycombGridToWorldPostion((Utility.Honeycomb.WorldPointToHoneycombGrid(transform.position)));
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

        if (!Head) // -> is Head
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
            PillapillarController tail = Instantiate(FindObjectOfType<MapGenerator>().SnakePrefab, transform.position - (-transform.right.normalized)*Time.deltaTime, Quaternion.identity).GetComponent<PillapillarController>();

            egg.PillapillarBornt(tail);
            

            Tail = tail;
            tail.Head = this;
           
            SetTailPosition();
        }
    }
    private int decrementHeadIndex(int index, int decrent)
    {
        index -= decrent;
        while (index < 0) index += GetPathCount();

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
    private void setPath(List<HoneycombVectors> path)
    {
        Path.Clear();
        //Path.Add(Utility.HoneycombGridToWorldPostion((Utility.WorldToHoneycomb(transform.position))));
        Target = Utility.Honeycomb.HoneycombGridToWorldPostion(Utility.Honeycomb.GetHoneycombDirection(Utility.Honeycomb.WorldPointToHoneycombGrid(transform.position), path[0].Direction, path[0].Distance));
        Path.Add(Target);
        for(int i = 1; i < path.Count; i += 1)
        {
            Path.Add(Utility.Honeycomb.HoneycombGridToWorldPostion(Utility.Honeycomb.GetHoneycombDirection(Utility.Honeycomb.WorldPointToHoneycombGrid(Path[i-1]), path[i].Direction, path[i].Distance)));
        }
        pointSnake();
    }
    public int GetPathCount()
    {
        if (!Head) return Path.Count;
        else return Head.GetPathCount();
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
        //Debug.Log($"headIndex {headIndex}");
        if (!Head) return Path[headIndex];
        else return Head.GetNextHeadTarget(headIndex);
    }
    private Vector2 getNewTarget()
    {
        headIndex += 1;
        if (!Head)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player)
            {
                HoneycombPos playerHex = Utility.Honeycomb.WorldPointToHoneycombGrid(player.transform.position);
                HoneycombPos snakeHex = Utility.Honeycomb.WorldPointToHoneycombGrid(transform.position);

                //Debug.Log($"playerHex: {playerHex} {Map.StaticMap.GetHoneycomb((int)playerHex.x, (int)playerHex.y).LocationType.ToString()}");


                if (true && player && Utility.Honeycomb.DistanceBetweenHoneycomb(playerHex, snakeHex) < AttackRadius && Map.StaticMap.GetHoneycomb((int)playerHex.x, (int)playerHex.y).LocationType == HoneycombTypes.Variety.Chamber)
                {
                    HoneycombPos playerGridHex = playerHex;// Utility.WorldPointToHoneycombGrid(player.transform.position);
                    HoneycombPos startGridHex = snakeHex; // Utility.WorldPointToHoneycombGrid(transform.position);
                    Vector2 playerGridPos = findPathToHoneycomb(startGridHex, playerGridHex);
                    return playerGridPos;
                }
                else return getRandomLoc();
            }
            else return getRandomLoc();

        }
        else
        {
            // headIndex += 1;
            return Head.GetNextHeadTarget(headIndex);
        }
    }

    int pathAttempts = 0;
    private Vector2 getRandomLoc()
    {
        Debug.Log("getRandomLoc");
        int randDist = Random.Range(1, 4) * 3;
        int ranDir = Random.Range(0, 2);
        
        if (ranDir == 0) Direction = GetNewDirection(Direction, 1);
        else Direction = GetNewDirection(Direction, -1);
        //Debug.Log("HoneyDir: " + Direction + " HoneyDist: " + randDist + " HoneyPos: " + Utility.WorldToHoneycomb(Target));
        //Debug.Log("--------------------------- new Path Start----------------------------");
        //List<MapHoneycomb> path = Utility.GetHoneycombPath(Utility.WorldPointToHoneycombGrid(Target), Direction, randDist);
        //Debug.Log("path length: " + path.Count);
        //Debug.Log("--------------------------- new Path Honeycombs----------------------------");
        
        MapHoneycomb newTarget = Utility.Honeycomb.GetHoneycombFreePath(Utility.Honeycomb.WorldPointToHoneycombGrid(Target), Direction, randDist, GetPillapillarsPos());

        

        //Debug.Log("--------------------------- new Path End----------------------------");
        //return Utility.HoneycombGridToWorldPostion( Utility.GetHoneycombDirection(Utility.WorldToHoneycomb(Target), Direction, randDist));
        if (newTarget != null)
        {
            pathAttempts = 0;
            return newTarget.position;
        }
        else
        {
            pathAttempts += 1;
            if (pathAttempts > 6)
            {
                Debug.LogWarning($"Pillapillar {gameObject.name} could not find a newTarget");
                return Vector2.zero;
            }
            return getRandomLoc();
        }
        
    }

    private List<HoneycombPos> GetPillapillarsPos()
    {
        List<HoneycombPos> pillapillarPos = new List<HoneycombPos>();
        foreach (PillapillarController pillapillar in FindObjectsOfType<PillapillarController>())
        {
            pillapillarPos.Add(Utility.Honeycomb.WorldPointToHoneycombGrid(pillapillar.transform.position));
        }
        return pillapillarPos;
    }

    private HoneycombDir GetNewDirection(HoneycombDir current, int turns) //turns + for counter clockwise
    {
        HoneycombDir newDir = new HoneycombDir(0, 0);
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

    private Vector2 findPathToHoneycomb(HoneycombPos startHoneycomb, HoneycombPos targetHoneycomb)
    {
        //Vector2 targetPos = Utility.HoneycombGridToWorldPostion(targetHoneycomb);
        //Vector2 startPos = Utility.HoneycombGridToWorldPostion(startHoneycomb);
        HoneycombPos closestHex = startHoneycomb;

        if (startHoneycomb.x < targetHoneycomb.x && startHoneycomb.y > targetHoneycomb.y)
        {
            //(1,-1) || (0,-1)
            HoneycombDir dirOne = new HoneycombDir(1, -1);
            HoneycombDir dirTwo = new HoneycombDir(0, -1);
            closestHex = compareShortestPaths(dirOne, dirTwo, startHoneycomb, targetHoneycomb);
        }
        else if(startHoneycomb.x < targetHoneycomb.x && startHoneycomb.y < targetHoneycomb.y)
        {
            //(1,1) || (0,1)
            HoneycombDir dirOne = new HoneycombDir(1, 1);
            HoneycombDir dirTwo = new HoneycombDir(0, 1);
            closestHex = compareShortestPaths(dirOne, dirTwo, startHoneycomb, targetHoneycomb);

        }
        else if (startHoneycomb.x < targetHoneycomb.x && startHoneycomb.y == targetHoneycomb.y)
        {
            //(1,1) || (1,-1)
            HoneycombDir dirOne = new HoneycombDir(1, 1);
            HoneycombDir dirTwo = new HoneycombDir(1, -1);
            closestHex = compareShortestPaths(dirOne, dirTwo, startHoneycomb, targetHoneycomb);
        }
        else if (startHoneycomb.x > targetHoneycomb.x && startHoneycomb.y > targetHoneycomb.y)
        {
            //(-1,-1) || (0,-1)
            HoneycombDir dirOne = new HoneycombDir(-1, -1);
            HoneycombDir dirTwo = new HoneycombDir(0, -1);
            closestHex = compareShortestPaths(dirOne, dirTwo, startHoneycomb, targetHoneycomb);
        }
        else if (startHoneycomb.x > targetHoneycomb.x && startHoneycomb.y < targetHoneycomb.y)
        {
            //(-1,1) || (0,1)
            HoneycombDir dirOne = new HoneycombDir(-1, 1);
            HoneycombDir dirTwo = new HoneycombDir(0, 1);
            closestHex = compareShortestPaths(dirOne, dirTwo, startHoneycomb, targetHoneycomb);
        }
        else if (startHoneycomb.x > targetHoneycomb.x && startHoneycomb.y == targetHoneycomb.y)
        {
            //(-1,1) || (-1,-1)
            HoneycombDir dirOne = new HoneycombDir(-1, 1);
            HoneycombDir dirTwo = new HoneycombDir(-1, -1);
            closestHex = compareShortestPaths(dirOne, dirTwo, startHoneycomb, targetHoneycomb);
        }
        else if (startHoneycomb.x == targetHoneycomb.x && startHoneycomb.y < targetHoneycomb.y)
        {
            // (0,1)
            HoneycombPos hexOne = FindShortestPath(startHoneycomb, new HoneycombDir(0,1), targetHoneycomb);
            if (Utility.Honeycomb.DistanceBetweenHoneycomb(closestHex, targetHoneycomb) > Utility.Honeycomb.DistanceBetweenHoneycomb(hexOne, targetHoneycomb)) closestHex = hexOne;
        }
        else if (startHoneycomb.x == targetHoneycomb.x && startHoneycomb.y > targetHoneycomb.y)
        {
            // (0,-1)
            HoneycombPos hexOne = FindShortestPath(startHoneycomb, new HoneycombDir(0, -1), targetHoneycomb);
            if (Utility.Honeycomb.DistanceBetweenHoneycomb(closestHex, targetHoneycomb) > Utility.Honeycomb.DistanceBetweenHoneycomb(hexOne, targetHoneycomb)) closestHex = hexOne;
        }
        else
        {
            //(0,0)
            Debug.Log("Snake Follow Player Error");
        }
        if (closestHex.isNull) return Utility.Honeycomb.HoneycombGridToWorldPostion(startHoneycomb);
        return Utility.Honeycomb.HoneycombGridToWorldPostion( closestHex);
    }

    private HoneycombPos compareShortestPaths(HoneycombDir dirOne, HoneycombDir dirTwo, HoneycombPos startHoneycomb, HoneycombPos targetHoneycomb)
    {
        //Vector2 targetPos = Utility.HoneycombGridToWorldPostion(targetHoneycomb);
        //Vector2 startPos = Utility.HoneycombGridToWorldPostion(startHoneycomb);
        HoneycombPos closestHex = startHoneycomb;

        HoneycombPos hexOne = FindShortestPath(startHoneycomb, dirOne, targetHoneycomb);
        HoneycombPos hexTwo = FindShortestPath(startHoneycomb, dirTwo, targetHoneycomb);

        if (!hexOne.isNull && Utility.Honeycomb.DistanceBetweenHoneycomb(closestHex, targetHoneycomb) > Utility.Honeycomb.DistanceBetweenHoneycomb(hexOne, targetHoneycomb)) closestHex = hexOne;
        if (!hexTwo.isNull && Utility.Honeycomb.DistanceBetweenHoneycomb(closestHex, targetHoneycomb) > Utility.Honeycomb.DistanceBetweenHoneycomb(hexTwo, targetHoneycomb)) closestHex = hexTwo;
        return closestHex;
    }

    private HoneycombPos FindShortestPath(HoneycombPos startHex, HoneycombDir hexDir, HoneycombPos targetHex)
    {
        int distance = 1;
        HoneycombPos pathHex = startHex;
        MapHoneycomb newTarget = Utility.Honeycomb.GetHoneycombFreePath(startHex, hexDir, distance, GetPillapillarsPos());
        MapHoneycomb nextTarget = Utility.Honeycomb.GetHoneycombFreePath(startHex, hexDir, distance + 1, GetPillapillarsPos());
        while(newTarget != null && nextTarget != null && Utility.Honeycomb.DistanceBetweenHoneycomb(Utility.Honeycomb.WorldPointToHoneycombGrid(newTarget.position), targetHex) > Utility.Honeycomb.DistanceBetweenHoneycomb(Utility.Honeycomb.WorldPointToHoneycombGrid(nextTarget.position), targetHex))
        {
            newTarget = nextTarget;
            distance += 1;
            nextTarget = Utility.Honeycomb.GetHoneycombFreePath(startHex, hexDir, distance + 1, GetPillapillarsPos());
        }
        //Debug.Log("Closets HexPos: " + Utility.WorldPointToHoneycombGrid(newTarget.position));
        if (newTarget == null) return HoneycombPos.nullHex;
        return Utility.Honeycomb.WorldPointToHoneycombGrid(newTarget.position);
    }

    public void DestroyLink()
    {
        if (Tail)
        {
            Tail.Path = GetPath(Tail.headIndex);
            Tail.Head = null;
            Tail.Target = Tail.Path[Tail.headIndex];
            Tail.Direction = Utility.Honeycomb.WorldDirToHoneycombDir((Tail.Path[Tail.headIndex] - (Vector2)Tail.transform.position).normalized); // Tail.Path[Tail.headIndex - 1]).normalized;
        }

        Destroy(gameObject);
    }

    public void DestroySnake()
    {
        
        while (Tail)
        {
            PillapillarController tail = Tail.Tail;
            egg.PillapillarSuicide(Tail);
            Destroy(Tail.gameObject);
            Tail = tail;
        }
        while (Head)
        {
            PillapillarController head = Head.Head;
            egg.PillapillarSuicide(Head);
            Destroy(Head.gameObject);
            Head = head;
        }
        Debug.Log("Destroy Snake");
        egg.PillapillarSuicide(this);
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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<PillapillarController>())
        {
            PillapillarController link = collision.GetComponent<PillapillarController>();

            if(link != Head && link != Tail && Utility.Honeycomb.WorldPointToHoneycombGrid(collision.transform.position) == Utility.Honeycomb.WorldPointToHoneycombGrid(transform.position))
            {
                if (!link.Head) link.DestroySnake();
                else
                {
                    egg.PillapillarSuicide(this);
                    DestroyLink();
                }
            }
                //DestroySnake();
        }else if (collision.GetComponent<EnemyPhysics>())//bee collision
        {
            if(Head == null)
            {
                spawnTail();
            }
            else
            {
                TakeDamage(1);
            }
            Destroy(collision.gameObject);
        }
    }

    public override void TakeDamage(float damage)
    {
        Health -= damage;
        takeDamageAnimation();
        if (Health <= 0)
        {
            FindObjectOfType<LevelHandler>().EnemyDeath(gameObject);
            egg.PillapillarKilled(this);
            DestroyLink();
        }
    }

    private void takeDamageAnimation()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        float colorPercent =  Health / MaxHealth;
        sprite.color = new Color(1, colorPercent, colorPercent);
    }
}
[System.Serializable]
public class HoneycombVectors
{
    public HoneycombDir Direction;
    public int Distance;
}
