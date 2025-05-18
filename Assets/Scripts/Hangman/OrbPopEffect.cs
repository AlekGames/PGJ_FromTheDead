using UnityEngine;

public class OrbPopEffect : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Pop()
    {
        if (animator != null)
        {
            animator.SetTrigger("Pop");
        }
    }
}
