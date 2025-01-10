using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject winScreen;

    [SerializeField] private TMP_Text playerOneLapText;
    [SerializeField] private TMP_Text playerTwoLapText;
    [SerializeField] private TMP_Text winText;

    [SerializeField] private List<TMP_Text> maxLapsText = new List<TMP_Text>();
    [SerializeField] private int lapCount = 3;

    private void Start()
    {
        foreach(TMP_Text txt in maxLapsText)
        {
            txt.text = " / " + lapCount.ToString();
        }
    }

    public void UpdateScore(int i,  int player)
    {
        if (player == 1)
        {
            playerOneLapText.text =  i.ToString();
        }

        if (player == 2)
        {
            playerTwoLapText.text =  i.ToString();
        }
    }

    public void EnableWinScreen(int player)
    {
        winText.text = "Player: " + player.ToString() + " Has Won!";
        winScreen.SetActive(true);
    }
}
