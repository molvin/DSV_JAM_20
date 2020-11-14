using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StateKraft;

public class PlayerState : State
{
    [HideInInspector] public Player Player;
    public float DeltaTime => Player.TimeScale * Time.unscaledDeltaTime;

    public override void Initialize(object owner)
    {
        base.Initialize(owner);
        Player = (Player) owner;
    }
}