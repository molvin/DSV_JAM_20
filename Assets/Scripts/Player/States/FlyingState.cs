using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingState : PlayerState
{
    [Header("Speed")]
    public float MaxSpeed;
    public float BoostSpeed;
    [Header("Acceleration")]
    public float Acceleration;
    public float Deceleration;
    public float SpeedLerp;
    [Header("Strafe")]
    public float MaxStrafeSpeed = 10;
    public float StrafeSmoothTime = 0.1f;
    public float StrafeDecelerationSmoothTime = 0.1f;
    [Header("Rotation")]
    public float PitchUpSpeed;
    public float PitchDownSpeed;
    public float RollSpeed;
    public float YawSpeed;
    [Header("Current Speed")]
    public float Speed;
    public float StrafeSpeed;

    private Transform Model => Player.Model;

    public override void Enter()
    {
        Player.Velocity = Model.forward;
    }
        
    public override void StateUpdate()
    {
        //Input
        float acceleration = Input.GetAxisRaw("Acceleration");
        float pitch = Input.GetAxisRaw("Pitch");
        float roll = Input.GetAxisRaw("Roll");
        float yaw = Input.GetAxisRaw("Yaw");
        float maxSpeed = Input.GetButton("Boost") ? BoostSpeed : MaxSpeed;

        //Acceleration
        if(Player.Velocity.magnitude < maxSpeed)
        {
            Player.Velocity += Acceleration * DeltaTime * Model.forward;
        }
        else if(Player.Velocity.magnitude > maxSpeed)
        {
            Player.Velocity = Vector3.Lerp(Player.Velocity, Model.forward * maxSpeed, SpeedLerp * DeltaTime);
        }

        //Rotation
        if(Mathf.Abs(pitch) > Player.MinInput)
        {
            float speed = pitch > 0.0f ? PitchDownSpeed : PitchUpSpeed;
            Model.Rotate(Model.right, pitch * speed * DeltaTime, Space.World);
            Player.Velocity = Quaternion.AngleAxis(pitch * speed * DeltaTime, Model.right) * Player.Velocity;
        }
        if (Mathf.Abs(roll) > Player.MinInput)
        {
            Model.Rotate(Model.forward, roll * RollSpeed * DeltaTime, Space.World);
        }
        if(Mathf.Abs(yaw) > Player.MinInput)
        {
            Model.Rotate(Model.up, pitch * YawSpeed * DeltaTime, Space.World);
            Player.Velocity = Quaternion.AngleAxis(yaw * YawSpeed * DeltaTime, Model.up) * Player.Velocity;
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
        Debug.DrawRay(transform.position, Player.Velocity, Color.magenta);
        /*
        //Strafing
        float strafeVelocity = 0.0f;
        if (Mathf.Abs(yaw) > Player.MinInput)
        {
            StrafeSpeed = Mathf.SmoothDamp(StrafeSpeed, MaxStrafeSpeed * yaw, ref strafeVelocity, StrafeSmoothTime, 10000.0f, DeltaTime);
        }
        else
        {
            StrafeSpeed = Mathf.SmoothDamp(StrafeSpeed, 0.0f, ref strafeVelocity, StrafeDecelerationSmoothTime, 10000.0f, DeltaTime);
        }

        Vector3 velocity = Model.transform.right * StrafeSpeed;
        hit = PlayerPhysics.PreventCollision(() => Player.Cast(velocity.normalized, 100000.0f), ref velocity, transform, DeltaTime, 0.03f);

        transform.position += velocity * DeltaTime;
        Debug.DrawRay(transform.position, velocity, Color.red);
        */
    }
}
