 using UnityEngine;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Collections.Generic;
using System;

public class DataManager : MonoBehaviour {
    private void Start() {
        LoadXMLData();
    }

    [ContextMenu("Load XML Data")]
    public void LoadXMLData() {
        List<Quest> quests = new List<Quest>();
        ItemDatabaseObject database = GameObject.Find("Player").GetComponent<Pick_Items>().inventory.database;
        List<GameObject> questGivers = new List<GameObject>();

        XmlDocument doc = new XmlDocument();
        string path = $"{Application.dataPath}/Resources/quests.xml";
        doc.Load(path);

        XmlNode parent = doc.SelectSingleNode("quests");

        XmlNodeList node = parent.SelectNodes("quest");

        for (var j = 0; j < node.Count; j++) {
            Guid id = Guid.NewGuid();
            GameObject questGiver = GameObject.Find("QuestGiver_" + node.Item(j).ChildNodes[0].InnerText);
            int countNeeded = int.Parse(node.Item(j).ChildNodes[1].InnerText);
            string objectName = node.Item(j).ChildNodes[2].InnerText;
            string options = node.Item(j).ChildNodes[4].InnerText;
            QuestType type;
            Enum.TryParse<QuestType>(node.Item(j).ChildNodes[3].InnerText, out type);
            EquipmentObject reward;

            int i = 0;
            for (i = 0; i < database.Items.Length; i++) {
                if (database.Items[i].name == node.Item(j).ChildNodes[5].InnerText) { 
                    break;
                }
            }
            reward = (EquipmentObject)database.Items[i];
            Quest quest = new Quest(id, questGiver, countNeeded, objectName, type, reward, options);
            
            if (!questGivers.Contains(questGiver)) {
                questGivers.Add(questGiver);
            }

            quests.Add(quest);
        }

        for (var i = quests.Count - 1; i >= 0; i--) {
            quests[i].questGiver.GetComponent<QuestGiver>().quests.Push(quests[i]);
        }


        foreach (GameObject questGiver in questGivers) {
            questGiver.GetComponent<QuestGiver>().Init(false);
        }

    }

}