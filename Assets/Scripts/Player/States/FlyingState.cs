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

    public override void StateUpdate()
    {
        //Input
        float acceleration = Input.GetAxisRaw("Acceleration");
        float pitch = Input.GetAxisRaw("Pitch");
        float roll = Input.GetAxisRaw("Roll");
        float yaw = Input.GetAxisRaw("Yaw");

        //Acceleration
        if(acceleration > Player.MinInput && Speed < MaxSpeed)
        {
            Speed += acceleration * Acceleration * DeltaTime;
        }
        else if(acceleration < -Player.MinInput && Speed > BaseSpeed)
        {
            Speed += acceleration * Deceleration * DeltaTime;
        }
        else if(Mathf.Abs(acceleration) < Player.MinInput)
        {
            Speed = Mathf.Lerp(Speed, BaseSpeed, SpeedLerp * DeltaTime);
        }

        //Rotation
        if(Mathf.Abs(pitch) > Player.MinInput)
        {
            transform.Rotate(transform.right, pitch * PitchSpeed * DeltaTime, Space.World);
        }
        if (Mathf.Abs(roll) > Player.MinInput)
        {
            transform.Rotate(transform.forward, roll * RollSpeed * DeltaTime, Space.World);
        }
        float strafeVelocity = 0.0f;
        if (Mathf.Abs(yaw) > Player.MinInput)
        {
            StrafeSpeed = Mathf.SmoothDamp(StrafeSpeed, Mathf.Sign(yaw) * MaxStrafeSpeed, ref strafeVelocity, StrafeSmoothTime, 10000.0f, DeltaTime);
        }
        else
        {
            StrafeSpeed = Mathf.SmoothDamp(StrafeSpeed, 0.0f, ref strafeVelocity, StrafeDecelerationSmoothTime, 100000.0f, DeltaTime);
        }


        //Movement
        transform.position += ((transform.forward * Speed) + (transform.right * StrafeSpeed)) * DeltaTime;
    }
}
