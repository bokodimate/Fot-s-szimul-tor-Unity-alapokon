using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalMovement : MonoBehaviour {
    private int positionNumber = 1;
    private bool isFish = false;
    Vector3 theDestination;
    bool lookRotationSet = false;
    public Animator anim;
    string currentAnimation;
    
    System.Random rnd = new System.Random();

    float distance = 10;
    float angle = 67;
    float height = 10.0f;
    int scanFrequency = 30;
    public LayerMask layers;
    public LayerMask occlusionLayers;

    Collider[] colliders = new Collider[50];
    float scanInterval;
    float scanTimer;
    int count;
    Color meshColor = Color.red;
    Mesh mesh;

    public float speed = 2f;
    // Start is called before the first frame update

    void Start() {
        scanInterval = 1.0f / scanFrequency;

        theDestination = CalculateNextLocation();

        if (this.CompareTag("Fish")) {
            isFish = true;
        }

        if (!isFish) {
            currentAnimation = "walk";
            anim.Play(currentAnimation);
        }

        StartCoroutine(NextLocation());
    }

    // Update is called once per frame
    void Update() {
        scanTimer -= Time.deltaTime;
        if (scanTimer < 0) {
            scanTimer += scanInterval;
            Scan();


            //Vector3 curPos = transform.position;
            //if (Terrain.activeTerrain.SampleHeight(transform.position)-1 >= transform.position.y) {
            //    curPos.y = Terrain.activeTerrain.SampleHeight(transform.position);
            //    transform.position = curPos;
            //}
            Vector3 curPos = transform.position;
            curPos.y = Terrain.activeTerrain.SampleHeight(transform.position);
            transform.position = curPos;
        }

        if (!lookRotationSet) {
            lookRotationSet = true;
            transform.LookAt(theDestination);
        }

        if (Math.Floor(transform.position.x) == Math.Floor(theDestination.x) && Math.Floor(transform.position.z) == Math.Floor(theDestination.z)) {
            if (!isFish) { 
                currentAnimation = "idle";
                try { 
                    anim.Play(currentAnimation);
                } catch (Exception) {

                }
                //transform.GetComponent<Rigidbody>().isKinematic = true;
            }

            lookRotationSet = false;
        }


        var step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, theDestination, step);
    }

    IEnumerator NextLocation() {
        if (positionNumber == 1) {
            yield return new WaitForSeconds(20);

            if (!isFish) {
                currentAnimation = "walk";
                anim.Play(currentAnimation);
               // transform.GetComponent<Rigidbody>().isKinematic = false;
            }

            theDestination = CalculateNextLocation();
            
            positionNumber = 2;
            speed = 2f;

        }
        if (positionNumber == 2) {
            yield return new WaitForSeconds(20);

            if (!isFish) {
                currentAnimation = "walk";
                anim.Play(currentAnimation);
                //transform.GetComponent<Rigidbody>().isKinematic = false;
            }

            theDestination = CalculateNextLocation();
            
            positionNumber = 1;
            speed = 2f;

            StartCoroutine(NextLocation());
        }
        if (positionNumber == 3) {
            yield return new WaitForSeconds(20);
           
            if (!isFish) {
                currentAnimation = "run";
                anim.Play(currentAnimation);
                //transform.GetComponent<Rigidbody>().isKinematic = false;
                speed = 5f;
            }
            
            positionNumber = 3;

            StartCoroutine(NextLocation());
        }
    }

    private Vector3 CalculateNextLocation() {

        lookRotationSet = false;

        float x = rnd.Next((int)transform.position.x - 50, (int)transform.position.x + 50);
        float z = rnd.Next((int)transform.position.z - 50, (int)transform.position.z + 50);

        if (x < 0) {
            x = 100;
        }
        if (z < 0) {
            z = 100;
        }
        float y = Terrain.activeTerrain.SampleHeight(new Vector3(x, 0, z));


        if (isFish) {
            Vector3 min = transform.parent.GetComponent<BoxCollider>().bounds.min;
            Vector3 max = transform.parent.GetComponent<BoxCollider>().bounds.max;
            x = rnd.Next((int)min.x, (int)max.x);
            y = rnd.Next((int)min.y, (int)max.y);
            z = rnd.Next((int)min.z, (int)max.z);
        }

        return new Vector3(x, y, z);
    }

    private void Run(GameObject player) {
        
        StopCoroutine(NextLocation());

        transform.LookAt(player.transform.position);

        positionNumber = 3;
        speed = 5f;


        Vector3 tempPos = player.transform.position - transform.position;
        tempPos *= 3;
        Vector3 newPos =  transform.position - tempPos;

        if (newPos.x < 0) {
            newPos.x = 5;
        }
        if (newPos.z < 0) {
            newPos.z = 5;
        }

        newPos.y = Terrain.activeTerrain.SampleHeight(new Vector3(newPos.x, 0, newPos.z));

        theDestination = newPos;
        transform.LookAt(theDestination);
    }

    private void Scan() {
        count = Physics.OverlapSphereNonAlloc(transform.position, distance, colliders, layers, QueryTriggerInteraction.Collide);
        for (int i = 0; i < count; i++) {
            GameObject obj = colliders[i].gameObject;
            if (obj.CompareTag("Player") && IsInSight(obj)) {
                Run(obj);
            }
        }
    }

    public bool IsInSight(GameObject obj) {

        Vector3 origin = new Vector3(transform.position.x, transform.position.y - (height / 2), transform.position.z);

        Vector3 boxPos = obj.GetComponent<BoxCollider>().transform.position;
        Vector3 dest = new Vector3(boxPos.x, boxPos.y, boxPos.z);

        Vector3 direction = dest - origin;

        if (direction.y < 0 || direction.y > height) {
            return false;
        }

        direction.y = 0;
        float deltaAngle = Vector3.Angle(direction, transform.forward);

        if (deltaAngle > angle) {
            return false;
        }

        return true;
    }
    void OnDrawGizmosSelected() {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, theDestination);
        Gizmos.DrawSphere(theDestination, 1);
    }

}
