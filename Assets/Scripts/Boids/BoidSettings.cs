using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings yao")]
public class BoidSettings : ScriptableObject
{
    public float MinSpeed = 2;
    public float MaxSpeed = 5;
    public float PerceptionRadius = 2.5f;
    public float AvoidanceRadius = 1;
    public float MaxSteerForce = 3;

    public float AlingWeight = 1;
    public float CohesionWeight = 1;
    public float SeperateWeight = 1;
    public float WanderWeigt = 1;

    public float TargetWeight = 1;

    public LayerMask ObstacleMask;
    public float BoundsRadius = 0.27f;
    public float AvoidCollisionWeight = 10;
    public float CollisionAvoidDistance = 5;

    public float WanderJitter = .7f;
    public float WanderRadius = 8;
    public float WanderDistance = 5;
}