using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;
    [Serializable] private class Pool
    {
        [HideInInspector] public GameObject[] poolArray;
        public int poolSize = 10;
        public GameObject prefab;

        public void InitializePool(Transform parent)
        {
            poolArray = new GameObject[poolSize];
            for (int i = 0; i < poolSize; i++)
                poolArray[i] = Instantiate(prefab, parent);
            prefab.SetActive(false);
        }
    }
    [SerializeField] private Pool m_projectilePool;
    [SerializeField] private Pool m_EnemyPool;
    [Header("VFX")]
    [SerializeField] private Pool m_ImpactVFXPool;
    [SerializeField] private Pool m_EnemyExplosionVFXPool;
    [Header("SFX")]
    [SerializeField] private Pool m_ImpactSFXPool;
    [SerializeField] private Pool m_EnemyExplosionSFXPool;

    public enum ObjectType
    {
        ProjectileVFX,
        ImpactVFX,
        EnemyExplosionVFX,
        Enemy,
        ImpactSFX,
        EnemyDeathSFX,
    }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        m_projectilePool.InitializePool(transform);


        m_EnemyPool.InitializePool(transform);
        m_ImpactVFXPool.InitializePool(transform);
        m_EnemyExplosionVFXPool.InitializePool(transform);

        m_EnemyExplosionSFXPool.InitializePool(transform);
        m_ImpactSFXPool.InitializePool(transform);
    }
    public GameObject rentObject(ObjectType ObjectType)
    {       
        GameObject[] getPool(ObjectType targetPool)
        {
            switch (targetPool)
            {
                case ObjectType.ProjectileVFX:
                    return m_projectilePool.poolArray;
                case ObjectType.Enemy:
                    return m_EnemyPool.poolArray;
                case ObjectType.ImpactVFX:
                    return m_ImpactVFXPool.poolArray;
                case ObjectType.EnemyExplosionVFX:
                    return m_EnemyExplosionVFXPool.poolArray;
                case ObjectType.ImpactSFX:
                    return m_ImpactSFXPool.poolArray;
                case ObjectType.EnemyDeathSFX:
                    return m_EnemyExplosionSFXPool.poolArray;
                default:
                    Debug.LogWarning("No pool for this enum");
                    return null;
            }
        }
        GameObject[] selectedPoolArray = getPool(ObjectType);

        for (int i = 0; i < selectedPoolArray.Length; i++)
        {
            if(selectedPoolArray[i].gameObject.activeSelf == false)
            {
                selectedPoolArray[i].gameObject.SetActive(true);
                return selectedPoolArray[i].gameObject;
            }
        }

        Debug.LogWarning("ObjectPool empty - Instantiating new asset outside array");

        GameObject rentedObject = Instantiate(selectedPoolArray[0], transform);
        return rentedObject;
    }
}
