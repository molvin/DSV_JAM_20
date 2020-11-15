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

        if (Vector3.Dot(transform.up, Player.Up) < .8f)
        {
            return;
        }

        Vector3 ToPlayer = Player.Position - transform.position;
        float LookAheadTime = ToPlayer.magnitude / (m_MainProjectileData.travelSpeed + Player.Instance.Velocity.magnitude);
        Vector3 PredictedPlayer = Player.Position + Player.Instance.Velocity * LookAheadTime;
        Vector3 ToPredictedPlayer = PredictedPlayer - transform.position;

        if (FiringRange < ToPredictedPlayer.magnitude || Vector3.Dot(ToPredictedPlayer, Player.Forward) > 0)
        {
            return;
        }

        FireProjectile(ToPredictedPlayer, transform.position);
    }
}
