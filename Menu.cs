using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) {
            Debug.Log("esc");
            if (Time.timeScale == 1f) {
                Time.timeScale = 0;
                Debug.Log("0");
            } else {
                Time.timeScale = 1;
                Debug.Log("1");
            }
        }
    }
}
