using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StubPlayer : MonoBehaviour
{
    public float Acceleration;
    public float Deceleration;

    public Vector3 Velocity;

    private void Update()
    {
        //Vector3 Move = Input.GetAxisRaw("Horizontal") * Vector3.right + Input.GetAxisRaw("Vertical") * Vector3.forward;
        Vector3 Move = Vector3.zero;
        Move.Normalize();

        Velocity += Move * Acceleration * Time.deltaTime;
        transform.position += Velocity * Time.deltaTime;
        Velocity *= Mathf.Pow(Deceleration, Time.deltaTime);
    }
    private void LateUpdate()
    {
        if (Velocity.magnitude > 0.00001f)
            transform.forward = Velocity.normalized;
    }
}
