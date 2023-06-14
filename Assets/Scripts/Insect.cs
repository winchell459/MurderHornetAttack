using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Insect : MonoBehaviour
{
    public float CollisionDamage;
    public float Health = 1f;
    public float MaxHealth = 1f;
    public Vector2 CollisionForce;
    public enum Type { bee, spider, pillapillar, queen}
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
}
