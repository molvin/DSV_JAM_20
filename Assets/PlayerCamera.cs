using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Vector3 Offset;
    public Vector3 LookAtOffset;
    public float SmoothTime;
    public Player Player;
    public bool followCamera;

    private Vector3 velocity;

    private void Start()
    {
        transform.SetParent(null);
    }
    private void LateUpdate()
    {
        Vector3 targetPoint = Player.Model.TransformPoint(Offset);

        transform.position = Vector3.SmoothDamp(transform.position, targetPoint, ref velocity, SmoothTime);



        Quaternion target = Quaternion.LookRotation(Player.Model.forward, followCamera ? Player.Model.up : Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, target, 0.1f);
    }

}
