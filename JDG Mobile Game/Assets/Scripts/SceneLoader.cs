using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit!");
    }

    public void GoToTutorial()
    {
        SceneManager.LoadSceneAsync("TutoPlayerGame", LoadSceneMode.Single);
    }
}