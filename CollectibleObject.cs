using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Collectible Object", menuName = "Inventory System/Items/Collectible")]
public class CollectibleObject : ItemObject
{
    public void Awake() {
        type = ItemType.Collectible;
    }
}
