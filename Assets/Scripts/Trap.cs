using System;
using UnityEngine;

public delegate void TrapEventHandler();
public class Trap : MonoBehaviour
{
    public static event TrapEventHandler OnTrapTriggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnTrapTriggered?.Invoke();
    }
}
