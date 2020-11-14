using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : PlayerState
{
    public override void Enter()
    {
        Player.Velocity = Vector3.zero;
    }
}
