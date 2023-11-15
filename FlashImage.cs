using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FlashImage : MonoBehaviour
{
    Image img = null;
    Coroutine flashRoutine = null;

    private void Awake() {
        img = GetComponent<Image>();
    }

    public void StartFlash(float flashDuration, float maxAlpha, Color newColor) {
        img.color = newColor;

        // maxAlpha 0 és 1 között lehet csak. 
        maxAlpha = Mathf.Clamp(maxAlpha, 0.0f, 1.0f);

        if (flashRoutine != null) {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(Flash(flashDuration, maxAlpha));
    }

    IEnumerator Flash(float flashDuration, float maxAlpha) {
        float flashInDuration = flashDuration / 2;
        for (float t = 0; t <= flashInDuration; t += Time.deltaTime) {
            Color colorThisFrame = img.color;
            colorThisFrame.a = Mathf.Lerp(0, maxAlpha, t / flashInDuration);
            img.color = colorThisFrame;
            yield return null;

        }

        float flashOutDuration = flashDuration / 2;
        for (float t = 0; t <= flashOutDuration; t += Time.deltaTime) {
            Color colorThisFrame = img.color;
            colorThisFrame.a = Mathf.Lerp(maxAlpha, 0, t / flashInDuration);
            img.color = colorThisFrame;
            yield return null;

        }

        // Alpha legyen 0
        img.color = new Color32(0, 0, 0, 0);
    }
}
