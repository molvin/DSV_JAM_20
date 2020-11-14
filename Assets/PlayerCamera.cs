using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Vector3 Offset;
    public Vector3 LookAtOffset;
    public float SmoothTime;
    public Player Player;

    private Vector3 velocity;

    private void Start()
    {
        transform.SetParent(null);
    }
    private void LateUpdate()
    {
        Vector3 targetPoint = Player.transform.TransformPoint(Offset);

        transform.position = Vector3.SmoothDamp(transform.position, targetPoint, ref velocity, SmoothTime);

        transform.LookAt(Player.transform.TransformPoint(LookAtOffset));
    }

}
