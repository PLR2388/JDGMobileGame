using JDG.Application;
using JDG.Domain.Events;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

/// <summary>
/// Handles input detection including touch/click and Android back button.
/// Phase 19-20: Converted from singleton to regular MonoBehaviour with VContainer registration.
/// Phase 23: Migrated static UnityEvents to EventBus.
/// </summary>
public class InputManager : MonoBehaviour
{
    [SerializeField] private float clickDuration = 2f;

    private bool isTouchDetectionDisabled;
    private bool isTouchInProgress;
    private float totalDownTime;

    // Phase 23: EventBus for static UnityEvent migration
    private IEventBus _eventBus;

    /// <summary>
    /// VContainer method injection for EventBus.
    /// Phase 23: Inject IEventBus for static UnityEvent migration.
    /// </summary>
    [Inject]
    public void Construct(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    /// <summary>
    /// Checks if the user tap at a random location.
    ///
    /// [OBSOLETE] Use IInputService with EventBus (TouchStartedEvent) instead.
    /// </summary>
    [System.Obsolete("Use IInputService.SubscribeToTouchStarted() or EventBus TouchStartedEvent instead.")]
    public static bool IsTap
    {
        get
        {
#if UNITY_EDITOR
            return Input.GetKeyDown(KeyCode.Space);
#elif UNITY_ANDROID
        return Input.touchCount > 0;
#else
        return false;
#endif
        }
    }

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
    ///
    /// [OBSOLETE] Use IInputService.GetCurrentTouchPosition() instead.
    /// </summary>
    [System.Obsolete("Use IInputService.GetCurrentTouchPosition() instead for better testability.")]
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
    /// Phase 23: Publishes to EventBus in addition to static UnityEvents.
    /// </summary>
    private void HandleTouchInput()
    {

        if (isTouchDetectionDisabled) return;
        if (IsTouch)
        {
            totalDownTime = 0;
            isTouchInProgress = true;
            OnTouch.Invoke();
            _eventBus.Publish(new TouchStartedEvent
            {
                Position = (Vector2)Input.mousePosition,
                Timestamp = Time.time
            });
        }

        if (!isTouchInProgress) return;
        if (IsTouching)
        {
            totalDownTime += Time.deltaTime;

            if (totalDownTime >= clickDuration)
            {
                Debug.Log("Long click");
                OnLongTouch.Invoke();
                _eventBus.Publish(new LongTouchEvent
                {
                    Position = (Vector2)Input.mousePosition,
                    Duration = totalDownTime
                });
            }
        }
        if (IsJustStopTouching)
        {
            isTouchInProgress = false;
            OnReleaseTouch.Invoke();
            _eventBus.Publish(new TouchEndedEvent
            {
                Position = (Vector2)Input.mousePosition,
                Duration = totalDownTime
            });
        }
    }

    /// <summary>
    /// Handles the behavior for the Android back button.
    /// Phase 23: Publishes to EventBus in addition to static UnityEvent.
    /// </summary>
    private void HandleAndroidBackButton()
    {
        if (Application.platform == RuntimePlatform.Android &&
            Input.GetKeyDown(KeyCode.Escape))
        {
            // Make sure user is on Android platform
            // Check if Back was pressed this frame
            OnBackPressed.Invoke();
            _eventBus.Publish(new BackButtonPressedEvent
            {
                Timestamp = Time.time
            });
        }
    }
}