using UnityEngine;
public class EnemyWeapon : Weapon
{
    public float FiringRange;
    private RaycastHit[] PlayerHit = new RaycastHit[1];

    private void Update()
    {
        Vector3 playerDir = Player.Instance.transform.position - transform.position;
        int hit = Physics.RaycastNonAlloc(transform.position, playerDir, PlayerHit, FiringRange, m_MainProjectileData.targetLayers);
        if (hit > 0 && firingCooldownTime < Time.time)
        {
            transform.parent.rotation = Quaternion.LookRotation(playerDir);
            FireProjectile(playerDir, transform.position);
        }

        
    }
}
