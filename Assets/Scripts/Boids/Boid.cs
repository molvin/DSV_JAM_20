using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public float MaxSpeed;
    public float MaxForce;

    [Header("Steering")]
    public Transform SeekTarget;

    public Vector3 Velocity;
    private SteeringBehaviors Steering;

    private void Start()
    {
        Steering = new SteeringBehaviors(this);
    }
    private void Update()
    {
        Vector3 SteeringForce = Steering.Arrive(SeekTarget.position, .9f);
        Velocity += SteeringForce * Time.deltaTime;
        Velocity.Truncate(MaxSpeed); 
        transform.position += Velocity * Time.deltaTime;
    }
    private void LateUpdate()
    {
        if (Velocity.sqrMagnitude > 0.00001f)
        {
            transform.forward = Velocity;
        }
    }
}
