using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenController : Insect
{
    private HornetController player;
    public Transform body;
    private Transform cam;
    
    public float speed = 1;
    public float minDistance = 2;

    //public int health = 100;

    List<EnemyPhysics> beeShield = new List<EnemyPhysics>();
    public float spawnShieldTime = 10;
    private float lastSpawnShieldTime = 0;
    public int shieldMaxCount = 10;

    // Start is called before the first frame update
    void Start()
    {
        cam = FindObjectOfType<CameraController>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.up = cam.up;
        if (!player && LevelHandler.singleton.Player) player = LevelHandler.singleton.Player.GetComponent<HornetController>();

        MoveToPlayer();
        SpawnShield();
    }

    private void FixedUpdate()
    {
        foreach(EnemyPhysics bee in beeShield)
        {
            if (bee)
            {
                Vector2 normal = (transform.position - bee.transform.position);
                bee.rb.AddForce(Vector2.Perpendicular(normal.normalized) * (1 / normal.magnitude));
            }
            
        }
    }

    void MoveToPlayer()
    {
        if (player && Vector2.Distance(player.transform.position, transform.position) > minDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed*Time.deltaTime);
        }
    }

    void SpawnShield()
    {
        if(lastSpawnShieldTime + spawnShieldTime < Time.time && GetShieldCount() < shieldMaxCount)
        {
            HoneycombTower[] towers = GetClosestTowers();

            foreach(HoneycombTower tower in towers)
            {
                if (beeShield.Count < shieldMaxCount)
                {
                    beeShield.Add(tower.SpawnBee(gameObject));
                }
                else break;
                
            }
            lastSpawnShieldTime = Time.time;
        }
    }

    void ShieldAttack()
    {
        while(GetShieldCount() > 0)
        {
            BeeAttack();
        }
    }

    void BeeAttack()
    {
        EnemyPhysics bee = GetBee();
        if (bee)
        {
            bee.SetTarget(player.gameObject);
            bee.rb.AddForce((player.transform.position - bee.transform.position).normalized * 10);
        }
    }

    EnemyPhysics GetBee()
    {
        if(GetShieldCount() > 0)
        {
            EnemyPhysics bee = beeShield[0];
            beeShield.RemoveAt(0);
            return bee;
        }
        else
        {
            return null;
        }
    }
    private int GetShieldCount()
    {
        for(int i = beeShield.Count - 1; i >= 0; i--)
        {
            if (!beeShield[i]) beeShield.RemoveAt(i);
            else if (!beeShield[i].gameObject.activeSelf)
            {
                beeShield[i].SetTarget(player.gameObject);
                beeShield.RemoveAt(i);
            }
        }
        return beeShield.Count;
    }

    private HoneycombTower[] GetClosestTowers()
    {
        HoneycombTower[] towers = FindObjectsOfType<HoneycombTower>();
        for(int i = 0; i < towers.Length; i++)
        {
            for(int j = 1; j < towers.Length; j++)
            {
                if(Vector2.Distance(transform.position, towers[j-1].transform.position) > Vector2.Distance(transform.position, towers[j].transform.position))
                {
                    HoneycombTower temp = towers[j];
                    towers[j] = towers[j - 1];
                    towers[j - 1] = temp;
                }
            }
        }
        return towers;

    }

    public override void TakeDamage(float Damage)
    {
        Health -= Damage;
        if (Health <= 0)
        {
            FindObjectOfType<LevelHandler>().QueenDeath();
            Destroy(gameObject);
        }

        if(Health % 10 == 0)
        {
            ShieldAttack();
            lastSpawnShieldTime = int.MinValue;
        }
        else
        {
            BeeAttack();
        }
        
    }


}
