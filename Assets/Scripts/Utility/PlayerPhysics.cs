using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PlayerPhysics
{
    private const int EscapeCount = 100;
    
    public static HitData PreventCollision(Func<RaycastHit> raycastFunction, ref Vector3 velocity, Transform transform, float deltaTime, float skinWidth = 0.0f, float bounciness = 0.0f)
    {
        RaycastHit hit;
        float impactVelo = 0.0f;
        List<RaycastHit> raycastHits = new List<RaycastHit>();
        for(int i = 0; i < EscapeCount && (hit = raycastFunction()).collider != null; i++)
        {
            float distanceToCorrect = skinWidth / Vector3.Dot(velocity.normalized, hit.normal);
            float distanceToMove = hit.distance + distanceToCorrect;

            if (distanceToMove > velocity.magnitude * deltaTime)
                break;
        
            raycastHits.Add(hit);
            if (distanceToMove > 0.0f)
                transform.position += velocity.normalized * distanceToMove;
            Vector3 normalForce = CalculateNormalForce(hit.normal, velocity) * (1.0f + bounciness);
            impactVelo += normalForce.magnitude;
            velocity += normalForce;
        }

        HitData h = new HitData(raycastHits);
        return h;
    }
    public static Vector3 CalculateNormalForce(Vector3 normal, Vector3 velocity)
    {
        float dot = Vector3.Dot(velocity, normal.normalized);
        return -normal.normalized * (dot > 0.0f ? 0 : dot);
    }

    public struct HitData
    {
        public List<RaycastHit> Hits;
        public Vector3 Normal;
        public float SurfaceAngle;
        public float ImpactVelocity;
        public bool Hit => Hits != null && Hits.Count > 0;

        public HitData(List<RaycastHit> hits)
        {
            Hits = hits;
            SurfaceAngle = 0.0f;
            Normal = Vector3.zero;
            ImpactVelocity = 0.0f;
            
            if (hits == null || hits.Count == 0) return;
            Vector3 averageNormal = Hits.Aggregate(new Vector3(), (sum, hit) => sum += hit.normal) / Hits.Count;
            Normal = averageNormal;
            SurfaceAngle = Vector3.Angle(Vector3.up, averageNormal);
        }
    } 

}
