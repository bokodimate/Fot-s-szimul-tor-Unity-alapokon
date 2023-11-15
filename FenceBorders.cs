using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenceBorders : MonoBehaviour
{
    public GameObject fencePrefab;
    // Start is called before the first frame update
    void Start()
    {

        for(float x=1000; x>0; x-=6.6f) {
            float z = 0;
            float y = Terrain.activeTerrain.SampleHeight(new Vector3(x, 0, z));
            float nextY = Terrain.activeTerrain.SampleHeight(new Vector3(x - 6.6f, 0, z));

            var a = Mathf.Abs(y - nextY);
            var b = 6.6f;
            var rotationRad = Mathf.Atan2(a, b);
            var rotationDeg = (180/Mathf.PI)*rotationRad;

            if (y < nextY) {
                rotationDeg = -rotationDeg;
            }

            GameObject fence = Instantiate(fencePrefab, new Vector3(x, y, z), Quaternion.Euler(0f, 0f, rotationDeg));
            fence.transform.SetParent(this.transform.Find("FenceBorder").transform);
        }

        for (float x = 1000; x > 0; x -= 6.6f) {
            float z = 1000;
            float y = Terrain.activeTerrain.SampleHeight(new Vector3(x, 0, z));
            float nextY = Terrain.activeTerrain.SampleHeight(new Vector3(x - 6.6f, 0, z));

            var a = Mathf.Abs(y - nextY);
            var b = 6.6f;
            var rotationRad = Mathf.Atan2(a, b);
            var rotationDeg = (180 / Mathf.PI) * rotationRad;

            if (y < nextY) {
                rotationDeg = -rotationDeg;
            }

            GameObject fence = Instantiate(fencePrefab, new Vector3(x, y, z), Quaternion.Euler(0f, 0f, rotationDeg));
            fence.transform.SetParent(this.transform.Find("FenceBorder").transform);
        }

        for (float z = 0; z < 1000; z += 6.6f) {
            float x = 1000;
            float y = Terrain.activeTerrain.SampleHeight(new Vector3(x, 0, z));
            float nextY = Terrain.activeTerrain.SampleHeight(new Vector3(x, 0, z+ 6.6f));

            var a = Mathf.Abs(y - nextY);
            var b = 6.6f;
            var rotationRad = Mathf.Atan2(a, b);
            var rotationDeg = (180 / Mathf.PI) * rotationRad;

            if (y < nextY) {
                rotationDeg = -rotationDeg;
            }

            GameObject fence = Instantiate(fencePrefab, new Vector3(x, y, z), Quaternion.Euler(0f, 90f, rotationDeg));
            fence.transform.SetParent(this.transform.Find("FenceBorder").transform);
        }

        for (float z = 1000; z > 0; z -= 6.6f) {
            float x = 0;
            float y = Terrain.activeTerrain.SampleHeight(new Vector3(x, 0, z));
            float nextY = Terrain.activeTerrain.SampleHeight(new Vector3(x, 0, z - 6.6f));

            var a = Mathf.Abs(y - nextY);
            var b = 6.6f;
            var rotationRad = Mathf.Atan2(a, b);
            var rotationDeg = (180 / Mathf.PI) * rotationRad;

            if (y < nextY) {
                rotationDeg = -rotationDeg;
            }

            GameObject fence = Instantiate(fencePrefab, new Vector3(x, y, z), Quaternion.Euler(0f, -90f, rotationDeg));
            fence.transform.SetParent(this.transform.Find("FenceBorder").transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
