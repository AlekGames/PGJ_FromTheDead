using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathRayAttack : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform attackPosition;
    [SerializeField] private float toAttackPositionDuration;
    [SerializeField] private float attackDuration;
    [SerializeField] private int aoeDamage;
    private Transform player;
    public void StartDeathRay()
    {
        StartCoroutine(DeathRayCoroutine());
    }

    private IEnumerator DeathRayCoroutine()
    {
        Vector2 initialPos = transform.position;
        Vector2 targetPos = attackPosition.position;
        float elapsedTime = 0;

        while (elapsedTime < toAttackPositionDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector2.LerpUnclamped(initialPos, targetPos, elapsedTime / toAttackPositionDuration);
            yield return null;
        }
        transform.position = targetPos;
        elapsedTime = 0;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        while (elapsedTime < attackDuration)
        {
            elapsedTime += Time.deltaTime;
            
            if (player.position.y < 0)
            {
                player.GetComponent<PlayerHealth>().TakeDamage(aoeDamage);
            }
            yield return null;
        }
        elapsedTime = 0;
        
        while (elapsedTime < toAttackPositionDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector2.LerpUnclamped(targetPos, initialPos, elapsedTime / toAttackPositionDuration);
            yield return null;
        }
        transform.position = initialPos;
        animator.SetTrigger("EndAOE");
    }
}
