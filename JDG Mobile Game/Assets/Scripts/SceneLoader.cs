using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private GameState gameState;

    private void Awake()
    {
        gameState = FindObjectOfType<GameState>();
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit!");
    }

    public void GoToTutorial()
    {
        gameState.BuildDeckForTuto();
        SceneManager.LoadSceneAsync("TutoPlayerGame", LoadSceneMode.Single);
    }
}