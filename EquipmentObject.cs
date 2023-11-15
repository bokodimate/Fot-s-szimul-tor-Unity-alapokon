using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment Object", menuName = "Inventory System/Items/Equipment")]
public class EquipmentObject : ItemObject
{
    private bool _equipped;

    public EquipmentObject () {

    }
    private void Awake() {
        type = ItemType.Equipment;
    }
}

public class EquipmentItem : Item {
    public bool Equipped;
    public EquipmentItem(EquipmentObject item) : base(item) {
        Equipped = false;
    }
}