using System;
using UnityEngine;

public delegate void InputManagerEventHandler();
public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public event InputManagerEventHandler OnJump, OnLeft, OnRight, OnDash;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            OnDash?.Invoke();
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnJump?.Invoke();
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            OnLeft?.Invoke();
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            OnRight?.Invoke();
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}
