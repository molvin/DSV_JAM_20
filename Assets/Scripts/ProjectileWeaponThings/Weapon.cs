using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    [SerializeField] private ProjectileData m_projectileData;
    [HideInInspector] public ObjectPool m_objectPool;
    public void FireProjectile(Vector3 Dir)
    {
        Projectile Projectile = m_objectPool.rentObject(ObjectPool.ObjectType.Projectile).GetComponent<Projectile>();
        Projectile.InitializeProjectile(m_projectileData, transform.position, Quaternion.LookRotation(transform.forward));
    }
}
