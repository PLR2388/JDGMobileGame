using Sound;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private const string TutorialScene = "TutoPlayerGame";

    /// <summary>
    /// Quits the game application.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit!");
    }

    /// <summary>
    /// Navigates the player to the tutorial scene.
    /// </summary>
    public void GoToTutorial()
    {
        if (GameState.Instance != null)
        {
            GameState.Instance.BuildDeckForTuto();
        }

        if (AudioSystem.Instance != null)
        {
            AudioSystem.Instance.StopMusic();
        }

        SceneManager.LoadSceneAsync(TutorialScene, LoadSceneMode.Single);
    }

    /// <summary>
    /// Handles the onClick event for the story button. Shows a toast message.
    /// </summary>
    public void OnClickStory()
    {
        ShowAndroidToastMessage(
            LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.TOAST_ASK)
        );
    }

    /// <summary>
    /// Shows a toast message with the provided text. 
    /// Displays the message as a log entry in the Unity Editor and as a toast on Android.
    /// </summary>
    /// <param name="message">Message string to show in the toast.</param>
    private static void ShowAndroidToastMessage(string message)
    {
#if UNITY_EDITOR
        Debug.Log(message);
#elif UNITY_ANDROID
 DisplayAndroidToast(message);
#endif
    }

#if UNITY_ANDROID
    /// <summary>
    /// Shows a toast message on Android devices using the native Android Java interface.
    /// </summary>
    /// <param name="message">The message to be displayed as a toast.</param>
private void DisplayAndroidToast(string message)
{
    AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

    if (unityActivity != null)
    {
        AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
        unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
        {
            AndroidJavaObject toastObject =
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
            toastObject.Call("show");
        }));
    }
}
#endif
}