using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateKraft;

public class Player : MonoBehaviour
{
    public StateMachine MovementMachine;

    [Header("Collision")]
    public SphereCollider Collider;
    public LayerMask CollisionLayers;
    [Header("Input")]
    public float MinInput;
    [Header("Utility")]
    public Vector3 Velocity;
    public float TimeScale = 1.0f;

    private void Start()
    {
        MovementMachine.Initialize(this);
    }
    private void Update()
    {
        MovementMachine.Update();
    }

    public RaycastHit Cast()
    {
        return Cast(Velocity.normalized, 10000.0f);
    }
    public RaycastHit Cast(Vector3 direction, float distance)
    {
        Physics.SphereCast(transform.position, Collider.radius, direction, out RaycastHit hit, distance, CollisionLayers);
        return hit;
    }

}
