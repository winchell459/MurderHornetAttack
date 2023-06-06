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
            HoneycombTower[] towers = FindObjectsOfType<HoneycombTower>();
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
    private int GetShieldCount()
    {
        for(int i = beeShield.Count - 1; i >= 0; i--)
        {
            if (!beeShield[i]) beeShield.RemoveAt(i);
        }
        return beeShield.Count;
    }

    public override void TakeDamage(float Damage)
    {
        Health -= Damage;
        if (Health <= 0)
        {
            FindObjectOfType<LevelHandler>().QueenDeath();
            Destroy(gameObject);
        }
    }
}
