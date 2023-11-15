using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS0618 // Type or member is obsolete
public class MenuScript : MonoBehaviour
{
    private Button resumeButton;
    private Button quitButton;
    private void Start() {
        resumeButton = GameObject.Find("ResumeButton").GetComponent<Button>();
        quitButton = GameObject.Find("QuitButton").GetComponent<Button>();
        resumeButton.onClick.AddListener(Resume);
        quitButton.onClick.AddListener(Quit);

    }
    public static void Pause() {
        Time.timeScale = 0;
        GameObject.Find("Canvases").transform.Find("Menu").gameObject.active = true;
        GameObject.Find("Canvases").transform.Find("Canvas").GetComponent<CanvasGroup>().alpha = 0f;
        GameObject.Find("Main Camera").active = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public static void Resume() {
        Time.timeScale = 1;
        GameObject.Find("Canvases").transform.Find("Menu").gameObject.active = false;
        GameObject.Find("Canvases").transform.Find("Canvas").GetComponent<CanvasGroup>().alpha = 1f;
        GameObject.Find("Main Camera").active = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Quit() {
        Application.Quit();
        //EditorApplication.ExitPlaymode();
    }


#pragma warning restore CS0618 // Type or member is obsolete
}
