using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SteeringBehaviors
{
    private Boid Boid;
    public Vector3 WanderTarget;

    private const int NumViewDirections = 300;
    private Vector3[] Directions;

    public SteeringBehaviors(Boid Boid)
    {
        this.Boid = Boid;
        WanderTarget = Boid.transform.position;
        InitializeDirections();
    }

    private void InitializeDirections()
    {
        Directions = new Vector3[NumViewDirections];
        float GoldenRatio = (1 + Mathf.Sqrt(5)) / 2;
        float AngleIncrement = Mathf.PI * 2 * GoldenRatio;

        for (int i = 0; i < NumViewDirections; i++)
        {
            float t = (float)i / NumViewDirections;
            float Inclination = Mathf.Acos(1 - 2 *t);
            float Azimuth = AngleIncrement * i;

            float x = Mathf.Sin(Inclination) * Mathf.Cos(Azimuth);
            float y = Mathf.Sin(Inclination) * Mathf.Sin(Azimuth);
            float z = Mathf.Cos(Inclination);
            Directions[i] = new Vector3(x, y, z);
        }
    }

    public Vector3 Seek(Vector3 TargetPosition)
    {
        Vector3 DesiredVelocity = (TargetPosition - Boid.transform.position).normalized * Boid.MaxSpeed;
        return DesiredVelocity - Boid.Velocity;
    }
    public Vector3 Flee(Vector3 TargetPosition)
    {
        if (Vector3.Distance(Boid.transform.position, TargetPosition) > Boid.PanicDistance)
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
    public Vector3 Pursuit(Player Evader)
    {
        Vector3 ToEvader = Evader.transform.position - Boid.transform.position;

        float RelativeHeading = Vector3.Dot(Boid.transform.forward, Evader.transform.forward);
        if (Vector3.Dot(ToEvader, Boid.transform.forward) > 0 && RelativeHeading < -0.95)
        {
            return Seek(Evader.transform.position);
        }

        float LookAheadTime = ToEvader.magnitude / (Boid.MaxSpeed + Evader.Velocity.magnitude);
        return Seek(Evader.transform.position + Evader.Velocity * LookAheadTime);
    }
    public Vector3 Evade(StubPlayer Pursuer)
    {
        Vector3 ToPursuer = Pursuer.transform.position - Boid.transform.position;

        float LookAheadTime = ToPursuer.magnitude / (Boid.MaxSpeed + Pursuer.Velocity.magnitude);
        return Flee(Pursuer.transform.position + Pursuer.Velocity * LookAheadTime);
    }
    public Vector3 Wander()
    {
        WanderTarget += Random.insideUnitSphere * Boid.WanderJitter;
        WanderTarget.Normalize();
        WanderTarget *= Boid.WanderRadius;

        return WanderTarget + Boid.transform.forward * Boid.WanderDistance;
    }
    public Vector3 ObstacleAvoidance()
    {
        if (IsHeadingForCollision())
        {
            return Seek(Boid.transform.position + GetAvoidDirection() * Boid.MaxSpeed);
        }
        return Vector3.zero;
    }

    public Vector3 Separation()
    {
        Vector3 force = Vector3.zero;
        foreach (Boid b in Boid.FrameNighbours)
        {
            Vector3 FromNeighbour = Boid.transform.position - b.transform.position;
            if (FromNeighbour.magnitude > 0.0f && FromNeighbour.magnitude < Boid.AvoidDistance)
            {
                force += FromNeighbour.normalized / FromNeighbour.magnitude;
            }
        }
        return force;
    }

    public Vector3 Alignment()
    {
        if (Boid.FrameNighbours.Count == 0)
        {
            return Vector3.zero;
        }
        Vector3 AvgHeading = Vector3.zero;
        foreach (Boid b in Boid.FrameNighbours)
        {
            AvgHeading += b.transform.forward;
        }
        AvgHeading /= Boid.FrameNighbours.Count;
        AvgHeading -= Boid.transform.forward;
        return AvgHeading;
    }

    public Vector3 Cohesion()
    {
        Vector3 CenterOfMass = Vector3.zero;
        Vector3 force = Vector3.zero;

        foreach(Boid b in Boid.FrameNighbours)
        {
            CenterOfMass += b.transform.position;
        }
        if (Boid.FrameNighbours.Count != 0)
        {
            CenterOfMass /= Boid.FrameNighbours.Count;
            force = Seek(CenterOfMass);
        }
        return force;
    }

    private bool IsHeadingForCollision()
    {
        RaycastHit _hit;
        return Physics.SphereCast(Boid.transform.position, Boid.Radius, Boid.transform.forward, out _hit, Boid.AvoidDistance, Boid.RaycastLayer);
    }

    private Vector3 GetAvoidDirection()
    {
        for (int i = 0; i < Directions.Length; i++)
        {
            Vector3 direction = Boid.transform.TransformDirection(Directions[i]);
            if (!Physics.SphereCast(new Ray(Boid.transform.position, direction), Boid.Radius, Boid.AvoidDistance, Boid.RaycastLayer))
            {
                Debug.DrawRay(Boid.transform.position, direction * Boid.AvoidDistance, Color.red);
                return direction;
            }
        }
        return Boid.transform.forward;
    }
}
