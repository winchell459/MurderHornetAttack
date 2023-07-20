using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChamberEnemyTrigger : ChamberTrigger, IChunkObject
{
    public GameObject EnemyPrefab;
    private bool triggered;
    private GameObject TriggeredEnemy = null;
    public enum TriggerType { spawnOnTrigger, triggerOnce, soloSpawn}
    public TriggerType triggerType;
    int spawnCount = 0;

    protected override void OnExit(Collider2D collision)
    {
        triggered = false;
    }

    protected override void OnStay(Collider2D collision)
    {
        if (respawned && !triggered && (triggerType == TriggerType.spawnOnTrigger || (triggerType == TriggerType.triggerOnce && spawnCount < 1) || (triggerType == TriggerType.soloSpawn && TriggeredEnemy == null)))
        {
            spawnCount++;
            TriggeredEnemy = Instantiate(EnemyPrefab, GetSpawnLocation(), Quaternion.identity);
            respawned = false;
        }
        triggered = true;
    }

    protected virtual Vector2 GetSpawnLocation()
    {
        if (Chamber != null && Chamber.locations.Count > 0) return Chamber.locations[0];
        else return transform.position;
    }

    bool respawned = false;
    void IChunkObject.Activate()
    {
        gameObject.SetActive(true);
        if (!TriggeredEnemy)
        {
           respawned = true;
        }
       
    }

    void IChunkObject.Deactivate()
    {
        gameObject.SetActive(false);
    }
}
