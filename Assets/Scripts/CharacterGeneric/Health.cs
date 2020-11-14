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
        currentHealth -= damage;
        if (currentHealth <= 0)
            onDeath?.Invoke();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
            TakeDamage(5.0f);
    }

    public float CurrentHealth => currentHealth;
    public float MaxHealth => health;
}
