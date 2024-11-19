using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChangingMinigame : MonoBehaviour
{
    [SerializeField] public List<Color> colors = new List<Color>();
    [SerializeField] private GameObject cubePrefab; 
    [SerializeField] private int gridWidth = 5;     
    [SerializeField] private int gridHeight = 5;    
    [SerializeField] private float spacing = 1.5f;  // Spacing 
    [HideInInspector] public Color SelectedColor;
    private Color PostColor;

    public int randomColorInt;
    

    public int RoundsUntilNewColor = 2;
    private bool roundFinished = false;
    public int ColorsInPlay = 3;

    public int RoundCounter = 0;
    public float RoundLength = 1.1f;

    public float progressionSpeed;
    private int progressionInt = 4;

    public float TimerFirstHalf = 2;
    public float TimerSecoundHalf = 3;

    private bool timeToTrigger = false;
    public bool available = true;

    private List<GameObject> instantiatedCubes = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("A");

        progressionInt = ColorsInPlay + 1;

        for (int i = 0; i < colors.Count; i++)
        {
            Debug.Log("Color: " +colors[i]);
        }

        GenerateGrid();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (available)
        {
            StartCoroutine(ColorChanger());
        }
    }

    private void GenerateGrid()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                Vector3 position = new Vector3(x * spacing, 0, z * spacing);
                GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity, transform);
                instantiatedCubes.Add(cube);
                int randomIndex = Random.Range(0, ColorsInPlay);
                Color randomColor = colors[randomIndex];
                Renderer renderer = cube.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = randomColor;
                }
            }
        }
    }

    private IEnumerator ColorChanger()
    {
        available = false;
        RoundCounter++;
        if (RoundCounter == RoundsUntilNewColor && progressionInt < colors.Count)
        {
            progressionInt += 1;
            ColorsInPlay += 1;
            RoundCounter = 0;
        }
        
        
        randomColorInt = Random.Range(0, progressionInt);
        SelectedColor = colors[randomColorInt];

        yield return new WaitForSecondsRealtime(TimerFirstHalf);

        RemoveNonMatchingCubes();

        yield return new WaitForSecondsRealtime(TimerSecoundHalf);

        ResetCubes();

        available = true;
    }

    private void RemoveNonMatchingCubes()
    {
        foreach (GameObject cube in instantiatedCubes)
        {
            if (cube != null)
            {
                Renderer renderer = cube.GetComponent<Renderer>();
                if (renderer != null && renderer.material.color != SelectedColor)
                {
                    cube.SetActive(false); // Deactivate cube
                }
            }
        }
    }

    private void ResetCubes()
    {
        foreach (GameObject cube in instantiatedCubes)
        {
            if (cube != null)
            {
                // Reactivate the cube
                cube.SetActive(true);

                // Assign a new random color
                Renderer renderer = cube.GetComponent<Renderer>();
                if (renderer != null)
                {
                    int randomIndex = Random.Range(0, ColorsInPlay);
                    renderer.material.color = colors[randomIndex];
                }
            }
        }
    }

    // game starts

    //populate block with certain info, color, letters etc
    // > or have pre-exisiting blocks, prebuilt floor

    // triggered

    // color or trait is chosen

    // display chosen color or trait

    // timer counts down

    // disable all blocks not with trait

    // short timer

    // reset floor - re enable blocks

    // repeat


    //in case of color blindness, need icons to help differentate colors
    // > textures?
    // > limiting colors? if green no red? if blue no yllow?

}
