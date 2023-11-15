using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            PlayerMovement movement = other.GetComponent<PlayerMovement>();
            movement.isAtLadder = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            PlayerMovement movement = other.GetComponent<PlayerMovement>();
            movement.isAtLadder = false;
        }
    }
}
