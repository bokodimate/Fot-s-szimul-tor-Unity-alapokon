using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject {
    public ItemDatabaseObject database;
    public Inventory Container;

    public void AddItem(Item _item, int _amount) {

        if (_item.Properties.Length > 0) {
            SetEmptySlot(_item, _amount);
            return;
        }

        for (int i = 0; i < Container.Items.Length; i++) {
            if (Container.Items[i].ID == _item.Id) {
                Container.Items[i].AddAmount(_amount);
                return;
            }
        }

        SetEmptySlot(_item, _amount);
    }

    public void AddItem(EquipmentItem _item, int _amount) {
        SetEmptySlot(_item, _amount);
    }

    public InventorySlot SetEmptySlot(Item _item, int _amount) {
        for (int i = 0; i < Container.Items.Length; i++) {
            if (Container.Items[i].ID <= -1) {
                Container.Items[i].UpdateSlot(_item.Id, _item, _amount);
                return Container.Items[i];
            }
        }
        return null;
    }

    public void MoveItem(InventorySlot item1, InventorySlot item2) {
        InventorySlot temp = new InventorySlot(item2.ID, item2.item, item2.amount);
        item2.UpdateSlot(item1.ID, item1.item, item1.amount);
        item1.UpdateSlot(temp.ID, temp.item, temp.amount);
    }

    public void RemoveItem(Item _item) {
        for (int i = 0; i < Container.Items.Length; i++) {
            if(Container.Items[i].item == _item) {
                Container.Items[i].UpdateSlot(-1, null, 0);
            }
        }
        GameObject.Find("Player").GetComponent<QuestManager>().UpdateQuests();
    }

}

[System.Serializable]
public class Inventory {
    public InventorySlot[] Items = new InventorySlot[28];
}


[System.Serializable]
public class InventorySlot {
    public int ID;
    public Item item;
    public int amount;
    public InventorySlot() {
        ID = -1;
        item = null;
        amount = 0;
    }

    public InventorySlot(int _id, Item _item, int _amount) {
        ID = _id;
        item = _item;
        amount = _amount;
    }

    public void UpdateSlot(int _id, Item _item, int _amount) {
        ID = _id;
        item = _item;
        amount = _amount;
    }


    public void AddAmount(int value) {
        amount += value;
    }
}
