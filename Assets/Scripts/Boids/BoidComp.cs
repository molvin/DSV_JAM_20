using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidComp : MonoBehaviour
{
    [HideInInspector]
    public Vector3 AverageFlockHeading;
    [HideInInspector]
    public Vector3 CentreOfFlockmates;
    [HideInInspector]
    public Vector3 AverageAvoidanceHeading;
    [HideInInspector]
    public int NumPerceivedFlockmates;

    public Transform Target;

    private Vector3 Velocity;
    private Vector3 WanderTarget;
    private SphereCollider Collider;

    private BoidSettings Settings => BoidsManager.Instance.Settings;

    private void Start()
    {
        Velocity = Random.onUnitSphere * Settings.MaxSpeed;
        WanderTarget = transform.position;
        Collider = GetComponent<SphereCollider>();
    }
    public void UpdateBoy()
    {
        Vector3 Acceleration = Wander() * Settings.WanderWeigt;
        if (Target)
        {
            Vector3 Offset = Target.position - transform.position;
            Acceleration = SteerThowards(Offset) * Settings.TargetWeight; 
        }

        if (NumPerceivedFlockmates != 0)
        {
            CentreOfFlockmates /= NumPerceivedFlockmates;

            Vector3 OffsetToFlockCentre = CentreOfFlockmates - transform.position;

            Vector3 Alignment = SteerThowards(AverageFlockHeading) * Settings.AlingWeight;
            Vector3 Cohesion = SteerThowards(OffsetToFlockCentre) * Settings.CohesionWeight;
            Vector3 Separation = SteerThowards(AverageAvoidanceHeading) * Settings.SeperateWeight;

            Acceleration += Alignment;
            Acceleration += Cohesion;
            Acceleration += Separation;
        }

        if (IsHeadingForCollision())
        {
            Vector3 AvoidDirection = GetAvoidDirection();
            Vector3 AvoidForce = SteerThowards(AvoidDirection) * Settings.AvoidCollisionWeight;
            Acceleration += AvoidForce;
        }

        Velocity += Acceleration * Time.deltaTime;
        float speed = Velocity.magnitude;
        Vector3 Direction = Velocity / speed;
        speed = Mathf.Clamp(speed, Settings.MinSpeed, Settings.MaxSpeed);
        Velocity = Direction * speed;

        transform.position += Velocity * Time.deltaTime;
        transform.forward = Direction;
        FixCollision();
    }

    private void FixCollision()
    {
        foreach (var coll in Physics.OverlapSphere(transform.position, Collider.radius, Settings.ObstacleMask))
        {
            Vector3 Direction;
            float Distance;
            Physics.ComputePenetration(Collider, transform.position, transform.rotation, coll, coll.transform.position, coll.transform.rotation, out Direction, out Distance);
            transform.position += Direction * Distance;
        }
    }
    public Vector3 Wander()
    {
        WanderTarget += Random.insideUnitSphere * Settings.WanderJitter;
        WanderTarget.Normalize();
        WanderTarget *= Settings.WanderRadius;

        return WanderTarget + transform.forward * Settings.WanderDistance;
    }
    Vector3 SteerThowards(Vector3 Vector)
    {
        Vector3 v = Vector.normalized * Settings.MaxSpeed - Velocity;
        return Vector3.ClampMagnitude(v, Settings.MaxSteerForce);
    }
    private Vector3 GetAvoidDirection()
    {
        Vector3[] Directions = BoidHelper.Directions;
        for (int i = 0; i < Directions.Length; i++)
        {
            Vector3 direction = transform.TransformDirection(Directions[i]);
            if (!Physics.SphereCast(new Ray(transform.position, direction), Settings.BoundsRadius, Settings.CollisionAvoidDistance, Settings.ObstacleMask))
            {
                return direction;
            }
        }
        return transform.forward;
    }

    private bool IsHeadingForCollision()
    {
        RaycastHit _hit;
        return Physics.SphereCast(transform.position, Settings.BoundsRadius, transform.forward, out _hit, Settings.CollisionAvoidDistance, Settings.ObstacleMask);
    }
}
