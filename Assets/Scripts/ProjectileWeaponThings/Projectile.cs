using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private ProjectileData projectileData;
    private Collider[] ColliderBuffer = new Collider[5];
    [HideInInspector] public ObjectPool objectPool;

    public void InitializeProjectile(ProjectileData ProjectileData, Vector3 Pos, Quaternion Dir, ObjectPool ObjectPool)
    {
        projectileData = ProjectileData;
        transform.rotation = Dir;
        transform.position = Pos;
        gameObject.SetActive(true);
        objectPool = ObjectPool;
    }
    private void Update()
    {
        Physics.SphereCast(transform.position, projectileData.projectileRadius, transform.forward, out RaycastHit hit, projectileData.travelSpeed * Time.deltaTime, projectileData.targetLayers);
        if(hit.transform != null)
        {
            //Do the explosionThing
            GameObject explosionObject = objectPool.rentObject(ObjectPool.ObjectType.ImpactVFX);
            explosionObject.transform.position = hit.point;
            explosionObject.transform.localScale = Vector3.one * projectileData.explosionRadius;

            //deal le dmg
            int CollidersHit = Physics.OverlapSphereNonAlloc(hit.point, projectileData.explosionRadius, ColliderBuffer, projectileData.targetLayers);
            if (CollidersHit > 0)
            {
                for (int i = 0; i < CollidersHit; i++)
                {
                    if (ColliderBuffer[i].TryGetComponent(out IDestructible DestructibleThing))
                    {
                        DestructibleThing.DestroyThis();
                    }
                }
                gameObject.SetActive(false);
            }
        }
        
        else
        {
            transform.Translate(transform.forward * projectileData.travelSpeed * Time.deltaTime);

        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, projectileData.explosionRadius);
    }
}
