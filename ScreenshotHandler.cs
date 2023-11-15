using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class ScreenshotHandler : MonoBehaviour
{
    public InventoryObject inventory;
    private static ScreenshotHandler instance;

    private static Camera myCamera;
    private bool takeScreenshotOnNextFrame;

    public AudioSource shutterSoundSource;
    public AudioClip shutterSoundClip;
    [SerializeField] FlashImage flash = null;

    float noiseMultiplier;

    private GameObject player;

    private void Awake() {
        instance = this;
        myCamera = gameObject.GetComponent<Camera>();
        player = GameObject.Find("Player");
    }
    
    private void OnPostRender() {
        if (takeScreenshotOnNextFrame) {
            takeScreenshotOnNextFrame = false;
            RenderTexture rt = myCamera.targetTexture;

            Texture2D renderResult = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false, true);
            Rect rectangle = new Rect(0, 0, rt.width, rt.height);
            renderResult.ReadPixels(rectangle, 0, 0);

            Color[] pixels = renderResult.GetPixels();

            AutoExposure ae;

            Texture2D heightMap = NoiseGenerator.GenerateTexture(rt.width, rt.height, 400);
            Color[] heightMapPixels = heightMap.GetPixels();

            myCamera.GetComponent<PostProcessVolume>().profile.TryGetSettings(out ae);
            noiseMultiplier = player.GetComponent<SwitchCamera>().getIso() / 50;
            for (int p = 0; p < pixels.Length; p++) {
                if (heightMapPixels[p].g < noiseMultiplier) {
                    pixels[p] = pixels[p].gamma * (heightMapPixels[p].gamma/2);
                } else {
                    pixels[p] = pixels[p].gamma * ae.keyValue;
                }
            }
            renderResult.SetPixels(pixels);

            renderResult.Apply();
            

            byte[] byteArray = renderResult.EncodeToJPG();
            string fileName = System.DateTime.Now.ToString().Replace(".", "-").Replace(" ", "").Replace(":", "-") + ".jpg";

            if (!System.IO.Directory.Exists(Application.dataPath + "/Photos/")) {
                System.IO.Directory.CreateDirectory(Application.dataPath + "/Photos/");
            }
            System.IO.File.WriteAllBytes(Application.dataPath + "/Photos/" + fileName, byteArray);

            RenderTexture.ReleaseTemporary(rt);
            myCamera.targetTexture = null;

            PhotoObject photoObject = Instantiate(Resources.Load("Photo") as PhotoObject);
            photoObject.name = fileName;
            photoObject.Path = Application.dataPath + "/Photos/" + fileName;
            PhotoItem photoItem = new PhotoItem(photoObject);
            photoItem.Path = photoObject.Path;
            foreach (var x in transform.GetComponent<AnimalDetection>().Objects) {
                photoItem.Animals.Add(x.name);
            }
            DepthOfField dof;
            myCamera.GetComponent<PostProcessVolume>().profile.TryGetSettings(out dof);

            for (var i = 0; i < photoItem.Properties.Length; i++) {
                switch(photoItem.Properties[i].attribute) {
                    case Attributes.Aperture:
                        photoItem.Properties[i].setValue(dof.aperture.value);
                        break;
                    case Attributes.FocalLength:
                        photoItem.Properties[i].setValue(dof.focalLength.value);
                        break;
                    case Attributes.Iso:
                        photoItem.Properties[i].setValue(player.GetComponent<SwitchCamera>().isoValue);
                        break;
                }
            }
            if (player.GetComponent<PlayerMovement>().isOnBlind) {
                photoItem.IsOnBlind = true;
            }

            inventory.AddItem(photoItem, 1);
            player.GetComponent<QuestManager>().UpdateQuests();
        }
    }
    private void TakeScreenshot(int width, int height) {
        myCamera.targetTexture = RenderTexture.GetTemporary(width, height, 16);

        shutterSoundSource.PlayOneShot(shutterSoundClip);
        flash.StartFlash(0.25f, 0.5f, Color.black);
        takeScreenshotOnNextFrame = true;
    }

    public static void TakeScreenshot_Static(int width, int height) {
        if (!Cursor.visible && Cursor.lockState == CursorLockMode.Locked) { 
            instance.TakeScreenshot(width, height);
        }
    }


        
}
