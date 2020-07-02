﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Insect : MonoBehaviour
{
    public float CollisionDamage;
    public float Health = 1f;

    //public abstract void Collision(Insect collider);
    public abstract void Collision(float damage);
}
