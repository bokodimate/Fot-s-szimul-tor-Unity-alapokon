using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestType {
    Photo,
    Collect
}

[System.Serializable]
public class Quest
{
    private Guid ID;
    private GameObject QuestGiver;
    private string ObjectName;
    private int CountNeeded;
    private int Count;
    private QuestType Type;
    private bool QuestDone;
    private EquipmentObject Reward;
    private string Options;

    public Quest(Guid id, GameObject questGiver, int countNeeded, string objectName, QuestType type, EquipmentObject reward, string options) {
        ID = id;
        QuestGiver = questGiver;
        CountNeeded = countNeeded;
        ObjectName = objectName;
        Type = type;
        Reward = reward;
        QuestDone = false;
        Count = 0;
        Options = options;
    }

    public Guid id {
        get {
            return ID;
        }
        set { }
    }

    public GameObject questGiver {
        get {
            return QuestGiver;
        }
    }

    public int count {
        get {
            return Count;
        }
        set {
            Count = value;
        }
    }

    public int countNeeded {
        get {
            return CountNeeded;
        }
        set {
            CountNeeded = value;
        }
    }

    public QuestType type {
        get {
            return Type;
        }
    }

    public string objectName {
        get {
            return ObjectName;
        }
    }

    public bool questDone {
        get {
            return QuestDone;
        }
        set {
            QuestDone = value;
        }
    }

    public EquipmentObject reward {
        get {
            return Reward;
        }
    }
    public string options {
        get {
            return Options;
        }
    }

    public override string ToString() {
        return "ID: " + ID + " | QuestGiver: " + QuestGiver.name + " | CountNeeded: " + CountNeeded + " | Count: " + Count + " | Type: " + Type.ToString() + " | Reward: " + Reward.name;
    }
}
