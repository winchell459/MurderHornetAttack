using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillapillarController : Insect, IChunkObject
{
    public PillapillarController Head, Tail;
    public PillapillarEgg egg;
    public Vector2 Target;
    public List<Vector2> Path = new List<Vector2>();
    public float TargetThreshold = 0.01f;
    public float Velocity = 5;
    public HoneycombDir Direction = new HoneycombDir(-1,1);
    

    public int headIndex = 0;
    public GameObject SnakeLinkPrefab;

    public int SpawnRate = 3;
    private int lastSpawn = 0;

    public float SnakeSpacing = 1f;

    public float AttackRadius = 5f;
    public float roamingRadius = 10f;

    public enum PathTypes
    {
        Random,
        Pattern,
        Debug
    }
    public PathTypes PathType;

    public List<HoneycombVectors> MyPath;
    // Start is called before the first frame update
    void Start()
    {
        
        if (PathType == PathTypes.Random || PathType == PathTypes.Debug)
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

    [SerializeField]bool paused = false;
    // Update is called once per frame
    void Update()
    {

        if (!Head && !paused) // -> is Head
        {  
            if (Vector2.Distance(transform.position, Target) > Velocity * Time.deltaTime)
            {
                transform.position = Vector2.MoveTowards(transform.position, Target, Velocity * Time.deltaTime);
            }
            else if (headIndex <= Path.Count - 2)
            {
                headIndex = (headIndex + 1);
                HandleNewTarget(Path[headIndex]);
            }
            else
            {
                HandleGetNewTarget();
                testingSpawnTail();
            }
            SetTailPosition();
        }
        else if (!Head /*&& paused*/ && PathType == PathTypes.Debug)
        {
            GetNextDebugTarget();
        }

        //debug display path in scene view
        if (!Head)
        {
            int lineCount = Mathf.Min(Path.Count, 50);
            for (int i = 1 + Path.Count - lineCount; i < Path.Count; i++)
            {
                Debug.DrawLine(Path[i - 1], Path[i], Color.red);
            }
        }

    }
    private void HandleGetNewTarget()
    {
        Vector2 newTarget;
        if (PathType == PathTypes.Random)
        {
            newTarget = getNewTarget();
            if (newTarget != Vector2.zero)
            {
                Path.Add(newTarget);
                headIndex += 1;
            }
            else
            {
                //pause pillapillar
                paused = true;

                HoneycombPos snakeHex = Utility.Honeycomb.WorldPointToHoneycombGrid(transform.position);
                newTarget = Utility.HoneycombPathfinding.GetHoneycombInDirection(snakeHex, Direction, 1).worldPos;
                Debug.LogWarning("Pillapillar is Trapped!");

                Path.Add(newTarget);
                headIndex += 1;
            }
        }
        else if (PathType == PathTypes.Pattern)
        {
            headIndex = (headIndex + 1) % Path.Count;
            newTarget = Path[headIndex];
        }
        else
        {
            if (headIndex == Path.Count - 1)
            {
                paused = true;
                return;
            }
            else
            {
                headIndex++;
                paused = false;
            }

            newTarget = Path[headIndex];

        }

        HandleNewTarget(newTarget);
    }
    private void HandleNewTarget(Vector2 newTarget)
    {
        float remainingDistance = Velocity * Time.deltaTime - Vector2.Distance(transform.position, Target);
        Vector2 newPos = remainingDistance * (newTarget - Target).normalized + Target;

        if (headIndex >= Path.Count || headIndex < 0) Debug.LogWarning($"headIndex/Path.Count: {headIndex}/{Path.Count}");

        HoneycombDir newDirection = Utility.Honeycomb.WorldDirToHoneycombDir((Path[headIndex] - newPos).normalized);
        if (Utility.HoneycombPathfinding.CheckReverseDirection(newDirection,Direction))
        {
            Debug.LogWarning("Reversed Direction");
            newTarget = getRandomLoc();
            if(newTarget == Vector2.zero)
            {
                //move foward one hex
                HoneycombPos snakeHex = Utility.Honeycomb.WorldPointToHoneycombGrid(transform.position);
                newTarget = Utility.HoneycombPathfinding.GetHoneycombInDirection(snakeHex, Direction, 1).worldPos;
                Debug.LogWarning("Pillapillar is Trapped!");
            }
            Path[Path.Count - 1] = newTarget;
            newPos = remainingDistance * (newTarget - Target).normalized + Target;
            newDirection = Utility.Honeycomb.WorldDirToHoneycombDir((Path[headIndex] - newPos).normalized);
        }

        transform.position = newPos;
        Target = newTarget;
        Direction = newDirection;

        pointSnake();
    }

    Vector2 debugTarget = Vector2.zero;
    Transform debugTargetTransform = null;
    private Vector2 GetNextDebugTarget()
    {
        Vector3 target = transform.position;
        if (Input.GetMouseButton(0))
        {
            target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            debugTarget = target;

            RaycastHit2D hit = Physics2D.Raycast(target, Vector2.zero);
            if (hit && hit.transform.GetComponent<Insect>())
            {
                debugTargetTransform = hit.transform;
            }

            target = SetDebugTarget(target);

            
        }
        else if (debugTargetTransform)
        {
            target = SetDebugTarget(debugTargetTransform.position);
        }
        else if (debugTarget != Vector2.zero)
        {
            if(Vector2.Distance(transform.position, debugTarget) > 1)
            {
                target = SetDebugTarget(debugTarget);
            }
            else
            {
                debugTarget = Vector2.zero;
            }
        }
        else
        {
            paused = true;
        }
        return target;
    }
    private Vector2 SetDebugTarget(Vector2 debugTarget)
    {
        HoneycombPos clickHex = Utility.Honeycomb.WorldPointToHoneycombGrid(debugTarget);
        HoneycombPos snakeHex = Utility.Honeycomb.WorldPointToHoneycombGrid(transform.position);
        Vector2 target = Utility.HoneycombPathfinding.FindPathToHoneycomb(snakeHex, clickHex, GetPillapillarsPos());
        Debug.Log($"startHex:{snakeHex} clickHex{clickHex} targetHex:{Utility.Honeycomb.WorldPointToHoneycombGrid(target)}");

        Path.Add(target);
        paused = false;
        return target;
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
            if(LevelHandler.singleton.PillapillerSpawnTail(this))spawnTail();
        }
    }
    public static PillapillarController SpawnPillipallar(PillapillarEgg egg, Vector2 pos, Transform parent)
    {
        PillapillarController pillapillarLink = Instantiate(MapManager.singleton.SnakePrefab, pos, Quaternion.identity).GetComponent<PillapillarController>();
        egg.PillapillarBornt(pillapillarLink);
        pillapillarLink.transform.parent = parent;
        Map.StaticMap.AddTransientChunkObject(pillapillarLink);
        return pillapillarLink;
    }
    private void spawnTail()
    {
        if (Tail) Tail.spawnTail();
        else
        {
            PillapillarController tail = SpawnPillipallar(egg, transform.position - (-transform.right.normalized)*Time.deltaTime, transform.parent);

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
        Target = Utility.Honeycomb.HoneycombGridToWorldPostion(Utility.HoneycombPathfinding.GetHoneycombInDirection(Utility.Honeycomb.WorldPointToHoneycombGrid(transform.position), path[0].Direction, path[0].Distance));
        Path.Add(Target);
        for(int i = 1; i < path.Count; i += 1)
        {
            Path.Add(Utility.Honeycomb.HoneycombGridToWorldPostion(Utility.HoneycombPathfinding.GetHoneycombInDirection(Utility.Honeycomb.WorldPointToHoneycombGrid(Path[i-1]), path[i].Direction, path[i].Distance)));
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
            for(int i = 0; i <= startIndex && i < Path.Count; i+= 1)
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
        if (!Head)
        {
            headIndex = Mathf.Min(headIndex, Path.Count - 1);
            return Path[headIndex];
        }
        else return Head.GetNextHeadTarget(headIndex);
    }
    

    void FindFood()
    {

    }

    private Vector2 getNewTarget()
    {

        if (!Head)
        {
            List<HoneycombPos> otherPos = GetPillapillarsPos();
            HoneycombPos snakeHex = Utility.Honeycomb.WorldPointToHoneycombGrid(transform.position);
            GameObject player = LevelHandler.singleton.Player ? LevelHandler.singleton.Player.gameObject : null;
            if (player)
            {
                HoneycombPos playerHex = Utility.Honeycomb.WorldPointToHoneycombGrid(player.transform.position);
                if (true && player && Utility.Honeycomb.DistanceBetweenHoneycomb(playerHex, snakeHex) < AttackRadius && Map.StaticMap.GetHoneycomb((int)playerHex.x, (int)playerHex.y).LocationType == HoneycombTypes.Variety.Chamber)
                {
                    HoneycombPos playerGridHex = playerHex;
                    HoneycombPos startGridHex = snakeHex;
                    Vector2 playerGridPos = Utility.HoneycombPathfinding.FindPathToHoneycomb(startGridHex, playerGridHex, otherPos);
                    if (Utility.HoneycombPathfinding.CheckValidNewTarget(snakeHex.worldPos, playerGridPos, Direction))
                    {
                        Debug.Log("-------------- player target -----------------");
                        return playerGridPos;
                    }
                    
                }
                
            }
            
            EnemyPhysics[] bees = GetClosestBee();

            if (bees.Length > 0)
            {
                foreach(EnemyPhysics bee in bees)
                {
                    HoneycombPos beeHex = Utility.Honeycomb.WorldPointToHoneycombGrid(bee.transform.position);
                    if (Utility.Honeycomb.DistanceBetweenHoneycomb(beeHex, snakeHex) < AttackRadius * 2)
                    {
                        Vector2 beePos = Utility.HoneycombPathfinding.FindPathToHoneycomb(snakeHex, beeHex, otherPos);
                        if (Utility.HoneycombPathfinding.CheckValidNewTarget(snakeHex.worldPos, beePos, Direction))
                        {
                            Debug.Log("-------------- bee target -----------------");
                            return beePos;
                        }
                        
                    }
                }
                

            }

            // ----------------------------- Return to Egg Radius or Trapped || Random ---------------------------------------
            HoneycombPos eggHex = Utility.Honeycomb.WorldPointToHoneycombGrid(egg.transform.position);
            Vector2 newTarget = getRandomLoc();
            if (Utility.Honeycomb.DistanceBetweenHoneycomb(snakeHex, eggHex) > roamingRadius)
            {
                Debug.Log("-------------- return to egg -----------------");
                newTarget = Utility.HoneycombPathfinding.FindPathToHoneycomb(snakeHex, eggHex, otherPos);
            }
            else
            {
                Debug.Log("-------------- random location target -----------------");
                
                if (newTarget == Vector2.zero)
                {
                    Debug.Log("-------------- return to egg -----------------");

                    newTarget = Utility.HoneycombPathfinding.FindPathToHoneycomb(snakeHex, eggHex, otherPos);

                }
            }
            return newTarget;

        }
        else
        {
            return Head.GetNextHeadTarget(headIndex);
        }
    }

    private Vector2 getRandomLoc()
    {
        Debug.Log("getRandomLoc");
        
        int ranDir = Random.Range(0, 2);
        int otherDir = 0;

        MapHoneycomb newTarget;
        List<HoneycombPos> otherPos = GetPillapillarsPos();

        if(ranDir == 0)
        {
            otherDir = 1;
            newTarget = GetRandomLoc(-1, otherPos);
        }
        else
        {
            otherDir = -1;
            newTarget = GetRandomLoc(1, otherPos);
        }

        if (newTarget != null)
        {
            return newTarget.position;
        }
        else
        {
            Debug.Log("getRandomLoc with otherDir");
            newTarget = GetRandomLoc(otherDir, otherPos);
            if (newTarget != null)
            {
                return newTarget.position;
            }
            else
            {
                Debug.LogWarning($"Pillapillar {gameObject.name} could not find a newTarget, returning {Vector2.negativeInfinity}");
                return Vector2.zero;
            }
        }
        
    }

    private MapHoneycomb GetRandomLoc(int turn, List<HoneycombPos> otherPos)
    {
        int randDist = Random.Range(1, 4) * 3;
        HoneycombDir newDirection;
        newDirection = Utility.HoneycombPathfinding.GetNewDirection(Direction, turn);
        
        return Utility.HoneycombPathfinding.GetHoneycombFreePath(Utility.Honeycomb.WorldPointToHoneycombGrid(Target), newDirection, randDist, otherPos);
    }

    //------------------------ 

    private List<HoneycombPos> GetPillapillarsPos()
    {
        List<HoneycombPos> pillapillarPos = new List<HoneycombPos>();
        foreach (PillapillarController pillapillar in FindObjectsOfType<PillapillarController>())
        {
            pillapillarPos.Add(Utility.Honeycomb.WorldPointToHoneycombGrid(pillapillar.transform.position));
        }
        if (PathType == PathTypes.Debug && LevelHandler.singleton.Player) pillapillarPos.Add(Utility.Honeycomb.WorldPointToHoneycombGrid(LevelHandler.singleton.Player.transform.position));
        return pillapillarPos;
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

        HandleLinkDeath();
    }

    public void DestroySnake()
    {
        
        while (Tail)
        {
            PillapillarController tail = Tail.Tail;
            egg.PillapillarSuicide(Tail);
            Tail.HandleLinkDeath();
            //Destroy(Tail.gameObject);
            Tail = tail;
        }
        while (Head)
        {
            PillapillarController head = Head.Head;
            egg.PillapillarSuicide(Head);
            Head.HandleLinkDeath();
            //Destroy(Head.gameObject);
            Head = head;
        }
        Debug.Log("Destroy Snake");
        egg.PillapillarSuicide(this);
        HandleLinkDeath();
    }

    private void HandleLinkDeath()
    {
        myChunk.RemoveTransientChunkObject(this);
        Destroy(gameObject);
    }

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log(collision.gameObject.name);
        if (collision.gameObject.GetComponent<PillapillarController>())
        {
            PillapillarController link = collision.gameObject.GetComponent<PillapillarController>();

            if (link != Head && link != Tail)
            {
                if (!link.Head) link.DestroySnake();
                else
                {
                    egg.PillapillarSuicide(this);
                    DestroyLink();
                }
            }
            //DestroySnake();
        }
        
    }
    public bool digging = false;
    private void OnTriggerStay2D(Collider2D collision)
    {
        //if (paused) return;
        if (collision.GetComponent<EnemyPhysics>())//bee collision
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
        }else if (collision.CompareTag("Honeycomb"))
        {
            if (digging)
            {
                collision.GetComponent<Honeycomb>().DamageHoneycomb(float.PositiveInfinity, HoneycombTypes.Areas.Garden, HoneycombTypes.Variety.Chamber);
            }
            else
            {
                collision.GetComponent<Honeycomb>().DamageHoneycomb(float.PositiveInfinity, HoneycombTypes.Areas.Connection, HoneycombTypes.Variety.Embedded);
            }
            
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
            GameManager.PillapillarLinkKilled();
            if (!Head && !Tail) GameManager.PillapillarKilled();
            DestroyLink();
        }
    }

    private void takeDamageAnimation()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        float colorPercent =  Health / MaxHealth;
        sprite.color = new Color(1, colorPercent, colorPercent);
    }

    public void Activate()
    {
        RespawnHead();
        RespawnTail();
    }

    public void Deactivate()
    {
        if(CheckDespawnHead() && CheckDespawnTail())
        {
            DespawnHead();
            DespawnTail();
        }
        
    }

    public bool CheckDespawnHead()
    {
        if (!Utility.Honeycomb.GetActiveMapChunk(transform.position))
        {
            if (!Head)
            {
                return true;
            }
            else return Head.CheckDespawnHead();
        }
        else
        {
            return false;
        }
    }

    public bool CheckDespawnTail()
    {
        if (!Utility.Honeycomb.GetActiveMapChunk(transform.position))
        {
            if (!Tail) return true;
            else return Tail.CheckDespawnTail();
        }
        else
        {
            return false;
        }
    }

    public void DespawnHead()
    {
        gameObject.SetActive(false);
        if (Head) Head.DespawnHead();
    }

    public void DespawnTail()
    {
        gameObject.SetActive(false);
        if (Tail) Tail.DespawnTail();
    }

    public void RespawnHead()
    {
        gameObject.SetActive(true);
        if (Head) Head.RespawnHead();
    }

    public void RespawnTail()
    {
        gameObject.SetActive(true);
        if (Tail) Tail.RespawnTail();
    }

    GameObject IChunkObject.GameObject()
    {
        return gameObject;
    }

    MapChunk myChunk;
    void IChunkObject.SetMyChunk(MapChunk myChunk)
    {
        this.myChunk = myChunk;
    }
}
[System.Serializable]
public class HoneycombVectors
{
    public HoneycombDir Direction;
    public int Distance;
}
