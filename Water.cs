using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) { 
            PlayerMovement movement = other.GetComponent<PlayerMovement>();
            movement.isSwimming = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            PlayerMovement movement = other.GetComponent<PlayerMovement>();
            movement.isSwimming = false;
        }
    }
}
