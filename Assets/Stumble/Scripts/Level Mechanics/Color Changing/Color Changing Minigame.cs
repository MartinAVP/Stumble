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
    public bool noImmediateRepeats = false;
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

        progressionInt = Mathf.Min(ColorsInPlay, colors.Count);

        for (int i = 0; i < colors.Count; i++)
        {
            Debug.Log("Color: " + colors[i]);
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
        List<Color> colorPool = GenerateColorPool(gridWidth * gridHeight, progressionInt);
        ShuffleList(colorPool);

        int index = 0;
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                Vector3 position = new Vector3(x * spacing, 0, z * spacing);
                GameObject cube = Instantiate(cubePrefab, position, Quaternion.identity, transform);
                instantiatedCubes.Add(cube);

                Renderer renderer = cube.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = colorPool[index];
                    index++;
                }
            }
        }
    }

    private IEnumerator ColorChanger()
    {
        available = false;
        RoundCounter++;
        if (RoundCounter == RoundsUntilNewColor)
        {
            progressionInt = Mathf.Min(progressionInt + 1, colors.Count);
            ColorsInPlay = Mathf.Min(ColorsInPlay + 1, colors.Count);
            RoundCounter = 0;
        }

        List<Color> colorPool = GenerateColorPool(instantiatedCubes.Count, ColorsInPlay);
        ShuffleList(colorPool);

        PostColor = SelectedColor;

        if (noImmediateRepeats)
        {
            while (SelectedColor == PostColor)
            {
                SelectedColor = colorPool[Random.Range(0, colorPool.Count)];
            }
        }
        else
        {
            SelectedColor = colorPool[Random.Range(0, colorPool.Count)];
        }

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
        List<Color> colorPool = GenerateColorPool(instantiatedCubes.Count, ColorsInPlay);
        ShuffleList(colorPool);

        int index = 0;
        foreach (GameObject cube in instantiatedCubes)
        {
            if (cube != null)
            {
                cube.SetActive(true);
                Renderer renderer = cube.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = colorPool[index];
                    index++;
                }
            }
        }
    }

    private List<Color> GenerateColorPool(int totalCubes, int colorsInPlay)
    {
        List<Color> colorPool = new List<Color>();
        int actualColorsInPlay = Mathf.Min(colorsInPlay, colors.Count);

        int cubesPerColor = totalCubes / actualColorsInPlay;
        int remainder = totalCubes % actualColorsInPlay;

        for (int i = 0; i < actualColorsInPlay; i++)
        {
            for (int j = 0; j < cubesPerColor; j++)
            {
                colorPool.Add(colors[i]);
            }
        }

        for (int i = 0; i < remainder; i++)
        {
            colorPool.Add(colors[Random.Range(0, actualColorsInPlay)]);
        }

        while (colorPool.Count < totalCubes)
        {
            colorPool.Add(colors[Random.Range(0, actualColorsInPlay)]);
        }

        return colorPool;
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
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
