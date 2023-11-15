using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour {
    [SerializeField]
    protected List<Quest> quests = new List<Quest>();

    public InventoryObject inventory;

    public void AddQuest(Quest quest) {
        quests.Add(quest);
        UpdateQuests();
    }
    public void RemoveQuest(Guid questID) {
        quests.Remove(quests.Find(x => x.id == questID));
        Destroy(GameObject.Find(questID.ToString()).gameObject);
    }

    public void CompleteQuest(Quest quest) {
        RemoveQuest(quest.id);
        Destroy(GameObject.Find(quest.id.ToString()).gameObject);
        InventoryObject inventory = this.GetComponentInParent<Pick_Items>().inventory;
        var counter = 0;
        for (var i = 0; i < inventory.Container.Items.Length; i++) {
            if (inventory.Container.Items[i].ID == -1) continue;
            if (quest.type == QuestType.Collect) {
                if (inventory.Container.Items[i].item.Name == quest.objectName) {
                    inventory.Container.Items[i].amount -= quest.countNeeded;
                    if (inventory.Container.Items[i].amount <= 0) {
                        inventory.RemoveItem(inventory.Container.Items[i].item);
                    }
                    break;
                }
            } else if (quest.type == QuestType.Photo) {
                try {
                    for (var j = 0; j < (inventory.Container.Items[i].item as PhotoItem).Animals.Count; j++) {
                        if ((inventory.Container.Items[i].item as PhotoItem).Animals[j] == quest.objectName) {
                            inventory.RemoveItem(inventory.Container.Items[i].item);
                            counter++;
                        }
                    }

                    if (counter == quest.countNeeded) {
                        break;
                    }
                } catch (Exception) {

                }
            }
        }

        inventory.AddItem(new EquipmentItem(quest.reward), 1);
        GameObject.Find("InfoBox").GetComponent<InfoBox>().ShowBox("Kaptál egy " + quest.reward.name + "-t!", true);

    }

    public void UpdateQuests() {
        foreach (var quest in quests) {
            int count = 0;
            if (quest.type == QuestType.Collect) {

                for (var i = 0; i < inventory.Container.Items.Length; i++) {
                    if (inventory.Container.Items[i].ID == -1) {
                        continue;
                    } else {
                        if (quest.objectName == inventory.Container.Items[i].item.Name) {
                            count += inventory.Container.Items[i].amount;
                        }
                    }
                }
            } else if (quest.type == QuestType.Photo) {
                for (var i = 0; i < inventory.Container.Items.Length; i++) {
                    if (inventory.Container.Items[i].ID == -1 || inventory.Container.Items[i].item.Type != ItemType.Photo) {
                        continue;
                    } else {
                        for (var j = 0; j < (inventory.Container.Items[i].item as PhotoItem).Animals.Count; j++) {
                            if ((inventory.Container.Items[i].item as PhotoItem).Animals[j].Contains(quest.objectName)) {
                                if (quest.options == "fromblind") {
                                    if ((inventory.Container.Items[i].item as PhotoItem).IsOnBlind) {
                                        count++;
                                    }
                                } else {
                                    count++;
                                }
                            }
                        }
                    }
                }
            }
            quest.count = count;
            CheckState(quest);
        }

    }

    public void CheckState(Quest quest) {
        if (quest.count >= quest.countNeeded) {
            quest.questDone = true;
            //GameObject.Find(quest.id.ToString()).GetComponent<TMPro.TextMeshProUGUI>().overrideColorTags = true;
            GameObject.Find(quest.id.ToString()).GetComponent<TMPro.TextMeshProUGUI>().color = Color.yellow;
        } else {
            quest.questDone = false;
            GameObject.Find(quest.id.ToString()).GetComponent<TMPro.TextMeshProUGUI>().color = Color.gray;
        }
        quest.questGiver.GetComponent<QuestGiver>().QuestState(quest);
    }
}
