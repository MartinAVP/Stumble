using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIComponent : MonoBehaviour
{
/*    public Transform Character;
    public TextMeshProUGUI ScoreText;

    private PlayerDataHolder playerDataHolder;

    private void Start()
    {

        Character.gameObject.SetActive(false);
        ScoreText.gameObject.SetActive(false);
    }

    public void DisplayScores()
    {
        this.GetComponent<UICameraView>().EnableCharacterCam();

        playerDataHolder = PlayerDataHolder.Instance;
        int playerCount = playerDataHolder.GetPlayers().Count;
        int id = playerDataHolder.GetPlayerData(this.GetComponent<PlayerInput>()).id;

        switch (id)
        {
            case 0:
                ChangeUIAnchor(CharacterAnchor.BottomLeft); 
                break;
            case 1:
                ChangeUIAnchor(CharacterAnchor.BottomRight);
                break;
            case 2:
                ChangeUIAnchor(CharacterAnchor.BottomLeft);
                break;
            case 3:
                ChangeUIAnchor(CharacterAnchor.BottomRight);
                break;
        }

        // Enable the Camera

        Character.gameObject.SetActive(true);
        ScoreText.gameObject.SetActive(true);

    }

    private void ChangeUIAnchor(CharacterAnchor anchor)
    {
        switch(anchor){
            case CharacterAnchor.BottomLeft:
                // Set Anchor to Bottom Left
                Character.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
                Character.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
                Character.transform.localScale = new Vector3(-1,1,1);       // Invert Image
                //Character.transform.position = Vector3.zero;
                //Character.GetComponent<RectTransform>().position = new Vector3(Character.GetComponent<RectTransform>().rect.width / 2, Character.GetComponent<RectTransform>().rect.height / 2, 0);
                Character.GetComponent<RectTransform>().anchoredPosition = new Vector3(Character.GetComponent<RectTransform>().rect.width / 2, Character.GetComponent<RectTransform>().rect.height / 2, 0);

                ScoreText.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0);
                ScoreText.GetComponent<RectTransform>().anchorMin = new Vector2(1, 1);
                break;
            case CharacterAnchor.BottomRight:
                Character.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0);
                Character.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0);
                Character.transform.localScale = Vector3.one;
                //Character.transform.position = Vector3.zero;
                Character.GetComponent<RectTransform>().anchoredPosition = new Vector3(-Character.GetComponent<RectTransform>().rect.width / 2, Character.GetComponent<RectTransform>().rect.height / 2, 0);
                //Character.transform.position = new Vector3(-Character.GetComponent<RectTransform>().rect.width / 2, Character.GetComponent<RectTransform>().rect.height / 2, 0);
                //Character.transform.position = new Vector3(-Character.transform.position.x, Character.transform.position.y, Character.transform.position.z);

                ScoreText.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
                ScoreText.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
                break;
        }
    }


    public void UpdateScore(int newScore, float duration)
    {
        StartCoroutine(AnimateScore(newScore, duration));
    }

    private IEnumerator AnimateScore(int newScore, float duration)
    {
        int currentScore = int.Parse(ScoreText.text);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Interpolate between currentScore and newScore
            int interpolatedScore = (int)Mathf.Lerp(currentScore, newScore, t);
            ScoreText.text = interpolatedScore.ToString();

            yield return null; // Wait for the next frame
        }

        // Ensure the final score is set
        ScoreText.text = newScore.ToString();
    }


    // Anchors
    public enum CharacterAnchor
    {
        BottomLeft,
        BottomRight,
    }*/
}
