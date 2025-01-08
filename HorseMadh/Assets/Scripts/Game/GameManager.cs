using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// This script uses singletons and a subscribe event to let players change their lap count and check if any player has gotten 3 laps first
/// sources: https://www.youtube.com/watch?v=4I0vonyqMi8
/// </summary>

public class GameManager : MonoBehaviour
{
    [SerializeField] private int lapCount = 3;

    [SerializeField] private HorseMovement playerOne;
    [SerializeField] private HorseMovement playerTwo; 

    [Header("Win Events")]
    [SerializeField] private GameObject winScreen;
    [SerializeField] private float timeWinToTransition = 3f;
    public UnityEvent hasGameEnded;

    private void Start()
    {
        if (winScreen != null) winScreen.SetActive(false); //ensure its always false at the start
        if (playerOne != null) playerOne.onLapCompleted += CheckWin;
        if (playerTwo != null) playerTwo.onLapCompleted += CheckWin;
    }

    private void CheckWin(int i, HorseMovement player)
    {
        if (i >= lapCount)
        {
            StartCoroutine(WinPlayer(player));
        }
    }

    IEnumerator WinPlayer(HorseMovement player)
    {
        //instantiate win screen with the playerindex
        //let the winscreen handle itself
        if(winScreen !=null) winScreen.SetActive(true);

        //invoke sounds/paarticles/other things
        hasGameEnded.Invoke();

        //wait a second
        yield return new WaitForSeconds(timeWinToTransition);

        //transition to main menu
        SceneManager.LoadScene(0); //main menu should always be the first screen


        yield return null;
    }
}
