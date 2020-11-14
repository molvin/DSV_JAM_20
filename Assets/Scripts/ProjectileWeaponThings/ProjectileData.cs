using UnityEngine;

[CreateAssetMenu(fileName ="ProjectileData")]
public class ProjectileData : ScriptableObject
{
    public float projectileRadius;
    public float travelSpeed;
    public float impactRadius;

    public LayerMask targetLayers;
}
