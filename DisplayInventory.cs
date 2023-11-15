using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEditor;
using System;

public class DisplayInventory : MonoBehaviour {

    public MouseItem mouseItem = new MouseItem();

    public GameObject inventoryPrefab;
    public InventoryObject inventory;
    
    private float X_START;
    private float Y_START;
    private float X_SPACE_BETWEEN_ITEMS;
    private float Y_SPACE_BETWEEN_ITEMS;
    private int NUMBER_OF_COLUMNS = 4;
    private float SLOT_SIZE;

    Dictionary<GameObject, InventorySlot> itemsDisplayed = new Dictionary<GameObject, InventorySlot>();

    GameObject tooltipHolder;

    void Start() {
        CalculateSizes();
        CreateSlots();
        CreateTooltip();
    }

    // Update is called once per frame
    void Update() {
        UpdateSlots();
    }

    private void CalculateSizes() {
        Vector2 parentSize = this.GetComponentInParent<RectTransform>().sizeDelta;
        Vector2 InventoryCanvasSize = new Vector2(parentSize.y / 3, parentSize.x / 3);
        this.GetComponent<RectTransform>().sizeDelta = new Vector2(InventoryCanvasSize.x, InventoryCanvasSize.y);
        this.GetComponent<RectTransform>().localPosition = new Vector2(parentSize.x / 3, 0);

        SLOT_SIZE = InventoryCanvasSize.x / 5;
        X_SPACE_BETWEEN_ITEMS = (InventoryCanvasSize.x - (NUMBER_OF_COLUMNS * SLOT_SIZE)) / 5;
        Y_SPACE_BETWEEN_ITEMS = X_SPACE_BETWEEN_ITEMS;
        X_START = -(InventoryCanvasSize.x / 2) + (SLOT_SIZE / 2) + X_SPACE_BETWEEN_ITEMS;
        Y_START = (InventoryCanvasSize.y / 2) - (SLOT_SIZE / 2) - Y_SPACE_BETWEEN_ITEMS;
    }

