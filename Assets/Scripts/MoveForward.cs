using UnityEngine;

public class MoveForward : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float lifetime = 5f;
    
    void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }
        transform.position += transform.right * (moveSpeed * Time.deltaTime);
    }
}
