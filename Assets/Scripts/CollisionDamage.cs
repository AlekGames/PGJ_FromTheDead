using System;
using UnityEngine;

public class CollisionDamage : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerHealth>().TakeDamage(25);
        }
    }
    
}
