using System;
using UnityEngine;


public class Finish : MonoBehaviour
{
    public Action OnGameFinished;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            OnGameFinished?.Invoke();
    }
}

