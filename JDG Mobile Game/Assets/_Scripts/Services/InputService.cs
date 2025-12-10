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
    /// Wraps the existing InputManager and provides events through EventBus.
    /// Phase 19-20: Now injects InputManager instead of using .Instance.
    /// Phase 23: Updated to subscribe to EventBus instead of static UnityEvents.
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

        private IDisposable _touchStartedSubscription;
        private IDisposable _longTouchSubscription;
        private IDisposable _touchEndedSubscription;
        private IDisposable _backButtonSubscription;

        public InputService(IEventBus eventBus, InputManager inputManager)
        {
            _eventBus = eventBus;
            _inputManager = inputManager;

            // Phase 23: Subscribe to EventBus events instead of static UnityEvents
            _touchStartedSubscription = _eventBus.Subscribe<TouchStartedEvent>(OnTouchStarted);
            _longTouchSubscription = _eventBus.Subscribe<LongTouchEvent>(OnLongTouchDetected);
            _touchEndedSubscription = _eventBus.Subscribe<TouchEndedEvent>(OnTouchEnded);
            _backButtonSubscription = _eventBus.Subscribe<BackButtonPressedEvent>(OnBackButtonPressed);
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

        // Event handlers that receive EventBus events and notify local handlers
        // Phase 23: Updated to receive EventBus event parameters
        private void OnTouchStarted(TouchStartedEvent evt)
        {
            var eventData = new TouchEventData
            {
                Position = evt.Position,
                Timestamp = evt.Timestamp,
                FingerId = 0
            };

            NotifyHandlers(_touchStartedHandlers, eventData);
        }

        private void OnLongTouchDetected(LongTouchEvent evt)
        {
            var eventData = new TouchEventData
            {
                Position = evt.Position,
                Timestamp = Time.time,
                FingerId = 0
            };

            NotifyHandlers(_longTouchHandlers, eventData);
        }

        private void OnTouchEnded(TouchEndedEvent evt)
        {
            var eventData = new TouchEventData
            {
                Position = evt.Position,
                Timestamp = Time.time,
                FingerId = 0
            };

            NotifyHandlers(_touchEndedHandlers, eventData);
        }

        private void OnBackButtonPressed(BackButtonPressedEvent evt)
        {
            NotifyHandlers(_backButtonHandlers);
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
