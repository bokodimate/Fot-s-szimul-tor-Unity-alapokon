using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManagement : MonoBehaviour
{
    public GameObject target;
    public Camera cam;

    float scanInterval;
    float scanTimer;
    public int scanFrequency = 30;

    Collider[] colliders = new Collider[50];
#pragma warning disable IDE0044 // Add readonly modifier
    Mesh mesh;
#pragma warning restore IDE0044 // Add readonly modifier
    int count;
    public LayerMask layers;
    public LayerMask occlusionLayers;
    public List<GameObject> Objects = new List<GameObject>();
    public float distance = 10;

    public float angle = 30;
    public float height = 10.0f;

    private void Start() {
        scanInterval = 1.0f / scanFrequency;
    }
    private bool IsVisible(GameObject target) {
        var planes = GeometryUtility.CalculateFrustumPlanes(cam);
        var point = target.transform.position;

        foreach (var plane in planes) {
            if (plane.GetDistanceToPoint(point) < 0) {
                return false;
            }
        }
        return true;
    }

    private void Update() {

        //DrawFrustum(cam);
        angle = cam.GetComponent<Camera>().fieldOfView;


        scanTimer -= Time.deltaTime;
        if (scanTimer < 0) {
            scanTimer += scanInterval;
            Scan();
        }
    }
    private void OnDrawGizmos() {

        if (mesh) {
            Gizmos.color = Color.red;

            Gizmos.DrawMesh(mesh, new Vector3(transform.position.x, transform.position.y - (height / 2), transform.position.z), transform.rotation);
        }
        Gizmos.DrawWireSphere(transform.position, distance);

        for (int i = 0; i < count; i++) {
            
            BoxCollider collider = colliders[i].GetComponent<BoxCollider>();
           // Vector3 boxScale = collider.size * collider.transform.parent.localScale.x;
            float boxScale = collider.transform.parent.localScale.x;
            Vector3 boxPos = collider.transform.position;
            Vector3 boxCenter =  collider.center;

            Gizmos.DrawSphere(new Vector3(boxPos.x + boxCenter.x, boxPos.y + boxCenter.y + 1, boxPos.z + boxCenter.z), 0.3f);
            //Gizmos.DrawCube(new Vector3(boxPos.x + boxCenter.x, boxPos.y + boxCenter.y, boxPos.z + boxCenter.z), boxScale);
        }

        Gizmos.color = Color.green;
        foreach (var obj in Objects) {
            BoxCollider collider = obj.GetComponent<BoxCollider>();
            // Vector3 boxScale = collider.size * collider.transform.parent.localScale.x;
            float boxScale = collider.transform.parent.localScale.x;
            Vector3 boxPos = collider.transform.position;
            Vector3 boxCenter = collider.center;

            Gizmos.DrawSphere(new Vector3(boxPos.x + boxCenter.x, boxPos.y + boxCenter.y + 1, boxPos.z + boxCenter.z), 0.3f);
            //Gizmos.DrawCube(new Vector3(boxPos.x + boxCenter.x, boxPos.y + boxCenter.y, boxPos.z + boxCenter.z), boxScale);
        }
    }

    private void Scan() {
        count = Physics.OverlapSphereNonAlloc(transform.position, distance, colliders, layers, QueryTriggerInteraction.Collide);
        Objects.Clear();
        for (int i = 0; i < count; i++) {
            GameObject obj = colliders[i].gameObject;
            if (IsInSight(obj) && IsVisible(obj)) {
                Objects.Add(obj);
            }
        }
    }

    public bool IsInSight(GameObject obj) {

        Vector3 origin = new Vector3(transform.position.x, transform.position.y - (height / 2), transform.position.z);
        Vector3 objPos = obj.GetComponent<BoxCollider>().transform.position;
        Vector3 dest = new Vector3(objPos.x, objPos.y, objPos.z);

        Vector3 direction = dest - origin;


        var dist = Vector3.Distance(transform.position, objPos);
        var frustumHeight = 2.0f * dist * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);

        if (direction.y < 0 || direction.y > height) {
            return false;
        }

        direction.y = 0;
        float deltaAngle = Vector3.Angle(direction, transform.forward);

        if (deltaAngle > angle) {
            return false;
        }

        origin.y += frustumHeight / 2;
        dest.y += frustumHeight / 2;


        if (Physics.Linecast(origin, dest, occlusionLayers)) {
            return false;
        }
        return true;
        
    }



    void DrawFrustum(Camera cam) {
        Vector3[] nearCorners = new Vector3[4]; //Approx'd nearplane corners
        Vector3[] farCorners = new Vector3[4]; //Approx'd farplane corners
        Plane[] camPlanes = GeometryUtility.CalculateFrustumPlanes(cam); //get planes from matrix
        Plane temp = camPlanes[1]; camPlanes[1] = camPlanes[2]; camPlanes[2] = temp; //swap [1] and [2] so the order is better for the loop

        for (int i = 0; i < 4; i++) {
            nearCorners[i] = Plane3Intersect(camPlanes[4], camPlanes[i], camPlanes[(i + 1) % 4]); //near corners on the created projection matrix
            farCorners[i] = Plane3Intersect(camPlanes[5], camPlanes[i], camPlanes[(i + 1) % 4]); //far corners on the created projection matrix
        }

        for (int i = 0; i < 4; i++) {
            Debug.DrawLine(nearCorners[i], nearCorners[(i + 1) % 4], Color.red, Time.deltaTime, true); //near corners on the created projection matrix
            Debug.DrawLine(farCorners[i], farCorners[(i + 1) % 4], Color.blue, Time.deltaTime, true); //far corners on the created projection matrix
            Debug.DrawLine(nearCorners[i], farCorners[i], Color.green, Time.deltaTime, true); //sides of the created projection matrix
        }
    }

    Vector3 Plane3Intersect(Plane p1, Plane p2, Plane p3) { //get the intersection point of 3 planes
        return ((-p1.distance * Vector3.Cross(p2.normal, p3.normal)) +
                (-p2.distance * Vector3.Cross(p3.normal, p1.normal)) +
                (-p3.distance * Vector3.Cross(p1.normal, p2.normal))) /
            (Vector3.Dot(p1.normal, Vector3.Cross(p2.normal, p3.normal)));
    }
}
