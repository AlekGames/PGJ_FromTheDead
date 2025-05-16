using UnityEngine;

public class Boss_Idle : StateMachineBehaviour
{
    public float targetPosUpdateRate = 1f;
    public float attackRate = 5f;
    public float moveSpeed = 2.5f;
    public float startDist = 4f;
    public float randomStrength = 2f;
    
    private Transform player;
    private Vector3 targetPos;
    private float elapsedTime;
    private float elapsedAttackTime;
    private Collider2D bossCollider;
    
    private Transform transform;
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        targetPos = player.position + player.forward * startDist;
        transform = animator.GetComponent<Transform>();
        bossCollider = animator.GetComponent<Collider2D>();
        
        bossCollider.enabled = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        elapsedTime += Time.deltaTime;
        elapsedAttackTime += Time.deltaTime;

        if (elapsedAttackTime >= attackRate)
        {
            elapsedAttackTime = 0;
            
            int randomAttackIndex = Random.Range(0, 3);

            switch (randomAttackIndex)
            {
                case 0:
                    animator.SetTrigger("AttackRange");
                    break;
                case 1:
                    animator.SetTrigger("Dash");
                    break;
                case 2:
                    animator.SetTrigger("AOE");
                    break;
            }
        }

        if (elapsedTime >= targetPosUpdateRate)
        {
            elapsedTime = 0;
            
            Vector2 randomPos = (Vector2)player.position + Random.insideUnitCircle * randomStrength;
            targetPos = randomPos;
        }
        transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bossCollider.enabled = false;
        animator.ResetTrigger("AttackRange");
        animator.ResetTrigger("Dash");
        animator.ResetTrigger("AOE");
    }
}
