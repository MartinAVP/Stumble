using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CubeCalc2 : MonoBehaviour
{
    public Vector3[] Corners = new Vector3[8];

    [Header("Visuals")]
    [SerializeField] private Transform contact;
    [SerializeField] private Transform perpendicular;
    [SerializeField] private GameObject tagPrefab;

    private BoxCollider cubeCollider;
    private Vector3 colliderMultiplier;
    private Vector3 objectScale;
    public List<GameObject> visualizers;

    private void Start()
    {
        // Apply position to point object
        for (int i = 0; i < Corners.Length; i++)
        {
            GameObject visualizer = Instantiate(tagPrefab, Corners[i], Quaternion.identity);
            visualizer.name = "Corner " + i;
            visualizers.Add(visualizer);
        }
    }

    private void OnDisable()
    {
        visualizers.Clear();
        Corners = new Vector3[0];
    }

    private void Update()
    {
        cubeCollider = GetComponent<BoxCollider>();
        colliderMultiplier = cubeCollider.size;

        objectScale = this.transform.localScale;

        // Get the points without rotation
        Corners[0] = Vector3.Scale(new Vector3((objectScale.x / 2), (objectScale.y / 2), (objectScale.z / 2)), colliderMultiplier);
        Corners[1] = Vector3.Scale(new Vector3(-(objectScale.x / 2), (objectScale.y / 2), (objectScale.z / 2)), colliderMultiplier);
        Corners[2] = Vector3.Scale(new Vector3((objectScale.x / 2), (objectScale.y / 2), -(objectScale.z / 2)), colliderMultiplier);
        Corners[3] = Vector3.Scale(new Vector3(-(objectScale.x / 2), (objectScale.y / 2), -(objectScale.z / 2)), colliderMultiplier);

        Corners[4] = Vector3.Scale(new Vector3((objectScale.x / 2), -(objectScale.y / 2), (objectScale.z / 2)), colliderMultiplier);
        Corners[5] = Vector3.Scale(new Vector3(-(objectScale.x / 2), -(objectScale.y / 2), (objectScale.z / 2)), colliderMultiplier);
        Corners[6] = Vector3.Scale(new Vector3((objectScale.x / 2), -(objectScale.y / 2), -(objectScale.z / 2)), colliderMultiplier);
        Corners[7] = Vector3.Scale(new Vector3(-(objectScale.x / 2), -(objectScale.y / 2), -(objectScale.z / 2)), colliderMultiplier);

        // Add rotation and make point relative to object
        for (int i = 0; i < Corners.Length; i++)
        {
            Corners[i] = transform.position + (transform.rotation * Corners[i]);
        }

        for (int i = 0; (i < visualizers.Count); i++)
        {
            visualizers[i].transform.position = Corners[i];
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            RaycastHit hit;
            if (Physics.Raycast(this.transform.position, other.transform.position - this.transform.position, out hit, Mathf.Infinity))
            {
                Debug.DrawRay(this.transform.position, this.transform.TransformDirection(other.transform.position) * hit.distance, Color.yellow);
                Debug.Log(hit.point);
                contact.transform.position = hit.point;
            }

            Debug.Log("Player Collision");
            // Closest points 1 and 2
            //Debug.Log(-GetNormal(newPoint1, newPoint4, other.transform.position));
            //contact.transform.position = -GetNormal(newPoint1, newPoint4, other.transform.position);

            // Get the center position from 4 corners
            Vector3[] _tempSorted = SortVectorsByDistance(contact.transform.position);
            Vector3 centerSurfacePoint;
            // Check if its 45degrees
            centerSurfacePoint = (_tempSorted[0] + _tempSorted[1] + _tempSorted[2] + _tempSorted[3]) / 4;
            Debug.DrawLine(centerSurfacePoint, _tempSorted[0], Color.green, Mathf.Infinity);
            Debug.DrawLine(centerSurfacePoint, _tempSorted[1], Color.red, Mathf.Infinity);
            Debug.DrawLine(centerSurfacePoint, _tempSorted[2], Color.yellow, Mathf.Infinity);
            Debug.DrawLine(centerSurfacePoint, _tempSorted[3], Color.blue, Mathf.Infinity);
            contact.transform.position = centerSurfacePoint;
            contact.transform.rotation = this.transform.rotation;
            //perpendicular.transform.position = this.transform.position - GetNormal(_tempSorted[0], _tempSorted[1], centerSurfacePoint);
            // perpendicular a point from origin to center
            perpendicular.transform.rotation = this.transform.rotation;
            perpendicular.transform.position = (contact.position + (this.transform.position - GetNormal(_tempSorted[0], _tempSorted[1], centerSurfacePoint))).normalized * 2;
            Debug.DrawLine(this.transform.position, contact.transform.position * 5, Color.red, Mathf.Infinity);



/*            RaycastHit hit;
            if (Physics.Raycast(this.transform.position, other.transform.position - this.transform.position, out hit, Mathf.Infinity))
            {
                Debug.DrawRay(this.transform.position, this.transform.TransformDirection(other.transform.position) * hit.distance, Color.yellow);
                Debug.Log(hit.point);
                contact.transform.position = hit.point;
                //perpendicular.transform.position = -GetNormal(newPoint1, newPoint2, hit.point);
            }*/

            //other.transform.position;

            // Know where the player hit
            // Draw a line between player and center
            // Find the surface.
        }
    }

    Vector3 GetNormal(Vector3 a, Vector3 b, Vector3 o)
    {
        // Find vectors corresponding to two of the sides of the triangle.
        Vector3 side1 = a - o;
        Vector3 side2 = b - o;

        // Cross the vectors to get a perpendicular vector, then normalize it. This is the Result vector in the drawing above.
        return Vector3.Cross(side1, side2).normalized;
    }

    public Vector3[] SortVectorsByDistance(Vector3 referencePoint)
    {
        // Create a list of vectors with their corresponding distances
        var sortedVectors = Corners
            .Select(v => new
            {
                Vector = v,
                Distance = Vector3.Distance(v, referencePoint)
            })
            .OrderBy(vd => vd.Distance) // Sort by distance in ascending order
            .Select(vd => vd.Vector)    // Select the sorted vectors
            .ToArray();                 // Convert to array

        return sortedVectors;
    }
}
