using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour {
    public List<GameObject> prefabs = new List<GameObject>();
    public int spawnNumber = 50;

    List<GameObject> objects = new List<GameObject>();
    // Start is called before the first frame update
    void Start() {
        this.GetComponent<MeshRenderer>().enabled = false;

        Vector3 min = this.GetComponent<BoxCollider>().bounds.min;
        Vector3 max = this.GetComponent<BoxCollider>().bounds.max;
        for (var i = 0; i < spawnNumber; i++) {
            var randomX = Random.Range(min.x, max.x);
            var randomZ = Random.Range(min.z, max.z);

            var randomY = Terrain.activeTerrain.SampleHeight(new Vector3(randomX, 0, randomZ));

            GameObject prefab = prefabs[Random.Range(0, prefabs.Count)];

            Vector3 randomPosition = new Vector3(randomX, randomY, randomZ);
            var newObject = Instantiate(prefab, randomPosition, Quaternion.identity);
            
            newObject.transform.SetParent(this.transform);

            if (newObject.CompareTag("Pickables")) {
                newObject.GetComponent<Text>().text = i.ToString();
            }
            
            if (newObject.layer == 6) {
                newObject.GetComponent<Rigidbody>().isKinematic = true;

                if (newObject.CompareTag("Fish")) {
                    newObject.transform.localScale = new Vector3(0.2f, 2, 0.2f);
                }
            }
        }
    }

    // Update is called once per frame
    void Update() {

    }
}
