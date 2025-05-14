using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]private int maxHealth = 100;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("dead");
        Destroy(gameObject);
    }
}
