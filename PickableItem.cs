using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PickableItem : MonoBehaviour {
    // This script is on the pickable object
    
    public GameObject text;
    public Rigidbody Rb => rb;

    protected Rigidbody rb;

    private void Start() {
        text = new GameObject();
        text.name = "Text";
        text.transform.parent = this.transform;
        TextMeshPro t = text.AddComponent<TextMeshPro>();
        t.text = this.GetComponent<GroundItem>().item.description;
        if (t.text == "") {
            t.text = this.GetComponent<GroundItem>().item.name;
        }
        t.fontSize = 2;
        t.margin = new Vector4(9, 1.7f, 9, 2);
        t.horizontalAlignment = HorizontalAlignmentOptions.Center;
        text.SetActive(false);
        text.transform.position = transform.position;

        CapsuleCollider capsule = this.transform.gameObject.AddComponent<CapsuleCollider>();
        capsule.radius = 0.5f;
        capsule.height = 2;
        capsule.center = new Vector3(0, capsule.height/2, 0);
        capsule.isTrigger = true;
    }
    void Awake() {
        rb = GetComponent<Rigidbody>();
    }
    private void Update() {
        text.gameObject.transform.LookAt(Camera.main.transform);
        text.gameObject.transform.rotation = Camera.main.transform.rotation;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            GroundItem item = this.GetComponent<GroundItem>();

            InventoryObject inventory = other.gameObject.GetComponent<Pick_Items>().inventory;

            if (item) {
                if (item.item.type == ItemType.Equipment) {
                    inventory.AddItem(new EquipmentItem(item.item as EquipmentObject), 1);
                } else {
                    inventory.AddItem(new Item(item.item), 1);
                }
               
                other.gameObject.GetComponent<QuestManager>().UpdateQuests();

                InfoBox box = gameObject.transform.parent.gameObject.GetComponent<InfoBox>();
                if ( box == null) { 
                    box = gameObject.transform.parent.gameObject.AddComponent<InfoBox>();
                }
                box.ShowBox("Felvettél egy " + item.item.name + "-t", true);

                Destroy(this.gameObject);
                
            }
        }
    }

}