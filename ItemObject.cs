using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType {
    Default,
    Equipment,
    Photo,
    Collectible
}

public enum Attributes {
    FocalLengthMin,
    FocalLengthMax,
    ApertureMin,
    ApertureMax,
    Aperture,
    FocalLength,
    Iso,
    FromBlind
}
public abstract class ItemObject : ScriptableObject {
    public int Id;
    public Sprite uiDisplay;
    public ItemType type;
    public string ObjectName;
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
    public string name;
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
    [TextArea(15, 20)]

    public string description;

    public ItemProperties[] properties;


}

[System.Serializable]
public class Item {
    public string Name;
    public string ObjectName;
    public int Id;
    public ItemProperties[] Properties;
    public ItemType Type;

    public Item(ItemObject item) {
        ObjectName = item.ObjectName;
        Name = item.name;
        Id = item.Id;
        Properties = new ItemProperties[item.properties.Length];
        for (int i = 0; i < Properties.Length; i++) {
            Properties[i] = new ItemProperties(item.properties[i].value) {
                attribute = item.properties[i].attribute
            };
        }
        Type = item.type;
    }


}

[System.Serializable]
public class ItemProperties {
    public Attributes attribute;
    public float value;
    public ItemProperties(float value) {
        this.value = value;
    }


    public void setValue(float value) {
        this.value = value;
    }

}