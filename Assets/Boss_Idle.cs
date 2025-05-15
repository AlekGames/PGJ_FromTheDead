using UnityEngine;

public class Boss_Idle : StateMachineBehaviour
{
    public float targetPosUpdateRate = 1f;
    public float moveSpeed = 2.5f;
    public float startDist = 4f;
    public float randomStrength = 2f;
    private Transform player;
    private Vector3 targetPos;
    private float elapsedTime;
    private Transform transform;
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        targetPos = player.position + player.forward * startDist;
        transform = animator.GetComponent<Transform>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        elapsedTime += Time.deltaTime;

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
        
    }
}
