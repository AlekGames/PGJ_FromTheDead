using System;

using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [SerializeField]private int maxHealth = 100;
    [SerializeField]private float attackRate = 5f;
    [SerializeField]private float randomAttackOffset = 1f;
    [SerializeField] private Animator animatorController; 
    private int currentHealth;
    private float elapsedAttackTime;

    private void Start()
    {
        currentHealth = maxHealth;
        randomAttackOffset = Random.Range(0, randomAttackOffset);
    }

    private void Update()
    {
        elapsedAttackTime += Time.deltaTime;

        if (elapsedAttackTime >= attackRate + randomAttackOffset)
        {
            elapsedAttackTime = 0;
            randomAttackOffset = Random.Range(0, randomAttackOffset);
            int randomAttackIndex = Random.Range(0, 3);

            switch (randomAttackIndex)
            {
                case 0:
                    animatorController.SetTrigger("AttackRange");
                    break;
                case 1:
                    animatorController.SetTrigger("Dash");
                    break;
                case 2:
                    animatorController.SetTrigger("AOE");
                    break;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animatorController.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
