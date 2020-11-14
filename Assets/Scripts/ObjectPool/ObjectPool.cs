using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Serializable] private class Pool
    {
        [HideInInspector] public GameObject[] poolArray;
        public int poolSize;
        public GameObject prefab;

        public void InitializePool(Transform parent)
        {
            poolArray = new GameObject[poolSize];
            for (int i = 0; i < poolSize; i++)
                poolArray[i] = Instantiate(prefab, parent);
            prefab.SetActive(false);
        }
    }
    [SerializeField] private Pool m_projectileVFXPool;
    [SerializeField] private Pool m_EnemyPool;
    [SerializeField] private Pool m_ImpactVFXPool;
    [SerializeField] private Pool m_EnemyExplosionVFXPool;

    public enum ObjectType
    {
        ProjectileVFX,
        ImpactVFX,
        EnemyExplosionVFX,
        Enemy,
    }


    private void Start()
    {
        m_projectileVFXPool.InitializePool(transform);
        m_EnemyPool.InitializePool(transform);

        for (int i = 0; i < m_EnemyPool.poolSize; i++)
        {
            if (m_EnemyPool.poolArray[i].TryGetComponent<Weapon>(out Weapon weapon))
                weapon.m_objectPool = this;
            else
                Debug.LogWarning("Enemy spawned without weapon");
        }
    
        m_ImpactVFXPool.InitializePool(transform);
    }
    public GameObject rentObject(ObjectType ObjectType)
    {       
        GameObject[] getPool(ObjectType targetPool)
        {
            switch (targetPool)
            {
                case ObjectType.ProjectileVFX:
                    return m_projectileVFXPool.poolArray;
                case ObjectType.Enemy:
                    return m_EnemyPool.poolArray;
                case ObjectType.ImpactVFX:
                    return m_ImpactVFXPool.poolArray;
                case ObjectType.EnemyExplosionVFX:
                    return m_EnemyExplosionVFXPool.poolArray;
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
