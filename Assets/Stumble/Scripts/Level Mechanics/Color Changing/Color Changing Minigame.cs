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

    public int randomColorInt;

    public int RoundsUntilNewColor = 2;
    //
    public int ColorsInPlay = 3;

    public int RoundCounter = 0;
    public float frameDelay = 1.1f;

    public float progressionSpeed;
    private int progressionInt = 4;

    public float TimeUntilAnnouncerDisapears = 2;
    public float TimeUntilFloorDisapears = 3;

    private bool timeToTrigger = false;
    
    
    
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
    void Update()
    {
        
    }

    private void GenerateGrid()
    {
        // Ensure cubePrefab is assigned
        if (cubePrefab == null)
        {
            Debug.LogError("Cube Prefab is not assigned!");
            return;
        }


        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                Vector3 position = new Vector3(x * spacing, 0, z * spacing);
                GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity, transform);
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
        RoundCounter++;
        if (RoundCounter == RoundsUntilNewColor)
        {
            progressionInt += 1;
            ColorsInPlay += 1;
            RoundCounter = 0;
        }
        
        randomColorInt = Random.Range(0, progressionInt);
        SelectedColor = colors[randomColorInt];

        yield return new WaitForSecondsRealtime(frameDelay);
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
