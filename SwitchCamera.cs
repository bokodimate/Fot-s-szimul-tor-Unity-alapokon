using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class SwitchCamera : MonoBehaviour {
    DepthOfField dof;
    AutoExposure ae;

    public GameObject cam1;
    public GameObject cam2;
    public GameObject canvas;
    public GameObject CameraCanvas;

    private EquipmentItem equippedLens = null;
    private EquipmentItem equippedCamera = null;
    private int lensMin = 18;
    private int lensMax = 55;
    int focalLen = 18;

    private Text apertureText;
    private float apertureMin = 3.5f;
    private float apertureMax = 22f;
    private float[] apertureValues = new float[] { 2.8f, 3.5f, 4f, 4.8f, 5.6f, 6.3f, 8f, 9.5f, 11f, 13f, 16f, 19f, 22f, 27f, 32f };
    private int apertureIndex = 4;

    private Text isoText;
    private int[] isoValues = new int[] { 100, 200, 400, 800, 1600, 3200, 6400, 12800, 25600, 51200 };
    private int isoIndex = 4;

    private Text slotsAvailableText;

    InventoryObject playerInventory;

    private float aeValue;
    Text lightMeterText;

    public LayerMask occlusionLayer;



    // Start is called before the first frame update
    void Start() {
        playerInventory = Resources.Load("PlayerInventory") as InventoryObject;
        cam2.gameObject.SetActive(false);
        cam2.GetComponent<PostProcessVolume>().profile.TryGetSettings(out dof);
        cam2.GetComponent<PostProcessVolume>().profile.TryGetSettings(out ae);
        aeValue = ae.keyValue.value;

        Vector2 imageSize = canvas.GetComponent<RectTransform>().sizeDelta;
        canvas.GetComponentInChildren<RawImage>().GetComponent<RectTransform>().sizeDelta = imageSize;
        CreateApertureText();
        CreateSlotsAvailableText();
        CreateIsoText();

        lightMeterText = CameraCanvas.transform.Find("CamImage").Find("LightMeterText").GetComponent<Text>();

        MenuScript.Pause();
    }

    // Update is called once per frame

    RaycastHit hit;
    [System.Obsolete]
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (Time.timeScale == 1f) {
                MenuScript.Pause();
            } else {
                MenuScript.Resume();
            }
        }



        if (Input.GetKeyDown("c")) {
            if (cam1.active && equippedLens != null && equippedCamera != null) {
            //if (cam1.active) {

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                cam1.SetActive(false);
                cam2.SetActive(true);
                canvas.GetComponent<CanvasGroup>().alpha = 0f;

                CameraCanvas.SetActive(true);
                Vector2 imageSize = CameraCanvas.GetComponent<RectTransform>().sizeDelta;
                CameraCanvas.GetComponentInChildren<RawImage>().GetComponent<RectTransform>().sizeDelta = imageSize;
            } else if(cam2.active) {
                cam1.SetActive(true);
                cam2.SetActive(false);
                canvas.GetComponent<CanvasGroup>().alpha = 1f;
                CameraCanvas.SetActive(false);
                Vector2 imageSize = canvas.GetComponent<RectTransform>().sizeDelta;
                canvas.GetComponentInChildren<RawImage>().GetComponent<RectTransform>().sizeDelta = imageSize;
            }
            if (equippedLens == null || equippedCamera == null) {
                InfoBox box = GameObject.Find("InfoBox").GetComponent<InfoBox>();
                box.ShowBox("Fotózáshoz fogj a kezedbe egy kamerát és objektívet!", true);
            } 


        } else if (Input.GetKeyDown("i") && canvas.active) {
            var inventoryScreen = canvas.transform.Find("Inventory").Find("InventoryScreen").transform.gameObject;
            if (inventoryScreen.active) {
                inventoryScreen.active = false;
                inventoryScreen.transform.Find("tooltip").gameObject.SetActive(false);
            } else {
                inventoryScreen.active = true;
            }
        }

        if (cam2.active) {
            apertureText.color = Color.green;
            isoText.color = Color.green;
            CameraCanvas.transform.Find("CamImage").Find("FocusText").GetComponent<Text>().text = focalLen.ToString();
            if (Input.GetKey(KeyCode.LeftShift)) {
                apertureText.color = Color.yellow;
                if (Input.GetAxis("Mouse ScrollWheel") < 0 && apertureValues[apertureIndex] > apertureMin) {
                    apertureIndex -= 1;
                    if (apertureValues[apertureIndex] < apertureMin) {
                        apertureIndex -= 1;
                    }
                    aeValue += 0.25f;
                    dof.aperture.value = apertureValues[apertureIndex];
                } else if (Input.GetAxis("Mouse ScrollWheel") > 0 && apertureValues[apertureIndex] < apertureMax) {
                    apertureIndex += 1;
                    if (apertureValues[apertureIndex] > apertureMax) {
                        apertureIndex -=1;
                    }
                    aeValue -= 0.25f;
                    dof.aperture.value = apertureValues[apertureIndex];
                }
                apertureText.text = "Aperture\n" + "f " + apertureValues[apertureIndex];
            } else if (Input.GetKey(KeyCode.LeftControl)) {
                isoText.color = Color.yellow;
                if (Input.GetAxis("Mouse ScrollWheel") < 0 && isoIndex > 0) {
                    isoIndex -= 1;
                    aeValue -= 0.25f;
                } else if (Input.GetAxis("Mouse ScrollWheel") > 0 && isoIndex < isoValues.Length-1) {
                    isoIndex += 1;
                    aeValue += 0.25f;
                }
                
                isoText.text = "ISO\n" + isoValues[isoIndex].ToString();
            } else {
                if (Input.GetAxis("Mouse ScrollWheel") < 0 && focalLen > lensMin) {
                    focalLen -= 2;
                    if (focalLen < lensMin) {
                        focalLen = lensMin;
                    }
                    cam2.GetComponent<Camera>().focalLength = focalLen;
                    dof.focalLength.value = focalLen;
                }
                if (Input.GetAxis("Mouse ScrollWheel") > 0 && focalLen < lensMax) {
                    focalLen += 2;
                    if (focalLen > lensMax) {
                        focalLen = lensMax;
                    }
                    cam2.GetComponent<Camera>().focalLength = focalLen;
                    dof.focalLength.value = focalLen;
                }
                if (Input.GetMouseButtonDown(2)) {
                    var main = Camera.main.transform;

                    if (Physics.SphereCast(main.position, 0.3f, main.forward, out hit, 100, ~occlusionLayer)) {
                        Transform objectHit = hit.transform;
                        var distance = Vector3.Distance(main.position, objectHit.position);

                        dof.focusDistance.value = distance;
                        transform.GetComponentInChildren<AnimalDetection>().distance = distance;
                    }
                }
                if (Input.GetKeyDown(KeyCode.Mouse0)) {
                    ScreenshotHandler.TakeScreenshot_Static(Screen.width, Screen.height);
                }
            }
            //if (aeValue < 0) {
            //    aeValue = 0;
            //}
            ae.keyValue.value = aeValue;
            transform.GetComponentInChildren<AnimalDetection>().angle = cam2.GetComponent<Camera>().fieldOfView;

            int slotsAvailable = playerInventory.Container.Items.Length;

            foreach (var item in playerInventory.Container.Items) {
                if (item.ID != -1) {
                    slotsAvailable--;
                }
            }

            slotsAvailableText.text = "Tárhely\n" + slotsAvailable.ToString();
            UpdateLightMeter();
        }
    }

    private void OnDrawGizmos() { 
        Gizmos.color = Color.green;
        Vector3 sphereCastMidpoint = Camera.main.transform.position + (transform.forward * hit.distance);
        Gizmos.DrawWireSphere(sphereCastMidpoint, 0.3f);
        Gizmos.DrawSphere(hit.point, 0.1f);
        Debug.DrawLine(Camera.main.transform.position, sphereCastMidpoint, Color.green);

    }

    private void UpdateLightMeter() {
        string text = "|";
        Vector2 imageWH = canvas.GetComponent<RectTransform>().sizeDelta;
        if (aeValue == 1) {
            lightMeterText.GetComponent<RectTransform>().localPosition = new Vector2(0, -(imageWH.y/2) + 50 );
            lightMeterText.alignment = TextAnchor.MiddleCenter;
        } else if (aeValue < 1) {
            lightMeterText.GetComponent<RectTransform>().localPosition = new Vector2(-77, -(imageWH.y / 2) + 50);
            lightMeterText.alignment = TextAnchor.MiddleRight;
            for (var i = 1f; i > System.Math.Abs(ae.keyValue.value); i -= 0.1f) {
                text = text.Insert(0, "▮");
            }
        } else if (aeValue > 1) {
            lightMeterText.GetComponent<RectTransform>().localPosition = new Vector2(78, -(imageWH.y / 2) + 50);
            lightMeterText.alignment = TextAnchor.MiddleLeft;
            for (var i = 1f; i < System.Math.Abs(ae.keyValue.value); i += 0.1f) {
                text = text.Insert(1, "▮");
            }
        }

        if (aeValue <= 0) {
            text = "◄▮▮▮▮▮▮▮▮▮▮|";
        } else if (aeValue >= 2) {
            text = "|▮▮▮▮▮▮▮▮▮▮►";
        }

        lightMeterText.text = text;
    }

    private void CreateApertureText() {
        GameObject apertureTextObj = new GameObject();
        apertureTextObj.transform.parent = CameraCanvas.transform.Find("CamImage").transform;
        apertureTextObj.name = "ApertureText";
        apertureText = apertureTextObj.AddComponent<Text>();
        apertureTextObj.GetComponent<RectTransform>().localScale = Vector3.one;
        
        apertureText.color = Color.green;
        apertureText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        apertureText.text = "Aperture\n" + "f " + apertureValues[apertureIndex];
        apertureText.alignment = TextAnchor.MiddleCenter;
        apertureText.fontSize = 30;
        
        Vector2 textWH = new Vector2(150,150);
        apertureTextObj.GetComponent<RectTransform>().sizeDelta = textWH;
        Vector2 imageWH = canvas.GetComponent<RectTransform>().sizeDelta;

        apertureTextObj.GetComponent<RectTransform>().localPosition = new Vector2(-(imageWH.x/4), -((imageWH.y / 2)-50));
    }

    private void CreateSlotsAvailableText() {
        GameObject slotsAvailableObj = new GameObject();
        slotsAvailableObj.transform.parent = CameraCanvas.transform.Find("CamImage").transform;
        slotsAvailableObj.name = "SlotsAvailable";
        slotsAvailableText = slotsAvailableObj.AddComponent<Text>();
        slotsAvailableObj.GetComponent<RectTransform>().localScale = Vector3.one;

        slotsAvailableText.color = Color.green;
        slotsAvailableText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        slotsAvailableText.fontSize = 30;
        slotsAvailableText.text = playerInventory.Container.Items.Length.ToString();
        slotsAvailableText.alignment = TextAnchor.MiddleCenter;


        Vector2 textWH = new Vector2(150, 150);
        slotsAvailableObj.GetComponent<RectTransform>().sizeDelta = textWH;
        Vector2 imageWH = canvas.GetComponent<RectTransform>().sizeDelta;

        slotsAvailableObj.GetComponent<RectTransform>().localPosition = new Vector2(imageWH.x/4, -((imageWH.y / 2) -50));
    }

    private void CreateIsoText() {
        GameObject isoObj = new GameObject();
        isoObj.transform.parent = CameraCanvas.transform.Find("CamImage").transform;
        isoObj.name = "isoText";
        isoText = isoObj.AddComponent<Text>();
        isoObj.GetComponent<RectTransform>().localScale = Vector3.one;

        isoText.color = Color.green;
        isoText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
        isoText.fontSize = 30;
        isoText.text = "ISO\n" + isoValues[isoIndex].ToString();
        isoText.alignment = TextAnchor.MiddleCenter;


        Vector2 textWH = new Vector2(150, 150);
        isoObj.GetComponent<RectTransform>().sizeDelta = textWH;
        Vector2 imageWH = canvas.GetComponent<RectTransform>().sizeDelta;

        isoObj.GetComponent<RectTransform>().localPosition = new Vector2(-(imageWH.x / 8)*3, -((imageWH.y / 2) -50));
    }

    public void ChangeCamera(EquipmentItem camera) {
        if (camera == null) {
            equippedCamera = null;
            return;
        }
        if (equippedCamera != null) {
            equippedCamera.Equipped = false;
        }
        camera.Equipped = true;
        equippedCamera = camera;
    }
    public void ChangeLens(EquipmentItem lens) {
        if (lens == null) {
            equippedLens = null;
            return;
        }
        if (equippedLens != null) { 
            equippedLens.Equipped = false;
        }
        lens.Equipped = true;
        equippedLens = lens;
        int lensMin = -1, lensMax = -1;
        float aperatureMin = -1f, aperatureMax = -1f;
        for (var i = 0; i < lens.Properties.Length; i++) {
            switch (lens.Properties[i].attribute) {
                case Attributes.FocalLengthMin:
                    lensMin = (int)lens.Properties[i].value;
                    break;
                case Attributes.FocalLengthMax:
                    lensMax = (int)lens.Properties[i].value;
                    break;
                case Attributes.ApertureMin:
                    aperatureMin = lens.Properties[i].value;
                    break;
                case Attributes.ApertureMax:
                    aperatureMax = lens.Properties[i].value;
                    break;
            }
        }

        this.lensMin = lensMin;
        this.lensMax = lensMax;

        this.apertureMin = aperatureMin;
        this.apertureMax = aperatureMax;

        cam2.GetComponent<PostProcessVolume>().profile.TryGetSettings(out dof);
        cam2.GetComponent<Camera>().focalLength = lensMin;
        dof.focalLength.value = lensMin;
        dof.aperture.value = aperatureMin;

        focalLen = lensMin;
    }

    public float getIso() {
        return (float)isoIndex;
    }

    public int isoValue {
        get {
            return isoValues[isoIndex];
        }
    }

}
