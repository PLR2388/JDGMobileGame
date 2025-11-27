using System;
using System.Collections.Generic;
using JDG.Application;
using JDG.Application.Services;
using JDG.Domain.Events;
using UnityEngine;

namespace JDG.Infrastructure.Services
{
    /// <summary>
    /// Infrastructure implementation of IInputService.
    /// Wraps the existing InputManager and publishes events to EventBus.
    /// Uses dual publishing pattern - both UnityEvents (old) and EventBus (new).
    /// </summary>
    public class InputService : MonoBehaviour, IInputService
    {
        private readonly IEventBus _eventBus;
        private readonly InputManager _inputManager;

        private bool _isTouchDetectionDisabled;
        private bool _isTouchInProgress;
        private float _totalDownTime;
        private float _clickDuration = 2f;

        private readonly List<Action<TouchEventData>> _touchStartedHandlers = new();
        private readonly List<Action<TouchEventData>> _touchEndedHandlers = new();
        private readonly List<Action<TouchEventData>> _longTouchHandlers = new();
        private readonly List<Action> _backButtonHandlers = new();

        public InputService(IEventBus eventBus)
        {
            _eventBus = eventBus;
            _inputManager = InputManager.Instance;
        }

        public bool IsTouching => Input.GetMouseButton(0);

        public Position2D? GetCurrentTouchPosition()
        {
            if (!IsTouching)
                return null;

            var unityPos = InputManager.TouchPosition;
            return new Position2D(unityPos.x, unityPos.y);
        }

        // Helper method to convert Unity Vector2 to Position2D
        private static Position2D ToPosition2D(Vector2 vector)
        {
            return new Position2D(vector.x, vector.y);
        }

        // Helper method to convert Unity Vector3 to Position2D
        private static Position2D ToPosition2D(Vector3 vector)
        {
            return new Position2D(vector.x, vector.y);
        }

        public IDisposable SubscribeToTouchStarted(Action<TouchEventData> handler)
        {
            _touchStartedHandlers.Add(handler);
            return new DisposableSubscription(() => _touchStartedHandlers.Remove(handler));
        }

        public IDisposable SubscribeToTouchEnded(Action<TouchEventData> handler)
        {
            _touchEndedHandlers.Add(handler);
            return new DisposableSubscription(() => _touchEndedHandlers.Remove(handler));
        }

        public IDisposable SubscribeToLongTouch(Action<TouchEventData> handler)
        {
            _longTouchHandlers.Add(handler);
            return new DisposableSubscription(() => _longTouchHandlers.Remove(handler));
        }

        public IDisposable SubscribeToBackButton(Action handler)
        {
            _backButtonHandlers.Add(handler);
            return new DisposableSubscription(() => _backButtonHandlers.Remove(handler));
        }

        private void Update()
        {
            HandleTouchInput();
            HandleAndroidBackButton();
        }

        private void HandleTouchInput()
        {
            if (_isTouchDetectionDisabled) return;

            if (IsTouch())
            {
                _totalDownTime = 0;
                _isTouchInProgress = true;

                var unityPos = InputManager.TouchPosition;
                var eventData = new TouchEventData
                {
                    Position = ToPosition2D(unityPos),
                    Timestamp = Time.time,
                    FingerId = 0
                };

                // Publish to both old (UnityEvent) and new (service handlers + EventBus) systems
                InputManager.OnTouch.Invoke();
                NotifyHandlers(_touchStartedHandlers, eventData);
                _eventBus.Publish(new TouchStartedEvent
                {
                    Position = unityPos,
                    Timestamp = eventData.Timestamp
                });
            }

            if (!_isTouchInProgress) return;

            if (IsTouching)
            {
                _totalDownTime += Time.deltaTime;

                if (_totalDownTime >= _clickDuration)
                {
                    var unityPos = InputManager.TouchPosition;
                    var eventData = new TouchEventData
                    {
                        Position = ToPosition2D(unityPos),
                        Timestamp = Time.time,
                        FingerId = 0
                    };

                    // Dual publishing
                    InputManager.OnLongTouch.Invoke();
                    NotifyHandlers(_longTouchHandlers, eventData);
                    _eventBus.Publish(new LongTouchEvent
                    {
                        Position = unityPos,
                        Duration = _totalDownTime
                    });
                }
            }

            if (IsJustStopTouching())
            {
                _isTouchInProgress = false;

                var unityPos = InputManager.TouchPosition;
                var eventData = new TouchEventData
                {
                    Position = ToPosition2D(unityPos),
                    Timestamp = Time.time,
                    FingerId = 0
                };

                // Dual publishing
                InputManager.OnReleaseTouch.Invoke();
                NotifyHandlers(_touchEndedHandlers, eventData);
                _eventBus.Publish(new TouchEndedEvent
                {
                    Position = unityPos,
                    Duration = _totalDownTime
                });
            }
        }

        private void HandleAndroidBackButton()
        {
            if (Application.platform == RuntimePlatform.Android &&
                Input.GetKeyDown(KeyCode.Escape))
            {
                // Dual publishing
                InputManager.OnBackPressed.Invoke();
                NotifyHandlers(_backButtonHandlers);
                _eventBus.Publish(new BackButtonPressedEvent
                {
                    Timestamp = Time.time
                });
            }
        }

        private static bool IsTouch() => Input.GetMouseButtonDown(0);
        private static bool IsJustStopTouching() => Input.GetMouseButtonUp(0);

        private void NotifyHandlers(List<Action<TouchEventData>> handlers, TouchEventData eventData)
        {
            // Create a copy to avoid modification during iteration
            var handlersCopy = handlers.ToArray();
            foreach (var handler in handlersCopy)
            {
                try
                {
                    handler(eventData);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"InputService: Error in handler: {ex.Message}");
                }
            }
        }

        private void NotifyHandlers(List<Action> handlers)
        {
            var handlersCopy = handlers.ToArray();
            foreach (var handler in handlersCopy)
            {
                try
                {
                    handler();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"InputService: Error in handler: {ex.Message}");
                }
            }
        }

        private class DisposableSubscription : IDisposable
        {
            private readonly Action _unsubscribeAction;
            private bool _disposed;

            public DisposableSubscription(Action unsubscribeAction)
            {
                _unsubscribeAction = unsubscribeAction;
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    _unsubscribeAction();
                    _disposed = true;
                }
            }
        }
    }
}
