using UnityEngine;
using UnityEngine.SceneManagement;

public class DDOL : MonoBehaviour
{
    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (gameObject.name == "Message Box")
        {
            gameObject.SetActive(false);
        }
        var currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "_preload")
        {
            SceneManager.LoadSceneAsync("MainScreen", LoadSceneMode.Single);
        }
    }
}