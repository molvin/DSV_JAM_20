using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingState : PlayerState
{
    public float MaxSpeed;
    public float BaseSpeed;

    public float Acceleration;
    public float Deceleration;
    public float SpeedLerp;

    public float PitchSpeed;
    public float RollSpeed;
    public float YawSpeed;

    public float Speed;

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
        if (Mathf.Abs(yaw) > Player.MinInput)
        {

        }

        //Movement
        transform.position += transform.forward * Speed * DeltaTime;
    }
}
