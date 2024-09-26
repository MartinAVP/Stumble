using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public GameObject CarPrefab;
    public List<GameObject> CarLanes = new List<GameObject>();

    public int ActiveLanes;

    public bool Triggered = false;

    public float MinTimeBetweenCars = 1f;
    public float MaxTimeBetweenCars = 2f;
    
    public bool WavesMode = false;
    public float FlatWaveDelay = 4f;

    private bool spawning = false;
    private bool delayed = false;

    public bool RandomTimingWaves = false;
    public float MinTimeBetweenWaves = 5f;
    public float MaxTimeBetweenWaves = 10f;

    private int CarSpawnerCounter = 0;

    private int CarsSpawned = 0;
    private int S = 0;

    private int Prev_S;


    void FixedUpdate()
    {
        if (Triggered == true)
        {
            // accounts for the lack of first case (int for Prev_S) for the beginning of the spawn cycles
            if (CarsSpawned == 0)
            {
                S = Random.Range(0, 3);
                //Debug.Log("Random Lane: " + S);
                Prev_S = S;

                Instantiate(CarPrefab, CarLanes[S].transform.position, Quaternion.identity);
                CarsSpawned++;

                CarSpawnerCounter++;
                //Debug.Log("Car Count (Starter): " + CarsSpawned);

                StartCoroutine(Delay(true, true));

                //Debug.Log("start finished");
            }
            if (spawning)
            {
                //Debug.Log("b");
                spawning = false;
                StartCoroutine(Delay(true, false));
            }
        }
    }


    //delay between "waves"
    private IEnumerator Delay(bool car, bool wave) 
    {
        if (car)
        {
            while (CarSpawnerCounter < ActiveLanes)
            {
                float CT = Random.Range(MinTimeBetweenCars, MaxTimeBetweenCars);
                //Debug.Log(CT);
                yield return new WaitForSecondsRealtime(CT);
                //Debug.Log("car timer done");

                int Prev_Prev_S = Prev_S;
                Prev_S = S;

                if (ActiveLanes >= 3)
                {
                    while (S == Prev_S || S == Prev_Prev_S)
                    {
                        S = Random.Range(0, 3);
                    }
                }
                else
                {
                    while (S == Prev_S)
                    {
                        S = Random.Range(0, 3);
                    }
                }
                
                Instantiate(CarPrefab, CarLanes[S].transform.position, Quaternion.identity);
                //Debug.Log("Instantiated " + CarPrefab + " at " + CarLanes[S].transform.position + " with " + Quaternion.identity + " rotation");
                CarsSpawned++;
                CarSpawnerCounter++;

                Debug.Log("Car Count: " + CarsSpawned);
                if (CarSpawnerCounter == ActiveLanes)
                {
                    //Debug.Log("wave complete");
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
