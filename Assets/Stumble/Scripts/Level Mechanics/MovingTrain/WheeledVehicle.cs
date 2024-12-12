using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheeledVehicle : MonoBehaviour
{
    [SerializeField] private GameObject vehicle;
    [SerializeField] private List<GameObject> wheels;
    [SerializeField] private float speed = 5;
    [SerializeField] private float delayTime = 1;
    [SerializeField] public RotationAxis wheelRotationAxis;
    [SerializeField] public bool faceMovementDirection;
    [SerializeField] public List<Transform> patrolPoints;

    private PatrolMovement patrolMovement;
    private List<RotationMovement> rotationComponents = new List<RotationMovement>();
    private Vector3 previousPosition;
    private Vector3 vehicleForward;

    private void Start()
    {
        patrolMovement = vehicle.AddComponent<PatrolMovement>();
        patrolMovement.speed = speed;
        patrolMovement.delayTime = delayTime;
        patrolMovement.faceTowardNextNode = faceMovementDirection;

        patrolMovement.startMoving.AddListener(StartWheels);
        patrolMovement.stopMoving.AddListener(StopWheels);

        for (int i = 0; i < patrolPoints.Count; i++)
        {
            patrolMovement.PatrolNodes.Add(patrolPoints[i]);
        }

        for (int i = 0; i < wheels.Count; i++)
        {
            var wheel = wheels[i];
            var rotationMovement = wheel.AddComponent<RotationMovement>();
            rotationMovement.rotateAroundAxis = wheelRotationAxis;
            rotationMovement.rotationSpeed = speed * 20;

            rotationComponents.Add(rotationMovement);
        }
    }

    private void Update()
    {
        Vector3 velocity = vehicle.transform.position - previousPosition;
        float dot = Vector3.Dot(vehicle.transform.forward, velocity.normalized);
        //print(dot);

        for (int i = 0; i < rotationComponents.Count; i++)
        {
            rotationComponents[i].rotationSpeed = speed * 20 * dot;  
        }

        previousPosition = vehicle.transform.position;
    }

    private void StopWheels()
    {
        //print("stop rotating");
        for (int i = 0; i < rotationComponents.Count; i++)
        {
            RotationMovement rotationMovement = rotationComponents[i];
            rotationMovement.StopRotating();
        }
    }

    private void StartWheels()
    {
        //print("start rotating");
        for (int i = 0; i < rotationComponents.Count; i++)
        {
            RotationMovement rotationMovement = rotationComponents[i];
            rotationMovement.StartRotating(false);
        }
    }
}
