using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit!");
    }
}