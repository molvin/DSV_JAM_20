﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Weapon : MonoBehaviour
{

    [SerializeField] protected ProjectileData m_projectileData;
    protected ObjectPool m_objectPool;
    protected float firingCooldownTime;
    private ParticleSystem MuzzleFlash;

    public void FireProjectile(Vector3 Dir, Vector3 Position)
    {
        Projectile Projectile = ObjectPool.Instance.rentObject(ObjectPool.ObjectType.ProjectileVFX).GetComponent<Projectile>();
        Projectile.InitializeProjectile(m_projectileData, Position, Quaternion.LookRotation(Dir));
        firingCooldownTime = Time.time + m_projectileData.firingCooldown;
    }

}
