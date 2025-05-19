using UnityEngine;

public class OrbPopEffect : MonoBehaviour
{
    private Animator animator;

    public GameObject energyPrefab;


    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Pop(Transform target)
    {
        if (animator != null)
            animator.SetTrigger("Pop");

        if (energyPrefab != null && target != null)
        {
            GameObject energy = Instantiate(energyPrefab, transform.position, Quaternion.identity);
            StartCoroutine(MoveEnergy(energy.transform, target));
        }
    }

    System.Collections.IEnumerator MoveEnergy(Transform energy, Transform target)
    {
        float duration = 10f;
        float t = 0f;
        Vector3 start = energy.position;

        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = Mathf.SmoothStep(0f, 1f, t / duration); 
            energy.position = Vector3.Lerp(start, target.position, progress);
            yield return null;
        }

        Destroy(energy.gameObject);
    }


}
