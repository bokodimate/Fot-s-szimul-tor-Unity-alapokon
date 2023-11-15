using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AnimalDetection : MonoBehaviour {
    public float distance = 10;
    public float angle = 30;
    public float height = 10.0f;
    public Color meshColor = Color.red;
    public int scanFrequency = 30;
    public LayerMask layers;
    public LayerMask occlusionLayers;
    public List<GameObject> Objects = new List<GameObject>();

    Collider[] colliders = new Collider[50];
    Mesh mesh;
    int count;
    float scanInterval;
    float scanTimer;
    void Start() {
        scanInterval = 1.0f / scanFrequency;
    }

    // Update is called once per frame
    void Update() {
        scanTimer -= Time.deltaTime;
        if (scanTimer < 0) {
            scanTimer += scanInterval;
            Scan();
        }
    }

    private void Scan() {
        count = Physics.OverlapSphereNonAlloc(transform.position, distance, colliders, layers, QueryTriggerInteraction.Collide);
        Objects.Clear();
        for (int i = 0; i < count; i++) {
            GameObject obj = colliders[i].gameObject;
            if (IsInSight(obj)) {
                Objects.Add(obj);
            }
        }
    }

    public bool IsInSight(GameObject obj) {

        Vector3 origin = new Vector3(transform.position.x, transform.position.y - (height / 2), transform.position.z);

        Vector3 boxPos = obj.GetComponent<BoxCollider>().transform.position;
        Vector3 boxCenter = obj.GetComponent<BoxCollider>().center;
        Vector3 boxScale = obj.GetComponent<BoxCollider>().size;
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

        origin.y += height / 2;
        dest.y += height / 2;


        if (Physics.Linecast(origin, dest, occlusionLayers)) {
            return false;
        }

        return true;
    }

    Mesh createWedgeMesh() {
        Mesh mesh = new Mesh();

        int segments = 10;
        int numTriangles = (segments * 4) + 2 + 2;
        int numVertices = numTriangles * 3;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
        Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

        Vector3 topCenter = bottomCenter + Vector3.up * height;
        Vector3 topLeft = bottomLeft + Vector3.up * height;
        Vector3 topRight = bottomRight + Vector3.up * height;

        int vert = 0;

        // left side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;

        // right side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;

        float currentAngle = -angle;
        float deltaAngle = (angle * 2) / segments;
        for (int i = 0; i < segments; i++) {
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * distance;

            topLeft = bottomLeft + Vector3.up * height;
            topRight = bottomRight + Vector3.up * height;

            // far side
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;

            // top
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;

            // bottom
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomLeft;

            currentAngle += deltaAngle;
        }



        for (int i = 0; i < numVertices; i++) {
            triangles[i] = i;
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;

    }

    private void OnValidate() {
        mesh = createWedgeMesh();
    }

    private void OnDrawGizmos() {

        if (mesh) {
            Gizmos.color = meshColor;

            Gizmos.DrawMesh(mesh, new Vector3(transform.position.x, transform.position.y - (height / 2), transform.position.z), transform.rotation);
        }
        Gizmos.DrawWireSphere(transform.position, distance);

        for (int i = 0; i < count; i++) {
            Vector3 boxPos = colliders[i].GetComponent<BoxCollider>().transform.position;
            Vector3 boxCenter = colliders[i].GetComponent<BoxCollider>().center;
            Vector3 boxScale = colliders[i].GetComponent<BoxCollider>().size;
            Gizmos.DrawCube(new Vector3(boxPos.x + boxCenter.x, boxPos.y + boxCenter.y, boxPos.z + boxCenter.z), boxScale);
        }

        Gizmos.color = Color.green;
        foreach (var obj in Objects) {
            Vector3 boxPos = obj.GetComponent<BoxCollider>().transform.position;
            Vector3 boxCenter = obj.GetComponent<BoxCollider>().center;
            Vector3 boxScale = obj.GetComponent<BoxCollider>().size;
            Gizmos.DrawCube(new Vector3(boxPos.x + boxCenter.x, boxPos.y + boxCenter.y, boxPos.z + boxCenter.z), boxScale);

        }
    }
}
