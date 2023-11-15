using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour {

    public RawImage CompassImage;
    public Transform Player;

    void Start() {
        Vector2 size = transform.parent.GetComponent<RectTransform>().sizeDelta;
        Vector2 compassSize = new Vector2(size.x / 2, size.y / 15);
        this.GetComponent<RectTransform>().sizeDelta = compassSize;
        this.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, (compassSize.y / -2) - 5);
        this.GetComponent<RectTransform>().localScale = Vector3.one;

        this.transform.parent.Find("Compass Background").GetComponent<RectTransform>().sizeDelta = compassSize;
        this.transform.parent.Find("Compass Background").GetComponent<RectTransform>().anchoredPosition = new Vector2(0, (compassSize.y / -2) - 5);
        this.transform.parent.Find("Compass Background").GetComponent<RectTransform>().localScale = Vector3.one;
    }

    void Update() {
        CompassImage.uvRect = new Rect(Player.localEulerAngles.y / 360, 0, 1, 1);
    }
}
