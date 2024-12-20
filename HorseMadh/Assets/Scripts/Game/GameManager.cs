using UnityEngine;

/// <summary>
/// This script uses singletons and a subscribe event to let players change their lap count and check if any player has gotten 3 laps first
/// sources: https://www.youtube.com/watch?v=4I0vonyqMi8
/// </summary>

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    //win screen

    //get player 1
    //get player 2

    //update laps (player)

    //check if 3 laps are achieved by player X

    //show win screen for player X
}
