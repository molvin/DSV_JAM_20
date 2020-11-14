using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingState : PlayerState
{
    [Header("Speed")]
    public float MaxSpeed;
    public float BaseSpeed;
    [Header("Acceleration")]
    public float Acceleration;
    public float Deceleration;
    public float SpeedLerp;
    [Header("Strafe")]
    public float MaxStrafeSpeed = 10;
    public float StrafeSmoothTime = 0.1f;
    public float StrafeDecelerationSmoothTime = 0.1f;
    [Header("Rotation")]
    public float PitchSpeed;
    public float RollSpeed;
    public float YawSpeed;
    [Header("Current Speed")]
    public float Speed;
    public float StrafeSpeed;

    private Transform Model => Player.Model;

    public override void Enter()
    {
        Player.Velocity = transform.forward;
    }

    public override void StateUpdate()
    {
        //Input
        float acceleration = Input.GetAxisRaw("Acceleration");
        float pitch = Input.GetAxisRaw("Pitch");
        float roll = Input.GetAxisRaw("Roll");
        float yaw = Input.GetAxisRaw("Yaw");

        //Acceleration
        if(acceleration > Player.MinInput && Player.Velocity.magnitude < MaxSpeed)
        {
            Player.Velocity += acceleration * Acceleration * DeltaTime * Model.forward;
        }
        else if(acceleration < -Player.MinInput && Player.Velocity.magnitude > BaseSpeed)
        {
            Player.Velocity += acceleration * Deceleration * DeltaTime * Model.forward;
        }
        else if(Mathf.Abs(acceleration) < Player.MinInput)
        {
            Player.Velocity = Mathf.Lerp(Speed, BaseSpeed, SpeedLerp * DeltaTime) * Model.forward;
        }

        //Rotation
        if(Mathf.Abs(pitch) > Player.MinInput)
        {
            Model.Rotate(Model.right, pitch * PitchSpeed * DeltaTime, Space.World);
            Player.Velocity = Quaternion.AngleAxis(pitch * PitchSpeed * DeltaTime, Model.right) * Player.Velocity;
        }
        if (Mathf.Abs(roll) > Player.MinInput)
        {
            Model.Rotate(Model.forward, roll * RollSpeed * DeltaTime, Space.World);
        }
        float strafeVelocity = 0.0f;
        if (Mathf.Abs(yaw) > Player.MinInput)
        {

        }
        else
        {

        }

        PlayerPhysics.HitData hit = PlayerPhysics.PreventCollision(Player.Cast, ref Player.Velocity, transform, DeltaTime, 0.03f);

        Vector3 euler = Model.localEulerAngles;
        float modelRoll = euler.z;
        euler.z = 0.0f;
        Model.localEulerAngles = euler;

        Model.transform.forward = Player.Velocity.normalized;
        euler = Model.localEulerAngles;
        euler.z = modelRoll;
        Model.localEulerAngles = euler;

        //Movement
        transform.position += Player.Velocity * DeltaTime;
        Debug.DrawRay(transform.position, Player.Velocity * 100.0f, Color.magenta);
    }
}
