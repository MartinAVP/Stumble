using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuPlayerSelect : MonoBehaviour
{

    public int SelectedCharNum = 1;

    [SerializeField] private TMP_Text SelectedCharText;
    [SerializeField] private TMP_Text NextCharText;
    [SerializeField] private TMP_Text PastCharText;
    [SerializeField] private int NextCharNum = 2;
    [SerializeField] private int PastCharNum = 0;

    // Start is called before the first frame update
    void Awake()
    {
        PastCharText.text = " ";
        SelectedCharText.text = SelectedCharNum.ToString();
        NextCharText.text = NextCharNum.ToString();
    }

    public void DecreasePlayerCount()
    {
        if (SelectedCharNum > 1)
        {
            SelectedCharNum--;
            NextCharNum--;
            PastCharNum--;

            PastCharText.text = PastCharNum.ToString();
            SelectedCharText.text = SelectedCharNum.ToString();
            NextCharText.text = NextCharNum.ToString();

            if (PastCharNum == 0)
            {
                PastCharText.text = " ";
            }
        }
    }

    public void IncreasePlayerCount()
    {
        {
            if (SelectedCharNum < 4)
            {
                SelectedCharNum++;
                NextCharNum++;
                PastCharNum++;

                PastCharText.text = PastCharNum.ToString();
                SelectedCharText.text = SelectedCharNum.ToString();
                NextCharText.text = NextCharNum.ToString();

                if (NextCharNum == 5)
                {
                    NextCharText.text = " ";
                }
            }
        }
    }
}
