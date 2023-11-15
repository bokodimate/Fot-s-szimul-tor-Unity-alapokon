using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnBlind : MonoBehaviour
{
    // Start is called before the first frame update

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            other.GetComponent<PlayerMovement>().isOnBlind = false;
        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            other.GetComponent<PlayerMovement>().isOnBlind = false;
        }
    }


}
