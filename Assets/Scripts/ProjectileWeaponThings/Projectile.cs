using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private ProjectileData projectileData;
    Collider[] ColliderBuffer = new Collider[5];

    public void InitializeProjectile(ProjectileData ProjectileData, Vector3 Pos, Quaternion Dir)
    {
        projectileData = ProjectileData;
        transform.rotation = Dir;
        transform.position = Pos;
        gameObject.SetActive(true);
    }
    private void Update()
    {
        transform.Translate(transform.forward * projectileData.travelSpeed * Time.deltaTime);
        Physics.OverlapSphereNonAlloc(transform.position, projectileData.impactRadius, ColliderBuffer, projectileData.targetLayers);
        if (ColliderBuffer.Length > 0)
        {
            for(int i = 0; i < ColliderBuffer.Length; i++)
            {
                if(ColliderBuffer[i].TryGetComponent<IDestructible>(out IDestructible DestructibleThing)){
                    DestructibleThing.DestroyThis();
                }
            }
            gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, projectileData.impactRadius);
    }
}
