using UnityEngine;

public class Boss_DeathRay : StateMachineBehaviour
{
    private Collider2D bossCollider;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bossCollider = animator.GetComponent<Collider2D>();
        
        bossCollider.enabled = false;
    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }


    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bossCollider.enabled = true;
    }
    
}
