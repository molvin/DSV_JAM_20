﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : Weapon
{
    private const string k_FireString = "Fire1";
    private const string k_SwitchWeaponString = "SwitchWeapon";
    private ProjectileData m_CurrentWeapon;
    private ProjectileData m_SecondaryWeapon;
    void Update()
    {
        if (Input.GetButtonDown(k_FireString) && firingCooldownTime < Time.time)
        {
            FireProjectile(FindAutoAim(), transform.position);
        }

    }

    public Vector3 FindAutoAim()
    {
        foreach (var b in BoidsManager.Instance.Boids)
        {
            Vector3 direction = b.transform.position - transform.position;
            if (Vector3.Dot(transform.forward, direction.normalized) > .95)
            {
                return direction;
            }
        }
        return transform.forward;
    }
}
