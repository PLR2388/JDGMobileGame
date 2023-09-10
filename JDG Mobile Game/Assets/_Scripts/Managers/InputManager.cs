using UnityEngine;
using UnityEngine.Events;

public class InputManager : Singleton<InputManager>
{
    [SerializeField]
    private float clickDuration = 2f;

    private bool isTouchDetectionDisabled;
    private bool isTouchInProgress;
    private float totalDownTime;

    /// <summary>
    /// Checks if the user started a touch or click input.
    /// </summary>
    private bool IsTouch => Input.GetMouseButtonDown(0);
    
    /// <summary>
    /// Checks if the user is continuously touching or clicking.
    /// </summary>
    private bool IsTouching => Input.GetMouseButton(0);
    
    /// <summary>
    /// Checks if the user just stopped a touch or click input.
    /// </summary>
    private bool IsJustStopTouching => Input.GetMouseButtonUp(0);

    /// <summary>
    /// Gets the position of the current touch or click input.
    /// </summary>
    public static Vector3 TouchPosition
    {
        get
        {
#if UNITY_EDITOR
            return Input.mousePosition;
#elif UNITY_ANDROID
            return Input.GetTouch(0).position;
#else
            return Vector3.zero; // Default return for other platforms
#endif

        }
    }

    /// <summary>
    /// Enables touch detection.
    /// </summary>
    public void EnableDetectionTouch()
    {
        isTouchDetectionDisabled = false;
    }
    
    /// <summary>
    /// Disables touch detection.
    /// </summary>
    public void DisableDetectionTouch()
    {
        isTouchDetectionDisabled = true;
    }

    /// <summary>
    /// Event invoked when a touch/click starts.
    /// </summary
    public static readonly UnityEvent OnTouch = new UnityEvent();

    /// <summary>
    /// Event invoked when a long touch/click is detected.
    /// </summary>
    public static readonly UnityEvent OnLongTouch = new UnityEvent();

    /// <summary>
    /// Event invoked when a touch/click ends.
    /// </summary>
    public static readonly UnityEvent OnReleaseTouch = new UnityEvent();

    /// <summary>
    /// Event invoked when the Android back button is pressed.
    /// </summary>
    public static readonly UnityEvent OnBackPressed = new UnityEvent();

    private void Update()
    {
        HandleTouchInput();
        HandleAndroidBackButton();
    }
    
    /// <summary>
    /// Handles touch and click input detection.
    /// </summary>
    private void HandleTouchInput()
    {

        if (isTouchDetectionDisabled) return;
        if (IsTouch)
        {
            totalDownTime = 0;
            isTouchInProgress = true;
            OnTouch.Invoke();
        }
        
        if (!isTouchInProgress) return;
        if (IsTouching)
        {
            totalDownTime += Time.deltaTime;

            if (totalDownTime >= clickDuration)
            {
                Debug.Log("Long click");
                OnLongTouch.Invoke();
            }
        }
        if (IsJustStopTouching)
        {
            isTouchInProgress = false;
            OnReleaseTouch.Invoke();
        }
    }
    
    /// <summary>
    /// Handles the behavior for the Android back button.
    /// </summary>
    private static void HandleAndroidBackButton()
    {

        if (Application.platform == RuntimePlatform.Android &&
            Input.GetKeyDown(KeyCode.Escape))
        {
            // Make sure user is on Android platform
            // Check if Back was pressed this frame
            OnBackPressed.Invoke();
        }
    }
}