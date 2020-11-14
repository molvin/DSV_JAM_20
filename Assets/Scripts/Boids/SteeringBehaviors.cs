using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringBehaviors
{
    public float PanicDistance = 5.0f;

    private Boid Boid;
    public SteeringBehaviors(Boid Boid)
    {
        this.Boid = Boid;
    }

    public Vector3 Seek(Vector3 TargetPosition)
    {
        Vector3 DesiredVelocity = (TargetPosition - Boid.transform.position).normalized * Boid.MaxSpeed;
        return DesiredVelocity - Boid.Velocity;
    }
    public Vector3 Flee(Vector3 TargetPosition)
    {
        if (Vector3.Distance(Boid.transform.position, TargetPosition) > PanicDistance)
        {
            return Vector3.zero;
        }
        Vector3 DesiredVelocity = (Boid.transform.position - TargetPosition).normalized * Boid.MaxSpeed;
        return DesiredVelocity - Boid.Velocity;
    }
    public Vector3 Arrive(Vector3 TargetPosition, float Deceleration)
    {
        Vector3 ToTarget = TargetPosition - Boid.transform.position;

        if (ToTarget.magnitude > 0.0f)
        {
            float speed = ToTarget.magnitude / Deceleration;
            speed = Mathf.Min(speed, Boid.MaxSpeed);

            Vector3 DesiredVelocity = ToTarget.normalized * speed;
            return DesiredVelocity - Boid.Velocity;
        }

        return Vector3.zero;
    }
}
