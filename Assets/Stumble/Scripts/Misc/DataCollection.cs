using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DataCollection : MonoBehaviour
{
    private string formUrl = "https://docs.google.com/forms/u/0/d/e/1FAIpQLSebPamHxetQ_hxldnHQ9bTiAEVFFvFaUeuzYhnUywtK78o4uw/formResponse";

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(Post());
    }

    private IEnumerator Post()
    {
        WWWForm form = new WWWForm();
        form.AddField("entry.2012433436", "Howdy");
        form.AddField("entry.1663850345", "Howdy Cowboy");

        using (UnityWebRequest www = UnityWebRequest.Post(formUrl, form))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Feedback sent succesfully");
            }
            else
            {
                Debug.Log("Error in feedback submission " + www.error);
            }
        }
    }
}
