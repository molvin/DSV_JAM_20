using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

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
    [Header("Bump Damage")]
    public float MinDamage;
    public float MaxDamage;
    public float MinBumpSpeed;
    public float MaxBumpSpeed;

    private PostProcessVolume volume;
    private ChromaticAberration chromatic = null;

    private Transform Model => Player.Model;

    public override void Enter()
    {
        volume = Camera.main.GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings(out chromatic);
        Player.Velocity = Model.forward;
    }
        
    public override void StateUpdate()
    {
        //Input
        float topYaw = Input.GetAxisRaw("Acceleration");
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
        if(Input.GetButton("Boost")){
            chromatic.intensity.value = Mathf.Lerp(chromatic.intensity.value, 10f, 0.1f);
        }
        else{
            chromatic.intensity.value = Mathf.Lerp(chromatic.intensity.value, 0f, 0.1f);
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
            Model.Rotate(Model.up, yaw * YawSpeed * DeltaTime, Space.World);
            Player.Velocity = Quaternion.AngleAxis(yaw * YawSpeed * DeltaTime, Model.up) * Player.Velocity;
        }
        if (Mathf.Abs(topYaw) > Player.MinInput)
        {
            Model.Rotate(Model.right, topYaw * YawSpeed * DeltaTime, Space.World);
            Player.Velocity = Quaternion.AngleAxis(topYaw * YawSpeed * DeltaTime, Model.right) * Player.Velocity;
        }

        PlayerPhysics.HitData hit = PlayerPhysics.PreventCollision(Player.Cast, ref Player.Velocity, transform, DeltaTime, 0.03f);
        if(hit.Hit)
        {
            float damage = Mathf.Lerp(MinDamage, MaxDamage, Mathf.Clamp01(hit.ImpactVelocity - MinBumpSpeed / (MaxBumpSpeed - MinBumpSpeed)));
            gameObject.GetComponent<Health>().TakeDamage(damage);
            Player.ScratchSound.Play();
        }
        

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
    }
}
