using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public GameObject CarPrefab;
    public List<GameObject> CarLanes = new List<GameObject>();
    public List<int> AvailableLaneStorage = new List<int> { 0, 1, 2, 3 };

    public int ActiveLanes;

    public bool Triggered = false;

    public float MinTimeBetweenCars = 1f;
    public float MaxTimeBetweenCars = 2f;
    
    public bool WavesMode = false;
    public float FlatWaveDelay = 4f;

    private bool spawning = true;

    public bool RandomTimingWaves = false;
    public float MinTimeBetweenWaves = 5f;
    public float MaxTimeBetweenWaves = 10f;

    private int CarSpawnerCounter = 0;

    private int CarsSpawned = 0;

    void FixedUpdate()
    {
        if (Triggered == true)
        {
            //Debug.Log("a");
            if (spawning)
            {
                //Debug.Log("b");
                spawning = false;
                StartCoroutine(SpawnCars(true, false));
            }
        }
    }


    //delay between "waves"
    private IEnumerator SpawnCars(bool car, bool wave) 
    {
        //Debug.Log("c");
        if (car)
        {
            //Debug.Log("d");
            while (CarSpawnerCounter < ActiveLanes)
            {
                //Debug.Log("e");
                if (AvailableLaneStorage.Count > 0)
                {
                    //Debug.Log("f");
                    int selectedLane = Random.Range(0, AvailableLaneStorage.Count);
                    //Debug.Log(selectedLane);
                    int NewLane = AvailableLaneStorage[selectedLane];
                    //Debug.Log("g");
                    Instantiate(CarPrefab, CarLanes[NewLane].transform.position, Quaternion.identity);
                    CarsSpawned++;
                    CarSpawnerCounter++;

                    
                    if (AvailableLaneStorage.Count > 0)
                    {
                        AvailableLaneStorage.RemoveAt(selectedLane);
                    }

                    if (AvailableLaneStorage.Count <= 0 || CarSpawnerCounter >= ActiveLanes)
                    {
                        AvailableLaneStorage = new List<int> { 0, 1, 2, 3 };
                    }

                    //Debug.Log("h");
                    float CT = Random.Range(MinTimeBetweenCars, MaxTimeBetweenCars);
                    //Debug.Log(CT);
                    yield return new WaitForSecondsRealtime(CT);
                }
            }
        }

        if (WavesMode) //if (wave && WavesMode)
        {
            //randomizes the timer after the first spawn wave
            if (RandomTimingWaves == true)
            {
                float WT = Random.Range(MinTimeBetweenWaves, MaxTimeBetweenWaves);
                //Debug.Log(WT);
                yield return new WaitForSecondsRealtime(WT);
                spawning = true;
                CarSpawnerCounter = 0;

            }
            else
            {
                //Debug.Log("c");
                yield return new WaitForSecondsRealtime(FlatWaveDelay); 
                spawning = true;
                CarSpawnerCounter = 0;
            }
        }

        if (WavesMode != true) 
        {
            spawning = true;
            CarSpawnerCounter = 0;
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
