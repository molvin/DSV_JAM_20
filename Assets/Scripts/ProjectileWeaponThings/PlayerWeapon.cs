using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerWeapon : Weapon
{
    private const string k_FireString = "Fire1";
    private const string k_SwitchWeaponString = "SwitchWeapon";
    private ProjectileData m_CurrentWeapon;
    private ProjectileData m_SecondaryWeapon;
    void Update()
    {
        if ((Input.GetButton(k_FireString) || Input.GetAxisRaw("Shoot") > 0.6f) && firingCooldownTime < Time.time)
        {
            var direction = transform.forward;
            var tran = FindAutoAim();
            if (tran)
            {
                direction = (tran.position - transform.position).normalized;
            }

            FireProjectile(direction, transform.position);
        }

    }

    public Transform FindAutoAim()
    {
        return BoidsManager.Instance.Boids.Where(b =>
        {
            Vector3 direction = b.transform.position - transform.position;
            return direction.magnitude < Player.Instance.AutoAimMaxDistance && 
            Physics.Raycast(new Ray(transform.position, direction), direction.magnitude, Player.Instance.CollisionLayers) && 
            Vector3.Dot(transform.forward, direction.normalized) > Player.Instance.AutoAimAngle;
        }).OrderBy(b => Vector3.Distance(b.transform.position, transform.position)).Select(b => b.transform).FirstOrDefault();
    }
}
