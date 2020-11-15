using UnityEngine;
public class EnemyWeapon : Weapon
{
    public float FiringRange;
    public LayerMask Geometry;
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

        //Ray
        Vector3 ToBoid = transform.position - Player.Position;
        bool wallInWay = Physics.Raycast(new Ray(Player.Position, ToBoid.normalized), ToBoid.magnitude, Geometry);

        if(FiringRange < ToPredictedPlayer.magnitude || Vector3.Dot(ToPredictedPlayer, Player.Forward) > 0 || wallInWay)
        {
            return;
        }

        FireProjectile(ToPredictedPlayer, transform.position);
    }
}
