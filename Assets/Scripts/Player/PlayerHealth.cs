using System;
using Player;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]private Animator playerAnimator;
    [SerializeField]private int health = 100;
    private int currentHealth;

    private void Start()
    {
        currentHealth = health;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(10);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        playerAnimator.SetTrigger("Hurt");
        ScreenShake.Instance.ShakeCamera(10f, 0.2f);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        playerAnimator.SetTrigger("Death");
        GetComponent<PlayerController>().enabled = false;
        enabled = false;
    }
}
