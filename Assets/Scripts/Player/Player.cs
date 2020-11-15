using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateKraft;
using System;

public class Player : MonoBehaviour
{
    public static Player Instance;

    [Header("Auto Aim")]
    public float AutoAimAngle;
    public float AutoAimMaxDistance;

    public StateMachine MovementMachine;
    public Transform Model;
    [Header("Collision")]
    public SphereCollider Collider;
    public LayerMask CollisionLayers;
    [Header("Input")]
    public float MinInput;
    [Header("Utility")]
    public Vector3 Velocity;
    public float TimeScale = 1.0f;
    public AudioSource ScratchSound;
    public AudioSource BoosterAudioSource;

    public static Vector3 Forward => Instance.Model.forward;
    public static Vector3 Up => Instance.Model.up;
    public static Quaternion Rotation => Instance.Model.rotation;

    private void Start()
    {
        Instance = this;
        MovementMachine.Initialize(this);
    }
    private void Update()
    {
        MovementMachine.Update();
    }

    public RaycastHit Cast()
    {
        return Cast(Velocity, 10000.0f);
    }
    public RaycastHit Cast(Vector3 direction, float distance)
    {
        Physics.SphereCast(transform.position + Collider.center, Collider.radius, direction, out RaycastHit hit, distance, CollisionLayers);
        return hit;
    }

    public void SetForward(Vector3 forward)
    {
        Model.forward = forward;
        Velocity = forward;
    }
}
