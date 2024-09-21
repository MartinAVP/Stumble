using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CubeCalc : MonoBehaviour
{
    private Vector3 point1;
    private Vector3 point2;
    private Vector3 point3;
    private Vector3 point4;

    private Vector3 point5;
    private Vector3 point6;
    private Vector3 point7;
    private Vector3 point8;
    private Vector3 colliderSize;

    [SerializeField] private Transform point1GameObject;
    [SerializeField] private Transform point2GameObject;
    [SerializeField] private Transform point3GameObject;
    [SerializeField] private Transform point4GameObject;


    [SerializeField] private Transform point5GameObject;
    [SerializeField] private Transform point6GameObject;
    [SerializeField] private Transform point7GameObject;
    [SerializeField] private Transform point8GameObject;


    [SerializeField] private Transform contact;
    [SerializeField] private Transform perpendicular;

    private BoxCollider cubeCollider;
    private Vector3 colliderMultiplier;
    private Quaternion startRotation;

    Vector3 newPoint1;
    Vector3 newPoint2;
    Vector3 newPoint3;
    Vector3 newPoint4;
    Vector3 newPoint5;
    Vector3 newPoint6;
    Vector3 newPoint7;
    Vector3 newPoint8;

    // Widht / Height / Length
    //   X   /  Y     /   Z
    // Note: The Box Collider will take priority for the top most Box Collider Component in the Object
    private void Awake()
    {
        /*        startRotation = transform.rotation;
                transform.rotation = Quaternion.Euler(Vector3.zero);*/

    }

    private void Update()
    {
        cubeCollider = GetComponent<BoxCollider>();
        colliderMultiplier = cubeCollider.size;

        colliderSize = this.transform.localScale;
        // Get the points without rotation
        point1 = Vector3.Scale(new Vector3((colliderSize.x / 2), (colliderSize.y / 2), (colliderSize.z / 2)), colliderMultiplier);
        point2 = Vector3.Scale(new Vector3(-(colliderSize.x / 2), (colliderSize.y / 2), (colliderSize.z / 2)), colliderMultiplier);
        point3 = Vector3.Scale(new Vector3((colliderSize.x / 2), (colliderSize.y / 2), -(colliderSize.z / 2)), colliderMultiplier);
        point4 = Vector3.Scale(new Vector3(-(colliderSize.x / 2), (colliderSize.y / 2), -(colliderSize.z / 2)), colliderMultiplier);

        point5 = Vector3.Scale(new Vector3((colliderSize.x / 2), -(colliderSize.y / 2), (colliderSize.z / 2)), colliderMultiplier);
        point6 = Vector3.Scale(new Vector3(-(colliderSize.x / 2), -(colliderSize.y / 2), (colliderSize.z / 2)), colliderMultiplier);
        point7 = Vector3.Scale(new Vector3((colliderSize.x / 2), -(colliderSize.y / 2), -(colliderSize.z / 2)), colliderMultiplier);
        point8 = Vector3.Scale(new Vector3(-(colliderSize.x / 2), -(colliderSize.y / 2), -(colliderSize.z / 2)), colliderMultiplier);

        // Add rotation and make point relative to object
        newPoint1 = transform.position + (transform.rotation * point1);
        newPoint2 = transform.position + (transform.rotation * point2);
        newPoint3 = transform.position + (transform.rotation * point3);
        newPoint4 = transform.position + (transform.rotation * point4);

        newPoint5 = transform.position + (transform.rotation * point5);
        newPoint6 = transform.position + (transform.rotation * point6);
        newPoint7 = transform.position + (transform.rotation * point7);
        newPoint8 = transform.position + (transform.rotation * point8);

        // Apply position to point object
        point1GameObject.transform.position = newPoint1;
        point2GameObject.transform.position = newPoint2;
        point3GameObject.transform.position = newPoint3;
        point4GameObject.transform.position = newPoint4;

        point5GameObject.transform.position = newPoint5;
        point6GameObject.transform.position = newPoint6;
        point7GameObject.transform.position = newPoint7;
        point8GameObject.transform.position = newPoint8;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            Debug.Log("Player Collision");
            // Closest points 1 and 2
            //Debug.Log(-GetNormal(newPoint1, newPoint4, other.transform.position));
            //contact.transform.position = -GetNormal(newPoint1, newPoint4, other.transform.position);

            // Get the center position from 4 corners
            Vector3 centerSurfacePoint = (newPoint1 + newPoint2 + newPoint3 + newPoint4) / 4;
            contact.transform.position = centerSurfacePoint;
            contact.transform.rotation = this.transform.rotation;
            perpendicular.transform.position = this.transform.position - GetNormal(newPoint1, newPoint4, centerSurfacePoint);
            // perpendicular a point from origin to center
            perpendicular.transform.rotation = this.transform.rotation;
            perpendicular.transform.position = (contact.position + (this.transform.position - GetNormal(newPoint1, newPoint4, centerSurfacePoint))).normalized;
            Debug.DrawLine(this.transform.position, contact.transform.position * 5, Color.red, Mathf.Infinity);



            RaycastHit hit;
            if (Physics.Raycast(this.transform.position, other.transform.position - this.transform.position ,out hit, Mathf.Infinity)){
                Debug.DrawRay(this.transform.position, this.transform.TransformDirection(other.transform.position) * hit.distance, Color.yellow);
                Debug.Log(hit.point);
                //contact.transform.position = hit.point;
                //perpendicular.transform.position = -GetNormal(newPoint1, newPoint2, hit.point);
            }

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
}
