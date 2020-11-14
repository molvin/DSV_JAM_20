﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float health = 100;
    public Action onDeath;
    private float currentHealth;

    private bool dead = false;

    private void OnEnable()
    {
        currentHealth = health;
    }
    public void TakeDamage(float damage)
    {
        if (currentHealth > 0)
            dead = false;
        currentHealth -= damage;
        if (currentHealth <= 0 && !dead)
        {
            onDeath?.Invoke();
            dead = true;
        }
    }

    public float CurrentHealth => currentHealth;
    public float MaxHealth => health;
}
