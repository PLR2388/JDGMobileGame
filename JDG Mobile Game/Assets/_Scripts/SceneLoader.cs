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

    public void onClickStory()
    {
        _ShowAndroidToastMessage("Si tu as des idées pour l'histoire, je t'invite à me contacter !");
    }
    
    /// <param name="message">Message string to show in the toast.</param>
    private void _ShowAndroidToastMessage(string message)
    {
#if UNITY_EDITOR
        Debug.Log(message);
#elif UNITY_ANDROID
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

        if (unityActivity != null)
        {
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
                toastObject.Call("show");
            }));
        }
#endif
    }
}