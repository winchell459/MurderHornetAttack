using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillapillarController : /*Insect, IChunkObject*/ InsectGroup
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

    public int PetActiveCount = 5;
    private int feedingCount = 0;

    Vector2 invalidPos = Utility.HoneycombPathfinding.invalidPos;

    private List<HoneycombTypes.Variety> stayInChamber = new List<HoneycombTypes.Variety>{ HoneycombTypes.Variety.Embedded, HoneycombTypes.Variety.Path };
    private List<HoneycombTypes.Variety> debugVarieties = new List<HoneycombTypes.Variety>();
    private List<HoneycombTypes.Variety> invalidVarieties { get { return PathType == PathTypes.Debug ? debugVarieties : stayInChamber; } }

    public enum PathTypes
    {
        Random,
        Pattern,
        Debug
    }
    public PathTypes PathType;
    public PathTypes TailPathType;

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

    }

    [SerializeField]bool paused = false;
    // Update is called once per frame
    void Update()
    {
        if(!Head && PathType == PathTypes.Debug)
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleDebugClick();
            }
        }

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
        else if (!Head && PathType == PathTypes.Debug)
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
            newTarget = GetNewTarget();
            if (newTarget != invalidPos)
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
            if(newTarget == invalidPos)
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

    Vector2 debugTarget = Utility.HoneycombPathfinding.invalidPos;
    Transform debugTargetTransform = null;
    private Vector2 GetNextDebugTarget()
    {
        Vector3 target = transform.position;
        if (Input.GetMouseButton(0))
        {
            target = HandleDebugClick();
        }
        else if (debugTargetTransform)
        {
            target = SetDebugTarget(debugTargetTransform.position);
        }
        else if (debugTarget != invalidPos)
        {
            if(Vector2.Distance(transform.position, debugTarget) > 1)
            {
                target = SetDebugTarget(debugTarget);
            }
            else
            {
                debugTarget = invalidPos;
            }
        }
        else
        {
            //paused = true;
            
            
            GameObject player = LevelHandler.singleton.Player ? LevelHandler.singleton.Player.gameObject : null;
            if(player && Vector2.Distance(player.transform.position, transform.position) > roamingRadius)
            {
                target = SetDebugTarget(player.transform.position);
            }
            else
            {
                List<HoneycombPos> otherPos = GetPillapillarsPos();
                HoneycombPos snakeHex = player ? Utility.Honeycomb.WorldPointToHoneycombGrid(player.transform.position) : Utility.Honeycomb.WorldPointToHoneycombGrid(transform.position);
                Vector2 beeTargetPos = GetBeeTarget(snakeHex, otherPos);

                if (beeTargetPos != invalidPos)
                {
                    SetDebugTarget(beeTargetPos);
                    return beeTargetPos;
                }
                else paused = true;
            }
            
        }
        return target;
    }
    private Vector2 HandleDebugClick()
    {
        Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        debugTarget = target;

        RaycastHit2D hit = Physics2D.Raycast(target, Vector2.zero);
        if (hit && hit.transform.GetComponent<Insect>())
        {
            debugTargetTransform = hit.transform;
        }
        else
        {
            debugTargetTransform = null;
        }

        target = SetDebugTarget(target);
        return target;
    }
    private Vector2 SetDebugTarget(Vector2 debugTarget)
    {
        HoneycombPos clickHex = Utility.Honeycomb.WorldPointToHoneycombGrid(debugTarget);
        HoneycombPos snakeHex = Utility.Honeycomb.WorldPointToHoneycombGrid(transform.position);
        Vector2 target = Utility.HoneycombPathfinding.FindPathToHoneycomb(snakeHex, clickHex, GetPillapillarsPos(), invalidVarieties);
        Debug.Log($"startHex:{snakeHex} clickHex{clickHex} targetHex:{Utility.Honeycomb.WorldPointToHoneycombGrid(target)}");

        //-------------------- clear remaining points??? ----------------------
        Path.Add(target);
        paused = false;
        return target;
    }

    


    void FindFood()
    {

    }

    private Vector2 GetNewTarget()
    {

        if (!Head)
        {
            List<HoneycombPos> otherPos = GetPillapillarsPos();
            HoneycombPos snakeHex = Utility.Honeycomb.WorldPointToHoneycombGrid(transform.position);

            Vector2 playerTargetPos = GetPlayerTarget(snakeHex, otherPos);
            if (playerTargetPos != invalidPos) return playerTargetPos;

            Vector2 beeTargetPos = GetBeeTarget(snakeHex, otherPos);
            if (beeTargetPos != invalidPos) return beeTargetPos;

            // ----------------------------- Return to Egg Radius or Trapped || Random ---------------------------------------
            HoneycombPos eggHex = Utility.Honeycomb.WorldPointToHoneycombGrid(egg.transform.position);
            Vector2 newTarget = getRandomLoc();
            if (Utility.Honeycomb.DistanceBetweenHoneycomb(snakeHex, eggHex) > roamingRadius)
            {
                Debug.Log("-------------- return to egg -----------------");
                newTarget = Utility.HoneycombPathfinding.FindPathToHoneycomb(snakeHex, eggHex, otherPos, invalidVarieties);
            }
            else
            {
                Debug.Log("-------------- random location target -----------------");

                if (newTarget == invalidPos)
                {
                    Debug.Log("-------------- return to egg -----------------");

                    newTarget = Utility.HoneycombPathfinding.FindPathToHoneycomb(snakeHex, eggHex, otherPos, invalidVarieties);

                }
            }
            return newTarget;

        }
        else
        {
            return invalidPos;
        }
    }

    private Vector2 GetPlayerTarget(HoneycombPos snakeHex, List<HoneycombPos> otherPos)
    {
        GameObject player = LevelHandler.singleton.Player ? LevelHandler.singleton.Player.gameObject : null;
        if (player)
        {
            HoneycombPos playerHex = Utility.Honeycomb.WorldPointToHoneycombGrid(player.transform.position);
            if (true && player && Utility.Honeycomb.DistanceBetweenHoneycomb(playerHex, snakeHex) < AttackRadius && Map.StaticMap.GetHoneycomb((int)playerHex.x, (int)playerHex.y).LocationType == HoneycombTypes.Variety.Chamber)
            {
                HoneycombPos playerGridHex = playerHex;
                HoneycombPos startGridHex = snakeHex;
                Vector2 playerGridPos = Utility.HoneycombPathfinding.FindPathToHoneycomb(startGridHex, playerGridHex, otherPos, invalidVarieties);
                if (Utility.HoneycombPathfinding.CheckValidNewTarget(snakeHex.worldPos, playerGridPos, Direction))
                {
                    Debug.Log("-------------- player target -----------------");
                    return playerGridPos;
                }

            }

        }
        return invalidPos;
    }

    private Vector2 GetBeeTarget(HoneycombPos snakeHex, List<HoneycombPos> otherPos)
    {
        EnemyPhysics[] bees = GetClosestBee();

        if (bees.Length > 0)
        {
            foreach (EnemyPhysics bee in bees)
            {
                HoneycombPos beeHex = Utility.Honeycomb.WorldPointToHoneycombGrid(bee.transform.position);
                if (Utility.Honeycomb.DistanceBetweenHoneycomb(beeHex, snakeHex) < AttackRadius * 2)
                {
                    Vector2 beePos = Utility.HoneycombPathfinding.FindPathToHoneycomb(snakeHex, beeHex, otherPos, invalidVarieties);
                    if (Utility.HoneycombPathfinding.CheckValidNewTarget(snakeHex.worldPos, beePos, Direction))
                    {
                        Debug.Log("-------------- bee target -----------------");
                        return beePos;
                    }
                }
            }

        }
        return invalidPos;
    }

    private Vector2 getRandomLoc()
    {
        Debug.Log("getRandomLoc");

        int ranDir = Random.Range(0, 2);
        int otherDir = 0;

        MapHoneycomb newTarget;
        List<HoneycombPos> otherPos = GetPillapillarsPos();

        if (ranDir == 0)
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
                return invalidPos;
            }
        }

    }

    private MapHoneycomb GetRandomLoc(int turn, List<HoneycombPos> otherPos)
    {
        int randDist = Random.Range(1, 4) * 3;
        HoneycombDir newDirection;
        newDirection = Utility.HoneycombPathfinding.GetNewDirection(Direction, turn);

        return Utility.HoneycombPathfinding.GetHoneycombFreePath(Utility.Honeycomb.WorldPointToHoneycombGrid(Target), newDirection, randDist, otherPos, invalidVarieties);
    }

    private void setPath(List<HoneycombVectors> path)
    {
        Path.Clear();
        //Path.Add(Utility.HoneycombGridToWorldPostion((Utility.WorldToHoneycomb(transform.position))));
        Target = Utility.Honeycomb.HoneycombGridToWorldPostion(Utility.HoneycombPathfinding.GetHoneycombInDirection(Utility.Honeycomb.WorldPointToHoneycombGrid(transform.position), path[0].Direction, path[0].Distance));
        Path.Add(Target);
        for (int i = 1; i < path.Count; i += 1)
        {
            Path.Add(Utility.Honeycomb.HoneycombGridToWorldPostion(Utility.HoneycombPathfinding.GetHoneycombInDirection(Utility.Honeycomb.WorldPointToHoneycombGrid(Path[i - 1]), path[i].Direction, path[i].Distance)));
        }
        pointSnake();
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
            if(LevelHandler.singleton.PillapillerSpawnTail(this))spawnTail((int)MaxHealth);
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

    private void Feed(int energy)
    {
        feedingCount++;
        if(MaxHealth - Health < energy)
        {
            energy = energy - (int)(MaxHealth - Health);
            Health = MaxHealth;
            if (Tail) Tail.Feed(energy);
            else
            {
                spawnTail(energy);
            }
        }
        else
        {
            Health += energy;
        }
        takeDamageAnimation();

        CheckPetStatus();
    }

    private void CheckPetStatus()
    {
        if(feedingCount >= PetActiveCount)
        {
            PathType = PathTypes.Debug;
        }
    }
    
    private void spawnTail(int health)
    {
        if (Tail) Tail.spawnTail(health);
        else
        {
            PillapillarController tail = SpawnPillipallar(egg, transform.position - (-transform.right.normalized)*Time.deltaTime, transform.parent);

            Tail = tail;
            tail.Head = this;

            tail.PathType = TailPathType;
            tail.Health = health;
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
        HandleInsectGroupDeath();
        Destroy(gameObject);
    }

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log(collision.gameObject.name);
        if (!Head && collision.gameObject.GetComponent<PillapillarController>())
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
                Feed((int)MaxHealth);
            }
            else
            {
                TakeDamage(1);
            }

            collision.GetComponent<EnemyPhysics>().HandleDeath();
        }
        else if (collision.CompareTag("Honeycomb"))
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
            LevelHandler.singleton.EnemyDeath(gameObject);
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

    public override void Respawn()
    {
        RespawnHead();
        RespawnTail();
    }

    public override void Despawn()
    {
        DespawnHead();
        DespawnTail();

    }

    protected override bool CheckDespawn()
    {
        return CheckDespawnHead() && CheckDespawnTail();
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

    
}
[System.Serializable]
public class HoneycombVectors
{
    public HoneycombDir Direction;
    public int Distance;
}
