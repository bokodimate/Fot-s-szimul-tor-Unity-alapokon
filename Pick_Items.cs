using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pick_Items : MonoBehaviour {
    // This script is on the player

    public InventoryObject inventory;
    public GameObject DropInformation;

    public Camera PlayerCamera;

    private PickableItem _pickableItem;
    private LayerMask layer;

    [SerializeField]
    private Transform slot;



    private float distance = 10;
    private float angle = 360;
    private float height = 100.0f;
    private int scanFrequency = 30;
    public LayerMask layers;
    public LayerMask occlusionLayers;
    public List<GameObject> Objects = new List<GameObject>();




    Collider[] colliders = new Collider[50];
    int count;
    float scanInterval;
    float scanTimer;

    [System.Obsolete]
    private void Scan() {
        count = Physics.OverlapSphereNonAlloc(transform.position, distance, colliders, layers, QueryTriggerInteraction.Collide);
        foreach (GameObject obj in Objects) {   
            try {
                if (obj != null && obj.transform.FindChild("Text") != null) {
                    obj.transform.FindChild("Text").gameObject.SetActive(false);
                }
            } catch (Exception ex) {
                Debug.Log(ex);
            }
        }


        List<GameObject> OldObjects = new List<GameObject>(Objects);
        Objects.Clear();


        for (int i = 0; i < count; i++) {
            GameObject obj = colliders[i].gameObject;
            if (IsInSight(obj)) {
                Objects.Add(obj);
                try {
                    obj.transform.Find("Text").gameObject.SetActive(true);
                    obj.transform.Find("Particle System").gameObject.SetActive(true);
                } catch { }
            }
        }

        foreach (GameObject obj in OldObjects) {
            try {
                if (obj.transform.FindChild("Text").gameObject.active) {
                    obj.transform.FindChild("Particle System").gameObject.SetActive(true);
                } else {
                    obj.transform.FindChild("Particle System").gameObject.SetActive(false);
                }
            } catch (Exception) { }
        }

    }

    public bool IsInSight(GameObject obj) {

        Vector3 origin = new Vector3(transform.position.x, transform.position.y - (height / 2), transform.position.z);

        Vector3 pos = obj.GetComponent<Collider>().transform.position;

        Vector3 dest = new Vector3(pos.x, pos.y, pos.z);

        Vector3 direction = dest - origin;

        if (direction.y < 0 || direction.y > height) {
            return false;
        }

        direction.y = 0;
        float deltaAngle = Vector3.Angle(direction, transform.forward);

        if (deltaAngle > angle) {
            return false;
        }

        return true;
    }

    private void Start() {
        scanInterval = 1.0f / scanFrequency;
        layer = LayerMask.NameToLayer("Pickables");
    }

    [System.Obsolete]
    private void Update() {

        scanTimer -= Time.deltaTime;
        if (scanTimer < 0) {
            scanTimer += scanInterval;

            if (Camera.main != null && Camera.main.name == "Camera") {
                foreach (GameObject obj in Objects) {
                    try {
                        obj.transform.Find("Text").gameObject.SetActive(false);
                        obj.transform.Find("Particle System").gameObject.SetActive(false);
                    } catch { }
                }
            } else {
                if (Camera.main != null && Camera.main.name != "Camera") {
                    Scan();
                }
            }
        }

        if (Input.GetMouseButtonDown(0)) {
            var ray = PlayerCamera.ViewportPointToRay(Vector3.one * 0.5f);

            if (Physics.Raycast(ray, out RaycastHit hit, 3f) && Camera.main != null && Camera.main.name == "Main Camera") {
                var item = hit.transform.GetComponent<PickableItem>();
                var pickable = hit.transform.gameObject.layer == layer;

                if (pickable && item != null) {
                    PickItem(item);
                } else {
                    if (hit.transform.gameObject.transform.FindChild("Item") != null && hit.transform.gameObject.transform.FindChild("Item").gameObject.layer == layer) {
                        PickItem(hit.transform.gameObject.transform.GetComponent<PickableItem>());
                    }
                }
            }
        }

        //if (_pickableItem && Input.GetButtonDown("Fire1")) {
        //    PickItem(_pickableItem);
        //}

        if (Input.GetButtonDown("Fire2")) {
            if (_pickableItem) {
                DropItem(_pickableItem);
            }
        }


    }

    private void OnApplicationQuit() {
        inventory.Container.Items = new InventorySlot[28];
    }

    private void PickItem(PickableItem pickableItem) {
        try {
            GroundItem item = pickableItem.GetComponent<GroundItem>();

            if (item) {
                if (item.item.type == ItemType.Equipment) {
                    inventory.AddItem(new EquipmentItem(item.item as EquipmentObject), 1);
                } else {
                    inventory.AddItem(new Item(item.item), 1);
                }
                Destroy(pickableItem.gameObject);
                Objects.Clear();
                this.gameObject.GetComponent<QuestManager>().UpdateQuests();
            }
        } catch (Exception ex) {
            Debug.Log(ex.Message);
        }
    }

    private void Reset(PickableItem pickableItem) {
        ResetVelocities(pickableItem);
        ResetPosAndRotations(pickableItem);
    }

    private void ResetVelocities(PickableItem pickableItem) {
        pickableItem.Rb.velocity = Vector3.zero;
        pickableItem.Rb.angularVelocity = Vector3.zero;
    }

    private void ResetPosAndRotations(PickableItem pickableItem) {
        pickableItem.transform.localPosition = Vector3.zero;
        pickableItem.transform.localEulerAngles = Vector3.zero;
    }

    IEnumerator DropItemInformation() {
        DropInformation.SetActive(true);
        yield return new WaitForSeconds(3.0f);
        DropInformation.SetActive(false);
    }

    /// <summary>
    /// Method for dropping an item.
    /// </summary>
    /// <param name="item">Item.</param>
    private void DropItem(PickableItem pickableItem) {

        pickableItem.transform.SetParent(null);

        _pickableItem = null;

        pickableItem.transform.SetParent(null);
        pickableItem.transform.position = PlayerCamera.transform.position;
        pickableItem.Rb.isKinematic = false;

        pickableItem.Rb.AddForce(pickableItem.transform.forward * 2, ForceMode.VelocityChange);
    }
}