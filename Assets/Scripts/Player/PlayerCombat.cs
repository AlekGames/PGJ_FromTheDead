using UnityEngine;
using UnityEngine.Serialization;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private int attackDamage = 20;
    [SerializeField] private float attackRate = 2;
    [SerializeField] private LayerMask enemyLayers;

    private float nextAttackTime = 0;
    
    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                playerAnimator.SetTrigger("Attack");
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    public void Attack()
    {
        
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (var enemy in hitEnemies)
        {
            SoundManager.PlaySound(SoundType.PLAYER_ATTACK);
            ScreenShake.Instance.ShakeCamera(10f, 0.2f);
            enemy.GetComponent<Enemy>().TakeDamage(attackDamage);
        }
    }
}