using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public float MaxSpeed;
    public float MaxForce;
    public float Radius;
    public float Acceleration;

    public Vector3 Velocity = Vector3.zero;
    private SteeringBehaviors Steering;
    private SphereCollider Collider;

    [Header("Steering")]
    public float PanicDistance;
    public float WanderRadius;
    public float WanderDistance;
    public float WanderJitter;
    public LayerMask RaycastLayer;
    public float AvoidDistance;
    [Header("Behavior")]
    public float Seek;
    public float Wander;
    public float NeighbourRadius;
    public float Separation;
    public float Alightment;
    public float Cohesion;

    private void Start()
    {
        Steering = new SteeringBehaviors(this);
        Collider = GetComponent<SphereCollider>();
    }
    public List<Boid> FrameNighbours;
    private void Update()
    {
        SetFrameNeighbours();

        Vector3 SteeringForce = Steering.Pursuit(Player.Instance) * Seek;
        SteeringForce += Steering.Wander() * Wander;
        SteeringForce += Steering.Separation() * Separation;
        SteeringForce += Steering.Alignment() * Alightment;
        SteeringForce += Steering.Cohesion() * Cohesion;
        Vector3 Avoidance = Steering.ObstacleAvoidance();
        if (Avoidance.magnitude > float.Epsilon)
        {
            SteeringForce = Avoidance;
        }
        Velocity += SteeringForce * Acceleration * Time.deltaTime;
        Velocity.Truncate(MaxSpeed);
        transform.position += Velocity * Time.deltaTime;
        FixCollision();
    }
    private void LateUpdate()
    {
        if (Velocity.sqrMagnitude > 0.00001f)
        {
            transform.forward = Velocity;
        }
        Debug.DrawRay(transform.position, transform.forward * 5, Color.blue);
    }
    private void FixCollision()
    {
        foreach (var coll in Physics.OverlapSphere(transform.position, Radius, RaycastLayer))
        {
            Vector3 Direction;
            float Distance;
            Physics.ComputePenetration(Collider, transform.position, transform.rotation, coll, coll.transform.position, coll.transform.rotation, out Direction, out Distance);
            transform.position += Direction * Distance;
        }
    }
    private void SetFrameNeighbours() 
    {
        FrameNighbours = new List<Boid>();
        foreach (Boid b in BoidsManager.Instance.Boids)
        {
            if (b != this)
            {
                if (Vector3.Distance(b.transform.position, transform.position) < NeighbourRadius)
                {
                    FrameNighbours.Add(b);
                }
            }
        }
    }
}
