using UnityEngine;

[CreateAssetMenu(fileName ="ProjectileData")]
public class ProjectileData : ScriptableObject
{
    public float projectileRadius;
    public float travelSpeed;
    public float explosionRadius;
    public float firingCooldown = 1f;
    public float damage = 100;

    public LayerMask targetLayers;
}
