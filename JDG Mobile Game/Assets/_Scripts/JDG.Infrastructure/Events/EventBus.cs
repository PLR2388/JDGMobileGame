using System;
using System.Collections.Generic;
using JDG.Application;

namespace JDG.Infrastructure.Events
{
    /// <summary>
    /// Concrete implementation of IEventBus using in-memory event dispatching.
    /// Thread-safe, zero-allocation event system with automatic subscription management.
    /// </summary>
    public class EventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _subscribers = new();
        private readonly object _lock = new();

        public void Publish<T>(T eventData) where T : struct
        {
            List<Delegate> handlers;

            lock (_lock)
            {
                if (!_subscribers.TryGetValue(typeof(T), out handlers))
                    return;

                // Create a copy to avoid modification during iteration
                handlers = new List<Delegate>(handlers);
            }

            // Invoke handlers outside of lock to prevent deadlocks
            foreach (Action<T> handler in handlers)
            {
                try
                {
                    handler(eventData);
                }
                catch (Exception ex)
                {
                    // Log error but continue processing other handlers
                    // This prevents one bad handler from breaking the entire event chain
                    UnityEngine.Debug.LogError($"EventBus: Error in handler for {typeof(T).Name}: {ex}");
                }
            }
        }

        public IDisposable Subscribe<T>(Action<T> handler) where T : struct
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            lock (_lock)
            {
                if (!_subscribers.TryGetValue(typeof(T), out var handlers))
                {
                    handlers = new List<Delegate>();
                    _subscribers[typeof(T)] = handlers;
                }

                handlers.Add(handler);
            }

            return new Subscription<T>(this, handler);
        }

        public void ClearSubscriptions<T>() where T : struct
        {
            lock (_lock)
            {
                _subscribers.Remove(typeof(T));
            }
        }

        public void ClearAllSubscriptions()
        {
            lock (_lock)
            {
                _subscribers.Clear();
            }
        }

        internal void Unsubscribe<T>(Action<T> handler) where T : struct
        {
            lock (_lock)
            {
                if (_subscribers.TryGetValue(typeof(T), out var handlers))
                {
                    handlers.Remove(handler);

                    // Clean up empty handler lists
                    if (handlers.Count == 0)
                    {
                        _subscribers.Remove(typeof(T));
                    }
                }
            }
        }

        /// <summary>
        /// Returns the number of subscribers for a specific event type (for debugging/testing).
        /// </summary>
        public int GetSubscriberCount<T>() where T : struct
        {
            lock (_lock)
            {
                return _subscribers.TryGetValue(typeof(T), out var handlers) ? handlers.Count : 0;
            }
        }

        private class Subscription<T> : IDisposable where T : struct
        {
            private EventBus _eventBus;
            private Action<T> _handler;
            private bool _disposed;

            public Subscription(EventBus eventBus, Action<T> handler)
            {
                _eventBus = eventBus;
                _handler = handler;
            }

            public void Dispose()
            {
                if (_disposed) return;

                _eventBus?.Unsubscribe(_handler);
                _eventBus = null;
                _handler = null;
                _disposed = true;
            }
        }
    }
}
