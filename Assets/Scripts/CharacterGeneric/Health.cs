using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float health = 100;
    public void TakeDamage(float damage)
    {
        Debug.Log(damage);
        health -= damage;
        if (health >= 0)
            gameObject.SetActive(false);
    }
}
