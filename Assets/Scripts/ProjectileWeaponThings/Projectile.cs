using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private ProjectileData projectileData;
    private Collider[] ColliderBuffer = new Collider[5];

    public void InitializeProjectile(ProjectileData ProjectileData, Vector3 Pos, Quaternion Dir)
    {
        projectileData = ProjectileData;
        transform.rotation = Dir;
        transform.position = Pos;
        gameObject.SetActive(true);
    }
    private void Update()
    {
        Physics.SphereCast(transform.position, projectileData.projectileRadius, transform.forward, out RaycastHit hit, projectileData.travelSpeed * Time.deltaTime, projectileData.targetLayers);
        if(hit.transform != null)
        {
            int CollidersHit = Physics.OverlapSphereNonAlloc(transform.position, projectileData.explosionRadius, ColliderBuffer, projectileData.targetLayers);
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
