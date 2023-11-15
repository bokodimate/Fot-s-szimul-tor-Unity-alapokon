using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoBox : MonoBehaviour {
    // Start is called before the first frame update
    GameObject canvas;
    GameObject infoBox;
    GameObject infoBackground;

    private float timeLeft = 0f;
    private bool timer = false;
    private float timerSeconds = 3f;
   
    [TextArea(10, 20)]
    public string message;
    private void Update() {

        if (timer && timeLeft > 0) { 
            timeLeft -= Time.deltaTime;
            if (timeLeft < 0) {
                HideBox();
            }
        }
    }
    void Awake() {
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        if (canvas == null) return;
        infoBox = canvas.transform.Find("InfoBox").gameObject;
        infoBackground = infoBox.transform.Find("InfoBackground").gameObject;
    }

    public void ShowBox(string message, bool timer) {
        if (message == "" || canvas == null) return;
        this.timer = timer;
        infoBox.GetComponentInChildren<TextMeshProUGUI>().text = message;
        infoBackground.GetComponent<Image>().color = new Color(255, 255, 255, 0.5f);
        if (timer) {
            StartTimer();
        }
    }

    public void HideBox() {
        if (canvas == null) return;
        infoBox.GetComponentInChildren<TextMeshProUGUI>().text = "";
        infoBackground.GetComponent<Image>().color = new Color(255, 255, 255, 0f);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            ShowBox(this.message, false);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player") && !timer) {
            HideBox();
        }
    }

    private void StartTimer() {
        timeLeft = timerSeconds;
    }
}
