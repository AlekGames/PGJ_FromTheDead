using System;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [SerializeField] private GameObject finishMessage;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!finishMessage.activeSelf)
        {
            finishMessage.SetActive(true);
        }
    }
}
