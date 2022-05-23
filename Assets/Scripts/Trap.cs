using System;
using UnityEngine;


    public class Trap : MonoBehaviour
    {
        public Action OnPlayerDeath;
       private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                OnPlayerDeath?.Invoke();
        }
    }

