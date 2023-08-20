using UnityEngine;
using UnityEngine.Events;

public class InputManager : Singleton<InputManager>
{
    private const float ClickDuration = 2;

    private bool stopDetectClicking;
    private bool clicking;
    private float totalDownTime;

    private bool IsTouch => Input.GetMouseButtonDown(0);
    private bool IsTouching => Input.GetMouseButton(0);
    private bool IsJustStopTouching => Input.GetMouseButtonUp(0);

    public static Vector3 TouchPosition
    {
        get
        {
#if UNITY_EDITOR
            var position = Input.mousePosition;
#elif UNITY_ANDROID
        var position = Input.GetTouch(0).position;
#endif
            return position;
        }
    }

    public bool HasStopDetectTouch => stopDetectClicking;

    public void EnableDetectionTouch()
    {
        stopDetectClicking = false;
    }

    public void DisableDetectionTouch()
    {
        stopDetectClicking = true;
    }

    public static readonly UnityEvent OnTouch = new UnityEvent();

    public static readonly UnityEvent OnLongTouch = new UnityEvent();

    public static readonly UnityEvent OnReleaseTouch = new UnityEvent();

    public static readonly UnityEvent OnBackPressed = new UnityEvent();

    private void Update()
    {
        if (stopDetectClicking) return;
        if (IsTouch)
        {
            totalDownTime = 0;
            clicking = true;
            OnTouch.Invoke();
        }
        if (clicking && IsTouching)
        {
            totalDownTime += Time.deltaTime;

            if (totalDownTime >= ClickDuration)
            {
                Debug.Log("Long click");
                clicking = false;
                OnLongTouch.Invoke();
            }
        }
        if (clicking && IsJustStopTouching)
        {
            clicking = false;
            OnReleaseTouch.Invoke();
        }

        if (Application.platform == RuntimePlatform.Android &&
            Input.GetKeyDown(KeyCode.Escape))
        {
            // Make sure user is on Android platform
            // Check if Back was pressed this frame
            OnBackPressed.Invoke();
        }
    }
}