    public void UpdateSlots() {
        foreach (KeyValuePair<GameObject, InventorySlot> _slot in itemsDisplayed) {
            if (_slot.Value.ID >= 0) {
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = inventory.database.GetItem[_slot.Value.item.Id].uiDisplay;
                _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = _slot.Value.amount == 1 ? "" : _slot.Value.amount.ToString("n0");
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
                if (_slot.Value.item.Type == ItemType.Equipment && (_slot.Value.item as EquipmentItem).Equipped) {
                    _slot.Key.transform.GetComponent<Image>().color = Color.yellow;
                } else {
                    _slot.Key.transform.GetComponent<Image>().color = new Color32(73, 73, 73, 100);
                }
            } else {
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
                _slot.Key.transform.GetComponent<Image>().color = new Color32(73, 73, 73, 100);
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
                _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
        }
    }

    public void CreateSlots() {
        inventoryPrefab.GetComponent<RectTransform>().sizeDelta = new Vector2(SLOT_SIZE, SLOT_SIZE);
        itemsDisplayed = new Dictionary<GameObject, InventorySlot>();
        for (int i = 0; i < inventory.Container.Items.Length; i++) {
            var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
            obj.transform.Find("Image").GetComponent<RectTransform>().sizeDelta = new Vector2(SLOT_SIZE * 0.9f, SLOT_SIZE * 0.9f);
            obj.transform.Find("Text").GetComponent<RectTransform>().localPosition = new Vector2(SLOT_SIZE / 2, -SLOT_SIZE / 2);
            obj.transform.Find("Text").GetComponent<TextMeshProUGUI>().fontSize = SLOT_SIZE / 4;
            obj.GetComponent<RectTransform>().localPosition = GetPosition(i);

            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });
            AddEvent(obj, EventTriggerType.PointerClick, delegate { OnClick(obj); });

            itemsDisplayed.Add(obj, inventory.Container.Items[i]);
        }
    }

    private void CreateTooltip() {
        tooltipHolder = new GameObject();
        tooltipHolder.transform.SetParent(transform);
        tooltipHolder.transform.localPosition = new Vector3(0, -5000, 0);
        tooltipHolder.transform.localScale = Vector3.one;

        tooltipHolder.name = "tooltip";
        tooltipHolder.SetActive(false);

        var tooltip = new GameObject();
        tooltip.name = "TooltipBackground";
        tooltip.AddComponent<Image>();
        tooltip.transform.SetParent(tooltipHolder.transform);
        tooltip.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 200);
        tooltip.transform.GetComponent<RectTransform>().localPosition = Vector2.zero;
        tooltip.transform.GetComponent<RectTransform>().localScale = Vector3.one;
        tooltip.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);


        var tooltipText = new GameObject();
        tooltipText.name = "TooltipText";
        Text text = tooltipText.AddComponent<Text>();
        text.text = "";
        tooltipText.transform.SetParent(tooltipHolder.transform);
        tooltipText.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 200);
        tooltipText.transform.GetComponent<RectTransform>().localPosition = Vector3.zero;
        tooltipText.transform.GetComponent<RectTransform>().localScale = Vector3.one;

        text.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;

        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        text.fontSize = 24;
    }

    private void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action) {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }

    public void OnEnter(GameObject obj) {
        mouseItem.hoverObj = obj;
        //if (itemsDisplayed.ContainsKey(obj)) {
            //mouseItem.hoverItem = itemsDisplayed[obj];
            Item item = itemsDisplayed[obj].item;

            if (itemsDisplayed[obj].ID != -1) {
                Vector2 newPos = new Vector2(tooltipHolder.GetComponentInParent<RectTransform>().offsetMin.x, tooltipHolder.GetComponentInParent<RectTransform>().offsetMax.y);
                newPos.x = obj.GetComponent<RectTransform>().localPosition.x - 200;
                newPos.y = obj.GetComponent<RectTransform>().localPosition.y - 75;
                tooltipHolder.SetActive(true);
                tooltipHolder.transform.localPosition = newPos;
                tooltipHolder.GetComponentInChildren<Text>().text = item.Name.ToString() + "\n\n";

                if (item.GetType() == typeof(PhotoItem)) {
                    foreach (var x in (item as PhotoItem).Animals) {
                        tooltipHolder.GetComponentInChildren<Text>().text += x + "\n";
                    }
                }

                for (var i = 0; i < itemsDisplayed[obj].item.Properties.Length; i++) {
                    tooltipHolder.GetComponentInChildren<Text>().text += itemsDisplayed[obj].item.Properties[i].attribute.ToString() + ": " + itemsDisplayed[obj].item.Properties[i].value + "\n";
                }
            }
        //}
    }

    public void OnExit() {
        mouseItem.hoverObj = null;
       // mouseItem.hoverItem = null;
        tooltipHolder.SetActive(false);
    }
    public void OnDragStart(GameObject obj) {
        if (itemsDisplayed[obj].ID < 0) {
            return;
        }
        var mouseObject = new GameObject();
        var rt = mouseObject.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(50, 50);
        mouseObject.transform.SetParent(transform.parent);
        var img = mouseObject.AddComponent<Image>();
        img.sprite = inventory.database.GetItem[itemsDisplayed[obj].ID].uiDisplay;
        img.raycastTarget = false;

        mouseItem.obj = mouseObject;
        mouseItem.item = itemsDisplayed[obj];
    }
    public void OnDragEnd(GameObject obj) {
        if (mouseItem.hoverObj) {
            inventory.MoveItem(itemsDisplayed[obj], itemsDisplayed[mouseItem.hoverObj]);
        } else {
            ShowInfo(obj);
            HideImage(obj);

            Item item = itemsDisplayed[obj].item;
            if (item.Type == ItemType.Equipment && (item as EquipmentItem).Equipped) {
                SwitchCamera camera = GameObject.Find("Player").GetComponent<SwitchCamera>();
                if (item.Name.ToLower().Contains("Kamera".ToLower())) {
                    camera.ChangeCamera(null);
                } else if (item.Name.ToLower().Contains("Objekt�v".ToLower())) {
                    camera.ChangeLens(null);
                }
            }

            if (item.Type != ItemType.Photo) {
                for (var i = 0; i < itemsDisplayed[obj].amount; i++) {
                    var groundItem = Instantiate(Resources.Load(item.ObjectName)) as GameObject;
                    Vector3 playerPos = GameObject.Find("Player").transform.position;
                    float y = Terrain.activeTerrain.SampleHeight(new Vector3(playerPos.x + 3, 0, playerPos.z + 3));
                    groundItem.transform.position = new Vector3(playerPos.x + 3, y, playerPos.z + 3);
                    groundItem.transform.SetParent(GameObject.Find("Items").transform);
                }
            }

            inventory.RemoveItem(item);
        }

        Destroy(mouseItem.obj);
        mouseItem.item = null;
    }
    public void OnDrag(GameObject obj) {
        if (mouseItem.obj != null) {
            mouseItem.obj.GetComponent<RectTransform>().position = Input.mousePosition;
        }
    }

    public void HideImage(GameObject obj) {
        if (itemsDisplayed[obj].item != null && itemsDisplayed[obj].item.GetType() == typeof(PhotoItem)) { 
            Transform imageObject = transform.parent.Find("Image");
            if (imageObject.GetComponent<RawImage>().texture.name == itemsDisplayed[obj].item.Name.Split('.')[0]) {
                imageObject.gameObject.SetActive(false);
            } 
        }

    }

    public void OnClick(GameObject obj) {
        if (itemsDisplayed[obj].item != null) {
            if (itemsDisplayed[obj].item.GetType() == typeof(PhotoItem)) {
                Transform imageObject = transform.parent.Find("Image");
                RawImage image = imageObject.GetComponent<RawImage>();
                //AssetDatabase.Refresh();
                
                //Texture2D texture = (Texture2D)UnityEngine.Resources.Load(Application.dataPath + "/Resources/" + itemsDisplayed[obj].item.Name, typeof(Texture2D));
                Texture2D texture = null;
                byte[] fileData;

                if (System.IO.File.Exists(Application.dataPath + "/Photos/" + itemsDisplayed[obj].item.Name)) {
                    fileData = System.IO.File.ReadAllBytes(Application.dataPath + "/Photos/" + itemsDisplayed[obj].item.Name);
                    texture = new Texture2D(1920, 1080);
                    texture.LoadImage(fileData);
                }

                if (imageObject.gameObject.activeSelf && texture == image.texture) {
                    image.texture = null;
                    imageObject.gameObject.SetActive(false);
                } else {
                    imageObject.gameObject.SetActive(true);
                    if (texture == null) {
                        return;
                    }
                    image.texture = texture;
                }
            } else if (itemsDisplayed[obj].item.Type == ItemType.Equipment) {
                SwitchCamera camera = GameObject.Find("Player").GetComponent<SwitchCamera>();

                if (itemsDisplayed[obj].item.Name.ToLower().Contains("Kamera".ToLower())) {
                    camera.ChangeCamera((EquipmentItem)itemsDisplayed[obj].item);
                } else if (itemsDisplayed[obj].item.Name.ToLower().Contains("Objektív".ToLower())) {
                    camera.ChangeLens((EquipmentItem)itemsDisplayed[obj].item);
                }
            }
        }

    }

    public Vector3 GetPosition(int i) {
        return new Vector3(X_START + ((X_SPACE_BETWEEN_ITEMS + SLOT_SIZE) * (i % NUMBER_OF_COLUMNS)), Y_START + (-(Y_SPACE_BETWEEN_ITEMS + SLOT_SIZE) * (i / NUMBER_OF_COLUMNS)), 0f);
    }
        
    private void ShowInfo(GameObject obj) {
        InfoBox box = transform.parent.gameObject.GetComponent<InfoBox>();
        if (box == null) {
            box = transform.parent.gameObject.AddComponent<InfoBox>();
        }
        string message;
        if (itemsDisplayed[obj].ID == 4) {
            message = "Eldobt�l egy fot�t";
        } else {
            message = "Eldobt�l " + itemsDisplayed[obj].amount + " db " + itemsDisplayed[obj].item.Name.ToString() + "-t";
        }
        box.ShowBox(message, true);
    }
}

public class MouseItem {
    public GameObject obj;
    public InventorySlot item;
    //public InventorySlot hoverItem;
    public GameObject hoverObj;
}
