using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Photo Object", menuName = "Inventory System/Items/Photo")]
public class PhotoObject : ItemObject
{
    private string _path;
    private List<string> Animals = new List<string>();
    private bool IsOnBlind;
    private void Awake() {
        type = ItemType.Photo;
    }
    public PhotoObject() {

    }

    public string Path {
        get {
            return _path;
        }
        set {
            _path = value;
        }
    }

    public List<string> animals {
        get {
            return Animals;
        }
        set {
            Animals = value;
        }
    }
}

public class PhotoItem : Item {
    private string _path;
    public List<string> Animals = new List<string>();
    private bool _isOnBlind;
    public PhotoItem(PhotoObject item) : base(item) {
        _path = item.Path;
        Animals = item.animals;
    }
    public PhotoObject item;
    public string Path {
        get {
            return _path;
        }
        set {
            _path = value;
        }
    }

    public bool IsOnBlind {
        get {
            return _isOnBlind;
        }
        set {
            _isOnBlind = value;
        }
    }

}