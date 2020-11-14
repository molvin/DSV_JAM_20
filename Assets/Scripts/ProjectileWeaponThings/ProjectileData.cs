using UnityEngine;

[CreateAssetMenu(fileName ="ProjectileData")]
public class ProjectileData : ScriptableObject
{
    public float bulletRadius;
    public float bulletlength;
    public float travelSpeed;
    public float impactRadius;

    public LayerMask targetLayers;
}
