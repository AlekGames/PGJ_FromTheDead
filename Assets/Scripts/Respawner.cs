using System;
using Player;
using UnityEngine;

public class Respawner : MonoBehaviour
{
    private Vector2 _respawnPos;
    
    private void Start()
    {
        _respawnPos = transform.position;
        Trap.OnTrapTriggered += Trap_OnTrapTriggered;
    }

    private void Trap_OnTrapTriggered()
    {
        SpawnBody();
        Respawn();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("test");
    }

    private void SpawnBody()
    {
        // Clone the player
        GameObject deadBody = Instantiate(gameObject, transform.position, transform.rotation);

        // Remove unwanted components from the clone
        Destroy(deadBody.GetComponent<PlayerController>());
        Destroy(deadBody.GetComponent<Respawner>());
        Destroy(deadBody.GetComponent<Rigidbody2D>());
        Destroy(deadBody.transform.GetChild(0).gameObject);
    }

    private void SetRespawnPos(Vector2 newPosition)
    {
        _respawnPos = newPosition;
    }

    private void Respawn()
    {
        GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        transform.position = _respawnPos;
    }

    private void OnDestroy()
    {
        Trap.OnTrapTriggered -= Trap_OnTrapTriggered;
    }
}