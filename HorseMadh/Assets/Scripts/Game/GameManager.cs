using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

/// <summary>
/// This script uses singletons and a subscribe event to let players change their lap count and check if any player has gotten 3 laps first
/// sources: https://www.youtube.com/watch?v=4I0vonyqMi8
/// </summary>

public class GameManager : MonoBehaviour
{
    [SerializeField] private int lapCount = 3;

    [SerializeField] private int playerOneLap;
    [SerializeField] private int playerTwoLap;

    //singleton, static instance
    public static GameManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public void lapCountUp(int playerindex)
    {
        switch (playerindex)
        {
            case 0:
                playerOneLap++;
                CheckWin();
                break;

            case 1:
                playerTwoLap++;
                CheckWin();
                break;

            default:
                Debug.LogError("More then 2 'horsemovements' in the scene");
                break;
                
        }
    }

    private void CheckWin()
    {
        if (playerOneLap >= lapCount)
        {
            //playerOneWin
            StartCoroutine(WinPlayer(1));
        }

        if (playerTwoLap >= lapCount)
        {
            //playerTwoWin
            StartCoroutine(WinPlayer(2));
        }
    }

    IEnumerator WinPlayer(int player)
    {
        //enable win screen
        //edit text "player 'player' wins"
        //wait for a second
        //scene transition to menu

        //or

        //instantiate win screen with the playerindex
            //let the winscreen handle itself
        //wait a second
        //transition to main menu

       yield return null;
    }
}
