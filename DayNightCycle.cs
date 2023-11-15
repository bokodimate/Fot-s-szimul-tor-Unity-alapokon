using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using TMPro;

public class DayNightCycle : MonoBehaviour {

    public GameObject stars;

    public float time;
    public TimeSpan currenttime;
    public Transform SunTransform;
    public Light Sun;
    public TextMeshProUGUI timetext;

    public float intensity;
    public Color fogday = Color.gray;
    public Color fognight = Color.black;

    public int speed;


    private void Start() {
        Vector2 canvasSize = timetext.transform.parent.GetComponent<RectTransform>().sizeDelta;
        Vector2 textSize = timetext.GetComponent<RectTransform>().sizeDelta;
        timetext.GetComponent<RectTransform>().anchoredPosition = new Vector2((canvasSize.x/-2) + (textSize.x/2) + 10, (canvasSize.y/-2) + (textSize.y/2) + 10);
    }

    void Update() {
        ChangeTime();
    }

    public void ChangeTime() {
        //Vector2 canvasSize = timetext.transform.parent.GetComponent<RectTransform>().sizeDelta;
        //Vector2 textSize = timetext.GetComponent<RectTransform>().sizeDelta;
        //timetext.GetComponent<RectTransform>().anchoredPosition = new Vector2((canvasSize.x / -2) + (textSize.x / 2) + 10, (canvasSize.y / -2) + (textSize.y / 2) + 10);


        time += Time.deltaTime * speed;
        if (time > 86400) {
            time = 0;
        }

        currenttime = TimeSpan.FromSeconds(time);
        string[] temptime = currenttime.ToString().Split(":");
        timetext.text = temptime[0] + ":" + temptime[1];

        SunTransform.rotation = Quaternion.Euler(new Vector3((time - 21600) / 86400 * 360, -90, 0)); 
        if (SunTransform.rotation.x > 185) {

        }
        if (time > 43200) { 
            intensity = 1 + (43200 - time) / 43200;
        } else { 
            intensity = 1 - (43200 - time) / 43200;
        }

        if (time < 21600 || time > 64800) {
            intensity = 0;
            stars.SetActive(true);
        } else {
            stars.SetActive(false);
        }

        RenderSettings.fogColor = Color.Lerp(fognight, fogday, intensity * intensity);

        Sun.intensity = intensity;

    }

}