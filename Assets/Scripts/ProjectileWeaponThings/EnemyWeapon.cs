using UnityEngine;
public class EnemyWeapon : Weapon
{
    public float FiringRange;

    private void Update()
    {
        if (firingCooldownTime > Time.time)
        {
            return;
        }

        Vector3 ToPlayer = Player.Position - transform.position;
        if (FiringRange < ToPlayer.magnitude || Vector3.Dot(ToPlayer, Player.Forward) > 0)
        {
            return;
        }

        if (Vector3.Dot(transform.up, Player.Up) < .95f)
        {
            return;
        }

        FireProjectile(ToPlayer.normalized, transform.position);
    }
}
