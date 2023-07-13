using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Insect : MonoBehaviour
{
    public float CollisionDamage;
    public float Health = 1f;
    public float MaxHealth = 1f;
    public Vector2 CollisionForce;
    public enum Type { bee, spider, pillapillar, queen, ant}
    public Type type;

    //public abstract void Collision(Insect collider);
    public abstract void TakeDamage(float Damage);
    public virtual void TakeDamage(float Damage, Vector2 KickBackVelocity)
    {
        TakeDamage(Damage);
    }

    public GameObject InsectPrefab;
    public virtual Vector2 GetCollisionVelocity(Transform collidingObject, Vector2 collidingVelocity)
    {
        //return CollisionForce;
        return 25 * (collidingObject.position - transform.position).normalized; 
    }

    protected HoneycombTower[] GetClosestTowers()
    {
        HoneycombTower[] towers = FindObjectsOfType<HoneycombTower>();
        for (int i = 0; i < towers.Length; i++)
        {
            for (int j = 1; j < towers.Length; j++)
            {
                if (Vector2.Distance(transform.position, towers[j - 1].transform.position) > Vector2.Distance(transform.position, towers[j].transform.position))
                {
                    HoneycombTower temp = towers[j];
                    towers[j] = towers[j - 1];
                    towers[j - 1] = temp;
                }
            }
        }
        return towers;

    }

    protected EnemyPhysics[] GetClosestBee()
    {
        EnemyPhysics[] transforms = FindObjectsOfType<EnemyPhysics>();
        for (int i = 0; i < transforms.Length; i++)
        {
            for (int j = 1; j < transforms.Length; j++)
            {
                if (Vector2.Distance(transform.position, transforms[j - 1].transform.position) > Vector2.Distance(transform.position, transforms[j].transform.position))
                {
                    EnemyPhysics temp = transforms[j];
                    transforms[j] = transforms[j - 1];
                    transforms[j - 1] = temp;
                }
            }
        }
        return transforms;

    }
}
