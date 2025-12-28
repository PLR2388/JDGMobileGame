using JDG.Application.Services;
using Sound;
using UnityEngine;
using VContainer;

/// <summary>
/// Phase 17-18: Removed GameState singleton dependency via IDeckManagementService.
/// Phase 39: Uses ILocalizationService instead of LocalizationSystem.Instance.
/// Phase 8: Uses IAudioService instead of AudioSystem.Instance.
/// Phase 55: Uses ISceneLoaderService instead of direct SceneManager/Application calls.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    private const string TutorialScene = "TutoPlayerGame";

    // Phase 17-18: Injected dependencies
    private IDeckManagementService _deckManagementService;
    // Phase 39: ILocalizationService instead of LocalizationSystem.Instance
    private ILocalizationService _localizationService;
    // Phase 8: IAudioService instead of AudioSystem.Instance
    private IAudioService _audioService;
    // Phase 55: ISceneLoaderService instead of direct SceneManager/Application calls
    private ISceneLoaderService _sceneLoaderService;

    /// <summary>
    /// VContainer method injection for dependencies.
    /// Phase 17-18: Inject IDeckManagementService instead of GameState.Instance.
    /// Phase 39: Inject ILocalizationService instead of LocalizationSystem.Instance.
    /// Phase 8: Inject IAudioService instead of AudioSystem.Instance.
    /// Phase 55: Inject ISceneLoaderService instead of direct SceneManager/Application calls.
    /// </summary>
    [Inject]
    public void Construct(
        IDeckManagementService deckManagementService,
        ILocalizationService localizationService,
        IAudioService audioService,
        ISceneLoaderService sceneLoaderService)
    {
        _deckManagementService = deckManagementService;
        _localizationService = localizationService;
        _audioService = audioService;
        _sceneLoaderService = sceneLoaderService;
    }

    /// <summary>
    /// Quits the game application.
    /// Phase 55: Uses ISceneLoaderService instead of Application.Quit().
    /// </summary>
    public void QuitGame()
    {
        _sceneLoaderService.QuitGame();
    }

    /// <summary>
    /// Navigates the player to the tutorial scene.
    /// Phase 8: Uses IAudioService instead of AudioSystem.Instance.
    /// Phase 55: Uses ISceneLoaderService instead of SceneManager.
    /// </summary>
    public void GoToTutorial()
    {
        if (_deckManagementService != null)
        {
            _deckManagementService.BuildTutorialDecks();
        }

        // Phase 8: Use IAudioService instead of AudioSystem.Instance
        _audioService?.StopMusic();

        // Phase 55: Use ISceneLoaderService instead of SceneManager
        _sceneLoaderService.LoadSceneAsync(TutorialScene);
    }

    /// <summary>
    /// Handles the onClick event for the story button. Shows a toast message.
    /// Phase 39: Uses ILocalizationService instead of LocalizationSystem.Instance.
    /// Phase 54: Removed fallback pattern - DI is properly configured.
    /// </summary>
    public void OnClickStory()
    {
        var message = _localizationService.GetLocalizedValue(LocalizationKeys.TOAST_ASK.ToString());
        ShowAndroidToastMessage(message);
    }

    /// <summary>
    /// Shows a toast message with the provided text. 
    /// Displays the message as a log entry in the Unity Editor and as a toast on Android.
    /// This method CANNOT be static
    /// </summary>
    /// <param name="message">Message string to show in the toast.</param>
    private void ShowAndroidToastMessage(string message)
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