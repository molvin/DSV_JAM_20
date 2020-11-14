using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float health = 100;
    private float currentHealth;
    private void OnEnable()
    {
        currentHealth = health;
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health >= 0)
            gameObject.SetActive(false);
    }
}
