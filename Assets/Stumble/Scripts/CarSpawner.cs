using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public GameObject CarPrefab;

    public bool Triggered = false;

    //delay before next flip
    private float delayTime = 1f;
    private float delayCounter = 0f;
    private bool delayed = false;

    public GameObject LaneOne;
    public GameObject LaneTwo;
    public GameObject LaneThree;
    public GameObject LaneFour;

    public List<GameObject> CarLanes = new List<GameObject>();

    public int ActiveLanes = 2;

    private int CarSpawnerCounter = 0;

    private int CarsSpawned = 0;


    void Start()
    {
        Debug.Log(CarLanes[1].transform.position);
    }


    void FixedUpdate()
    {
        if (Triggered == true)
        {
            while (CarSpawnerCounter < ActiveLanes)
            {
                CarsSpawned++;
                int S = Random.Range(0, 3);
                Debug.Log("Going to instantiate " + CarPrefab + " at " + CarLanes[S].transform.position + " with " + Quaternion.identity + " rotation");
                Instantiate(CarPrefab, CarLanes[S].transform.position, Quaternion.identity);
                Debug.Log("Instantiated " + CarPrefab + " at " + CarLanes[S].transform.position + " with " + Quaternion.identity + " rotation");


                Debug.Log("Car Count: " + CarsSpawned);
                CarSpawnerCounter++;

            }
        }
    }



    //spawner

    //1  trigger activates spawner

    //2  spawner rolls for lanes

    //3  initialize cars in lanes
    //   -cars need to be orientent : towards the bottom?

    //4  delay

    //5  repeat steps 2 to 4

    //6 same trigger or end trigger deactivates spawner


    //cars 

    //1 get initialized

    //2 roll down
    //  -Not using ridged bodies, need to work on all objects
    //    transform edits

    //3 push players
    //  -bumper?

    //4 roll off?



    //5 despawn upon trigger
    //  -Trigger timer? despawns at a certain time 
    //    better than killbox level devs won't need to setup a killzone
    //  
    //  -when despawning fade for "emersion" 






}
