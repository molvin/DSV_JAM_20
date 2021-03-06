﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private ProjectileData m_ProjectileData;
    private Collider[] ColliderBuffer = new Collider[5];
    private ObjectPool m_ObjectPool;
    private float m_TravelledDistance;
    public TrailRenderer LR;
    public void InitializeProjectile(ProjectileData ProjectileData, Vector3 Pos, Quaternion Dir)
    {
        m_ProjectileData = ProjectileData;
        transform.rotation = Dir;
        transform.position = Pos;
        gameObject.SetActive(true);
        m_TravelledDistance = 0f;
        transform.localScale = Vector3.one * ProjectileData.projectileRadius;
        LR.Clear();
    }
    private void Start()
    {
        m_ObjectPool = ObjectPool.Instance;
    }
    private void Update()
    {
        Physics.SphereCast(transform.position, m_ProjectileData.projectileRadius, transform.forward, out RaycastHit hit, m_ProjectileData.travelSpeed * Time.deltaTime, m_ProjectileData.targetLayers);
        if(hit.transform != null)
        {
            //Do the ExplosionVFX
            GameObject explosionVFXObject = m_ObjectPool.rentObject(ObjectPool.ObjectType.ImpactVFX);
            explosionVFXObject.transform.position = hit.point;
            explosionVFXObject.transform.localScale = (Vector3.one * m_ProjectileData.explosionRadius)/1.1f ;
            explosionVFXObject.transform.rotation = Quaternion.LookRotation(hit.normal);

            GameObject explosionSFXObject = m_ObjectPool.rentObject(ObjectPool.ObjectType.ImpactSFX);
            explosionSFXObject.transform.position = hit.point;

            //deal le dmg
            int CollidersHit = Physics.OverlapSphereNonAlloc(hit.point, m_ProjectileData.explosionRadius, ColliderBuffer, m_ProjectileData.targetLayers);
            if (CollidersHit > 0)
            {
                for (int i = 0; i < CollidersHit; i++)
                {
                    if (ColliderBuffer[i].TryGetComponent(out IDamageable damageable))
                    {
                        damageable.TakeDamage(m_ProjectileData.damage);
                    }
                }
               
            }
            gameObject.SetActive(false);
        }
        
        else
        {
            float travelledDistanceThisFrame = m_ProjectileData.travelSpeed * Time.deltaTime;
            transform.Translate(Vector3.forward * travelledDistanceThisFrame);

            m_TravelledDistance += travelledDistanceThisFrame;
            if (m_TravelledDistance >= m_ProjectileData.maxTravelDistance)
                gameObject.SetActive(false);

        }

    }
    

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, m_ProjectileData.projectileRadius);
    }
}
