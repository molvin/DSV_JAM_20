using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(ParticleSystem), typeof(AudioSource))]
public class Weapon : MonoBehaviour
{

    [SerializeField] protected ProjectileData m_MainProjectileData;
    protected ObjectPool m_objectPool;
    protected float firingCooldownTime;
    private ParticleSystem m_MuzzleFlash;
    private AudioSource m_AudioSource;
    public ObjectPool.ObjectType projectile;

    private void Start()
    {
        m_MuzzleFlash = GetComponent<ParticleSystem>();
        m_AudioSource = GetComponent<AudioSource>();
    }
    public void FireProjectile(Vector3 Dir, Vector3 Position)
    {
        Projectile Projectile = ObjectPool.Instance.rentObject(projectile).GetComponent<Projectile>();
        Projectile.InitializeProjectile(m_MainProjectileData, Position, Quaternion.LookRotation(Dir));
        firingCooldownTime = Time.time + m_MainProjectileData.firingCooldown;
        m_MuzzleFlash.Play();
        m_AudioSource.Play();
    }

}
