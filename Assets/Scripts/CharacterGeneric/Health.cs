using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float health = 100;
    public Action onDeath;
    private float currentHealth;
    private void OnEnable()
    {
        currentHealth = health;
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health >= 0)
            onDeath?.Invoke();
    }
}
