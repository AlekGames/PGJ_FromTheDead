using System.Collections;
using UnityEngine;

public class DashAttack : MonoBehaviour
{
    [SerializeField] private float dashPrecision = 2f;
    [SerializeField] private float dashDuration = 0.45f;
    [SerializeField] private float contactThreshold = 0.2f;
    [SerializeField] private int dashDamage = 25;
    
    [SerializeField] private Animator animator;
    
    private Transform player;

    void Dash()
    {
        Debug.Log("Dash");
        StartCoroutine(DashCoroutine());
    }
    
    private IEnumerator DashCoroutine()
    {
        float elapsedTime = 0;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Vector2 randomPos = (Vector2)player.position + Random.insideUnitCircle * dashPrecision;
        Vector2 startPos = transform.position;

        while (elapsedTime < dashDuration)
        {
            elapsedTime += Time.deltaTime;
            Debug.Log(Vector2.Distance(player.position, transform.position));
            
            if (Vector2.Distance(player.position, transform.position) <= contactThreshold)
            {
                player.GetComponent<PlayerHealth>().TakeDamage(dashDamage);
            }
            transform.position = Vector2.LerpUnclamped(startPos, randomPos, elapsedTime / dashDuration);
            yield return null;
        }
        transform.position = randomPos;
        animator.SetTrigger("EndDash");
    }
}
