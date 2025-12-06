using System;
using System.Collections.Generic;
using JDG.Application;
using JDG.Application.Services;
using JDG.Domain.Events;
using JDG.Domain.ValueObjects;
using UnityEngine;

namespace JDG.Infrastructure.Services
{
    /// <summary>
    /// Infrastructure implementation of IInputService.
    /// Wraps the existing InputManager and publishes events to EventBus.
    /// Uses dual publishing pattern - both UnityEvents (old) and EventBus (new).
    ///
    /// Phase 19-20: Now injects InputManager instead of using .Instance.
    /// Note: This is a regular class, not a MonoBehaviour. InputManager MonoBehaviour
    /// handles the Update loop. This service just provides a clean interface.
    /// </summary>
    public class InputService : IInputService
    {
        private readonly IEventBus _eventBus;
        private readonly InputManager _inputManager;

        private readonly List<Action<TouchEventData>> _touchStartedHandlers = new();
        private readonly List<Action<TouchEventData>> _touchEndedHandlers = new();
        private readonly List<Action<TouchEventData>> _longTouchHandlers = new();
        private readonly List<System.Action> _backButtonHandlers = new();

        public InputService(IEventBus eventBus, InputManager inputManager)
        {
            _eventBus = eventBus;
            _inputManager = inputManager;

            // Subscribe to InputManager's static events and republish through our handlers + EventBus
            InputManager.OnTouch.AddListener(OnTouchStarted);
            InputManager.OnLongTouch.AddListener(OnLongTouchDetected);
            InputManager.OnReleaseTouch.AddListener(OnTouchEnded);
            InputManager.OnBackPressed.AddListener(OnBackButtonPressed);
        }

        public bool IsTouching => Input.GetMouseButton(0);

        public JDG.Domain.ValueObjects.Vector2? GetCurrentTouchPosition()
        {
            if (!IsTouching)
                return null;

            var unityPos = InputManager.TouchPosition;
            return new JDG.Domain.ValueObjects.Vector2(unityPos.x, unityPos.y);
        }

        // Helper method to convert Unity Vector2 to domain Vector2
        private static JDG.Domain.ValueObjects.Vector2 ToVector2(UnityEngine.Vector2 vector)
        {
            return new JDG.Domain.ValueObjects.Vector2(vector.x, vector.y);
        }

        // Helper method to convert Unity Vector3 to domain Vector2
        private static JDG.Domain.ValueObjects.Vector2 ToVector2(UnityEngine.Vector3 vector)
        {
            return new JDG.Domain.ValueObjects.Vector2(vector.x, vector.y);
        }

        public IDisposable SubscribeToTouchStarted(Action<TouchEventData> handler)
        {
            _touchStartedHandlers.Add(handler);
            return new DisposableSubscription(() => { _touchStartedHandlers.Remove(handler); });
        }

        public IDisposable SubscribeToTouchEnded(Action<TouchEventData> handler)
        {
            _touchEndedHandlers.Add(handler);
            return new DisposableSubscription(() => { _touchEndedHandlers.Remove(handler); });
        }

        public IDisposable SubscribeToLongTouch(Action<TouchEventData> handler)
        {
            _longTouchHandlers.Add(handler);
            return new DisposableSubscription(() => { _longTouchHandlers.Remove(handler); });
        }

        public IDisposable SubscribeToBackButton(System.Action handler)
        {
            _backButtonHandlers.Add(handler);
            return new DisposableSubscription(() => { _backButtonHandlers.Remove(handler); });
        }

        // Event handlers that republish InputManager events
        private void OnTouchStarted()
        {
            var unityPos = InputManager.TouchPosition;
            var eventData = new TouchEventData
            {
                Position = ToVector2(unityPos),
                Timestamp = Time.time,
                FingerId = 0
            };

            NotifyHandlers(_touchStartedHandlers, eventData);
            _eventBus.Publish(new TouchStartedEvent
            {
                Position = ToVector2(unityPos),
                Timestamp = eventData.Timestamp
            });
        }

        private void OnLongTouchDetected()
        {
            var unityPos = InputManager.TouchPosition;
            var eventData = new TouchEventData
            {
                Position = ToVector2(unityPos),
                Timestamp = Time.time,
                FingerId = 0
            };

            NotifyHandlers(_longTouchHandlers, eventData);
            _eventBus.Publish(new LongTouchEvent
            {
                Position = ToVector2(unityPos),
                Duration = 2f // InputManager's click duration
            });
        }

        private void OnTouchEnded()
        {
            var unityPos = InputManager.TouchPosition;
            var eventData = new TouchEventData
            {
                Position = ToVector2(unityPos),
                Timestamp = Time.time,
                FingerId = 0
            };

            NotifyHandlers(_touchEndedHandlers, eventData);
            _eventBus.Publish(new TouchEndedEvent
            {
                Position = ToVector2(unityPos),
                Duration = 0f
            });
        }

        private void OnBackButtonPressed()
        {
            NotifyHandlers(_backButtonHandlers);
            _eventBus.Publish(new BackButtonPressedEvent
            {
                Timestamp = Time.time
            });
        }

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

        private void NotifyHandlers(List<System.Action> handlers)
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
            private readonly System.Action _unsubscribeAction;
            private bool _disposed;

            public DisposableSubscription(System.Action unsubscribeAction)
            {
                _unsubscribeAction = unsubscribeAction;
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    _unsubscribeAction?.Invoke();
                    _disposed = true;
                }
            }
        }
    }
}
