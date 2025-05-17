using System;
using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]private Animator playerAnimator;
    [SerializeField]private int health = 100;
    [SerializeField]private float damageDelay = 1f;
    private int currentHealth;
    private bool canTakeDamage = true;

    private void Start()
    {
        currentHealth = health;
    }

    public void TakeDamage(int damage)
    {
        if (!canTakeDamage)
        {
            return;
        }
        currentHealth -= damage;
        playerAnimator.SetTrigger("Hurt");
        ScreenShake.Instance.ShakeCamera(10f, 0.2f);
        canTakeDamage = false;
        StartCoroutine(DamageDelay());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator DamageDelay()
    {
        yield return new WaitForSeconds(damageDelay);
        canTakeDamage = true;
    }

    private void Die()
    {
        playerAnimator.SetTrigger("Death");
        GetComponent<PlayerController>().enabled = false;
        enabled = false;
    }

    public void GameOver()
    {
        SceneManager.LoadScene("StartScreen");
    }
}
