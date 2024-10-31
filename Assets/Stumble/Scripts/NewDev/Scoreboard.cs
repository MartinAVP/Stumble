using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour
{
    public GameObject panel;
    public RawImage characterImage;
    public RawImage smallIcon;
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI positionText;
    public TextMeshProUGUI pointsAdded;
    public Animator pointAnim;
    private int points = 0;
    public PlayerData playerData { set; get; }

    private void Start()
    {
        //pointAnim = GetComponent<Animator>();
        pointsAdded.gameObject.SetActive(false);
    }

    public void UpdateValues(int points, int position)
    {
        pointsText.text = points.ToString() + " pts";
        positionText.text = "#" + position.ToString();
        this.points = points;
    }

    public void InvertPanel()
    {
        panel.transform.localScale = new Vector3(panel.transform.localScale.x * -1, 1, 1);

        smallIcon.transform.localScale = new Vector3(smallIcon.transform.localScale.x * -1, 1, 1);

        positionText.transform.localScale = new Vector3(positionText.transform.localScale.x * -1, 1, 1);
        pointsText.transform.localScale = new Vector3(pointsText.transform.localScale.x * -1, 1, 1);
    }


    public void UpdateScore(int newScore, float duration)
    {
        pointsAdded.gameObject.SetActive(true);

        pointsAdded.text = "+ " + Mathf.Abs(points - newScore).ToString();
        pointAnim.SetTrigger("PlayPoint");

        StartCoroutine(AnimateEnd());
        StartCoroutine(AnimateScore(newScore, duration));
    }

    private IEnumerator AnimateEnd()
    {
        yield return new WaitForSeconds(.99f);
        pointsAdded.gameObject.SetActive(false);
    }

    private IEnumerator AnimateScore(int newScore, float duration)
    {
        int currentScore = points;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Interpolate between currentScore and newScore
            int interpolatedScore = (int)Mathf.Lerp(currentScore, newScore, t);
            pointsText.text = interpolatedScore.ToString() + " pts";

            yield return null; // Wait for the next frame
        }

        // Ensure the final score is set
        pointsText.text = newScore.ToString() + " pts";
    }
}
