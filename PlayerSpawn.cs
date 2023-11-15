using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public GameObject player;
    public bool defaultSpawn = false;
    public bool VillageSpawn = false;
    public bool LakeSpawn = false;
    void Start()
    {
        Vector3 targetPosition;
        if (defaultSpawn) {
            targetPosition = transform.Find("Default").transform.position;
        } else if (VillageSpawn) {
            targetPosition = transform.Find("Village").transform.position;
        } else if (LakeSpawn) {
            targetPosition = transform.Find("Lake").transform.position;
        } else {
            targetPosition = transform.Find("Default").transform.position;
        }
        targetPosition.y = Terrain.activeTerrain.SampleHeight(targetPosition) + 2;
        player.transform.position = targetPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
