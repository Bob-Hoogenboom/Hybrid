using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnApplicationQuit()
    {
        Application.Quit();
    }

    public void LoadNextScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
