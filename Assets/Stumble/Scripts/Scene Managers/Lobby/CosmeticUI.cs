using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CosmeticUI : MonoBehaviour
{
    [SerializeField] private VisualCosmetic[] visuals = new VisualCosmetic[4];

    [SerializeField] Sprite test;

    public RectTransform container; // The parent container with a mask
    public float transitionDuration = .3f; // Speed of the interpolation
    public Image currentImage; // Reference to the current displayed image

    // Solve
    public static CosmeticUI Instance;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    /*    private bool isMoving = false;*/

    private void Start()
    {
        transitionDuration = CosmeticManager.Instance.inputDelay / 1.2f;

        for (int i = 0; i < visuals.Length; i++)
        {
            var visual = visuals[i];
            visual.hatArrows.gameObject.SetActive(false);
            visual.colorArrows.gameObject.SetActive(false);
        }
    }

    public void SetDefaultImage(Sprite sprite, int playerID, int cosmeticCategoryID)
    {
        switch (cosmeticCategoryID)
        {
            case 0:
                visuals[playerID].currentHatImage.color = Color.white;
                visuals[playerID].currentHatImage.sprite = sprite;
                break;
            case 1:
                visuals[playerID].currentColorImage.color = Color.white;
                visuals[playerID].currentColorImage.sprite = sprite;
                break;
            case 2:
                visuals[playerID].currentBootsImage.color = Color.white;
                visuals[playerID].currentBootsImage.sprite = sprite;
                break;
            default:
                Debug.LogError(" Not Category with that ID!");
                break;
        }
    }

    // Call this method to change the image
    private int index = 0;
    public void ChangeImage(Direction direction, Sprite newSprite, int playerID, int cosmeticCategoryID)
    {
        /*        if (isMoving) { return; }
                isMoving = true;*/
        Debug.Log("Called" + index++);
        CosmeticManager.SelectedCosmetic cosmetic = CosmeticManager.SelectedCosmetic.Colors;
        switch (cosmeticCategoryID)
        {
            case 0:
                container = visuals[playerID].hatContainer;
                currentImage = visuals[playerID].currentHatImage;
                cosmetic = CosmeticManager.SelectedCosmetic.Hats;
                break;
            case 1:
                container = visuals[playerID].colorContainer;
                currentImage = visuals[playerID].currentColorImage;
                cosmetic = CosmeticManager.SelectedCosmetic.Colors;
                break;
/*            case 2:
                container = visuals[playerID].bootsContainer;
                currentImage = visuals[playerID].currentBootsImage;
                cosmetic = CosmeticManager.SelectedCosmetic.Boots;
                break;*/
            default:
                Debug.LogError(" Not Category with that ID!");
                break;
        }

        if (currentImage == null)
        {
            // Initialize currentImage if it's null
/*            currentImage = Instantiate(new GameObject("CurrentImage"), container).AddComponent<Image>();
            currentImage.sprite = newSprite;
            currentImage.rectTransform.localPosition = Vector3.zero; // Center it in the container*/
        }
        else
        {
            // Create a new image for the transition
            Debug.Log("Calling Changing");
            Image newImage = new GameObject("New Image").AddComponent<Image>();
            //Image newImage = currentImage;
            //Debug.Log($"Instantiated new image: {newImage.name} at position: {newImage.transform.position}");
            newImage.transform.SetParent(container, false);

            newImage.sprite = newSprite;
            newImage.GetComponent<RectTransform>().sizeDelta = currentImage.GetComponent<RectTransform>().sizeDelta;

            // Set initial position based on direction
            Vector3 incomingStartPosition;
            Vector3 outgoingTargetPosition;

            if (direction == Direction.Right)
            {
                incomingStartPosition = new Vector3(-container.rect.width, 0, 0); // Start off-screen left
                outgoingTargetPosition = new Vector3(container.rect.width, 0, 0); // Move off-screen right
            }
            else // Direction.Left
            {
                incomingStartPosition = new Vector3(container.rect.width, 0, 0); // Start off-screen right
                outgoingTargetPosition = new Vector3(-container.rect.width, 0, 0); // Move off-screen left
            }

            newImage.rectTransform.localPosition = incomingStartPosition;

            // Start the coroutine for transition
            StartCoroutine(TransitionImages(outgoingTargetPosition, newImage, playerID, cosmetic));
        }
    }

    private IEnumerator TransitionImages(Vector3 outgoingTargetPosition, Image incomingImage, int playerID, CosmeticManager.SelectedCosmetic cosmetic)
    {
        Image outgoingImage = currentImage;

        float elapsedTime = 0f;

        // Move both images over the same duration
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;

            // Normalize time
            float t = Mathf.Clamp01(elapsedTime / transitionDuration);
            float outgoingSpeedFactor = 3f;
            float outgoingT = Mathf.Clamp01(t * outgoingSpeedFactor);

            // Calculate positions for both images
            outgoingImage.rectTransform.localPosition = Vector3.Lerp(Vector3.zero, outgoingTargetPosition, outgoingT);
            incomingImage.rectTransform.localPosition = Vector3.Lerp(incomingImage.rectTransform.localPosition, Vector3.zero, t);

            yield return null; // Wait for the next frame
        }

        // Finalize positions
        outgoingImage.rectTransform.localPosition = outgoingTargetPosition; // Ensure outgoing image is positioned correctly
        incomingImage.rectTransform.localPosition = Vector3.zero; // Ensure incoming image is centered

        currentImage = incomingImage; // Update the current image

        switch (cosmetic)
        {
            case CosmeticManager.SelectedCosmetic.Hats:
                visuals[playerID].currentHatImage = currentImage;
                break;
            case CosmeticManager.SelectedCosmetic.Colors:
                visuals[playerID].currentColorImage = currentImage;
                break;
/*            case CosmeticManager.SelectedCosmetic.Boots:
                visuals[playerID].currentBootsImage = currentImage;
                break;*/
        }

        Destroy(outgoingImage.gameObject); // Clean up the outgoing image
    }

    public void SetArrowsVisible(int playerID, CosmeticManager.SelectedCosmetic selectedCosmetic)
    {
        if (selectedCosmetic == CosmeticManager.SelectedCosmetic.Hats)
        {
            visuals[playerID].hatArrows.gameObject.SetActive(true);
            visuals[playerID].colorArrows.gameObject.SetActive(false);
        }
        else if(selectedCosmetic == CosmeticManager.SelectedCosmetic.Colors)
        {
            visuals[playerID].colorArrows.gameObject.SetActive(true);
            visuals[playerID].hatArrows.gameObject.SetActive(false);
        }
    }


    public enum Direction
    {
        Left,
        Right
    }

/*    void OnGUI()
    {
        // Create the first button
        if (GUI.Button(new Rect(10, 10, 150, 50), "Testing 1"))
        {
            //MoveImage(test, Direction.Left);
            //ChangeImage(Direction.Left, test);
        }

        // Create the second button
        if (GUI.Button(new Rect(10, 70, 150, 50), "Testing 2"))
        {
            //MoveImage(test, Direction.Right);
            //ChangeImage(Direction.Right, test);
        }
    }*/
}
