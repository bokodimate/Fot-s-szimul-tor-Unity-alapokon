using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuestGiver : MonoBehaviour {
    public Stack<Quest> quests = new Stack<Quest>();
    private string questName = "";
    private string questTask = "";
    private Guid questID;
    private Canvas questCanvas;
    public int count = 0;
    public QuestType questType;
    public ItemObject reward;

    string objName;
    private TextMeshPro textComponent;

    private bool QuestAccepted = false;
    private Button cancelButton;
    private Button acceptButton;
    private Button removeButton;

    private Quest quest;

    private bool questCompleted = false;

    public void Init(bool showQuestWindow) {
        AddQuestIcon();
        questCanvas = GameObject.Find("QuestCanvas").GetComponent<Canvas>();
        questCanvas.enabled = false;

        if (quests.Count > 0) {
            quest = quests.Peek();
            questID = quest.id;
            objName = quest.objectName;
            questType = quest.type;
            count = quest.countNeeded;
            reward = quest.reward;

            textComponent.text = "!";
            textComponent.color = Color.yellow;

            if (showQuestWindow) {
                ShowBox();
            }
        } else {
            questCompleted = true;
        }

        string objNameText;

        switch (objName) {
            case "Wild_rabbit_prefab":
                objNameText = "Vadnyúl";
                break;
            case "Wild_boar_prefab":
                objNameText = "Vaddisznó";
                break;
            case "Antler":
                objNameText = "Agancs";
                break;
            case "Brown_bear_prefab":
                objNameText = "Barna medve";
                break;
            default:
                objNameText = objName;
                break;
        }

        string options = "";
        switch (quest.options) {
            case "fromblind":
                options = "A fotót magaslesről készítsd!";
                break;
        }

        switch (questType) {
            case QuestType.Photo:
                questName = "Fotózás";
                questTask = "Készíts " + "\n" +
                        count + " db fotót " + "\n" +
                        objNameText + " állatról!" +
                       options + "\n" +
                        "\n" +
                        "Jutalom: \n" + quest.reward.name;
                break;
            case QuestType.Collect:
                questName = "Gyűjtés";
                questTask = "Gyűjts " + "\n" +
                count + " db " + objNameText + "-t \n\n" +
                "Jutalom: \n" + quest.reward.name;
                break;
        }

        foreach (var x in questCanvas.GetComponentsInChildren<TextMeshProUGUI>()) {
            if (x.name == "QuestCim") {
                x.text = questName;
            } else if (x.name == "QuestFeladat") {
                x.text = questTask;
            }
        }

        if (cancelButton == null) cancelButton = questCanvas.GetComponentsInChildren<Button>()[0];
        if (acceptButton == null) acceptButton = questCanvas.GetComponentsInChildren<Button>()[1];
        if (removeButton == null) removeButton = questCanvas.GetComponentsInChildren<Button>()[2];
    }

    private void AddQuestIcon() {
        if (textComponent != null) return;
        GameObject textObj = new GameObject();
        textObj.transform.SetParent(this.gameObject.transform, false);
        textComponent = textObj.AddComponent<TextMeshPro>();

        textComponent.fontSize = 5;
        textComponent.verticalAlignment = VerticalAlignmentOptions.Middle;
        textComponent.horizontalAlignment = HorizontalAlignmentOptions.Center;
        textComponent.rectTransform.localPosition = new Vector3(0, 2.2f, 0);
        textComponent.rectTransform.sizeDelta = new Vector2(1, 1);
    }
    void Update() {
        if (textComponent != null) {
            textComponent.gameObject.transform.LookAt(Camera.main.transform);
            textComponent.gameObject.transform.rotation = Camera.main.transform.rotation;
        }
    }

    private void ShowBox() {
        cancelButton.onClick.RemoveAllListeners();
        acceptButton.onClick.RemoveAllListeners();
        removeButton.onClick.RemoveAllListeners();

        if (questCanvas == null) return;
        questCanvas.enabled = true;
        questCanvas.GetComponent<CanvasGroup>().alpha = 1;
        questCanvas.GetComponentInChildren<TextMeshProUGUI>().text = questName;

        foreach (var x in questCanvas.GetComponentsInChildren<TextMeshProUGUI>()) {
            if (x.name == "QuestCim") {
                x.text = questName;
            } else if (x.name == "QuestFeladat") {
                x.text = questTask;
            }
        }

        if (QuestAccepted) {
            acceptButton.gameObject.SetActive(false);
            removeButton.gameObject.SetActive(true);
        } else {
            acceptButton.gameObject.SetActive(true);
            removeButton.gameObject.SetActive(false);
            acceptButton.GetComponentInChildren<TextMeshProUGUI>().text = "Elfogadás";
        }

        cancelButton.onClick.AddListener(HideQuest);
        acceptButton.onClick.AddListener(AcceptQuest);
        removeButton.onClick.AddListener(RemoveQuest);

        cancelButton.interactable = true;
        acceptButton.interactable = true;
        removeButton.interactable = true;

        if (quest != null && quest.questDone) {
            acceptButton.gameObject.SetActive(true);
            removeButton.gameObject.SetActive(false);
            acceptButton.GetComponentInChildren<TextMeshProUGUI>().text = "Teljesítve";
            acceptButton.onClick.RemoveAllListeners();
            acceptButton.onClick.AddListener(CompleteQuest);
        }

        // Show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void HideQuest() {
        if (questCanvas == null) return;
        questCanvas.enabled = false;
        questCanvas.GetComponent<CanvasGroup>().alpha = 0;

        cancelButton.onClick.RemoveAllListeners();
        acceptButton.onClick.RemoveAllListeners();
        removeButton.onClick.RemoveAllListeners();

        cancelButton.interactable = false;
        acceptButton.interactable = false;
        removeButton.interactable = false;

        // Hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void AcceptQuest() {
        //questID = Guid.NewGuid();
        //quest = new Quest(questID, this.gameObject, count, obj.name, questType, reward);
        if (questCanvas.enabled && !QuestAccepted) {
            try {
                textComponent.text = "?";
                textComponent.color = Color.gray;
                QuestAccepted = true;
                SwitchButtons();
                QuestInfo(quest);
                GameObject.Find("Player").GetComponent<QuestManager>().AddQuest(quest);
            } catch (Exception e) {
                Debug.Log(e.Message);
            }
        }
        acceptButton.gameObject.SetActive(false);
        removeButton.gameObject.SetActive(true);
        cancelButton.onClick.RemoveAllListeners();
        acceptButton.onClick.RemoveAllListeners();
        removeButton.onClick.RemoveAllListeners();
    }

    private void RemoveQuest() {
        try {
            GameObject.Find("Player").GetComponent<QuestManager>().RemoveQuest(questID);

        } catch (Exception e) {
            Debug.Log(e.Message);
        }
        QuestAccepted = false;
        SwitchButtons();
        textComponent.text = "!";
        textComponent.color = Color.yellow;
    }

    private void CompleteQuest() {
        GameObject.Find("Player").GetComponent<QuestManager>().CompleteQuest(quest);

        quests.Pop();
        acceptButton.GetComponentInChildren<TextMeshProUGUI>().text = "Elfogadás";

        questCompleted = false;
        QuestAccepted = false;
        textComponent.text = "";



        HideQuest();
        Init(true);
    }

    private void QuestInfo(Quest quest) {
        var qInfo = GameObject.Find("QuestInfo");
        Vector2 canvasSize = GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta;
        RectTransform qInfoRect = qInfo.GetComponent<RectTransform>();
        qInfo.GetComponent<RectTransform>().localPosition = new Vector2((canvasSize.x / 2) - qInfoRect.sizeDelta.x / 2, (canvasSize.y / 2) - qInfoRect.sizeDelta.y / 2);
        int childCount = qInfo.transform.childCount;
        var parentObject = GameObject.Find(quest.id.ToString());
        TextMeshProUGUI textObj;
        if (parentObject == null) {
            parentObject = new GameObject();
            parentObject.transform.parent = qInfo.transform;
            parentObject.name = quest.id.ToString();
            textObj = parentObject.AddComponent<TextMeshProUGUI>();
            textObj.fontSize = 28;
            parentObject.GetComponent<RectTransform>().sizeDelta = new Vector2(qInfoRect.sizeDelta.x, 75);
            //parentObject.GetComponent<RectTransform>().localPosition = new Vector2(0, -80 * childCount);
            parentObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

        } else {
            textObj = parentObject.GetComponent<TextMeshProUGUI>();
        }

        for (int i = 0; i < childCount; i++) {
            qInfo.transform.GetChild(i).GetComponent<RectTransform>().localPosition = new Vector2(0, -80 * i);
        }

        textObj.text = questTask.Replace("\n", "").Split("Jutalom")[0] + "\n";
        textObj.text += quest.count + "/" + quest.countNeeded;

    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player") && !questCompleted) {
            ShowBox();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            HideQuest();
        }
    }

    private void SwitchButtons() {
        if (acceptButton.gameObject.activeSelf) {
            acceptButton.gameObject.SetActive(false);
            removeButton.gameObject.SetActive(true);
        } else {
            acceptButton.gameObject.SetActive(true);
            removeButton.gameObject.SetActive(false);
        }
    }

    public void QuestState(Quest quest) {
        QuestInfo(quest);
        if (quest.questDone) {
            textComponent.text = "?";
            textComponent.color = Color.yellow;
            acceptButton.gameObject.SetActive(true);
            removeButton.gameObject.SetActive(false);
            acceptButton.GetComponentInChildren<TextMeshProUGUI>().text = "Teljesítve";
            acceptButton.onClick.RemoveAllListeners();
            acceptButton.onClick.AddListener(CompleteQuest);


            GameObject.Find("InfoBox").GetComponent<InfoBox>().ShowBox("Küldetés teljesítve", true);

        } else {
            textComponent.text = "?";
            textComponent.color = Color.gray;
        }
    }
}
