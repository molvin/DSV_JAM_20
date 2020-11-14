using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : Weapon
{
    private const string k_FireString = "Fire1";

    void Update()
    {
        if (Input.GetButton(k_FireString) && firingCooldownTime < Time.time)
        {
            FireProjectile(transform.forward, transform.position);
        }
    }
}
