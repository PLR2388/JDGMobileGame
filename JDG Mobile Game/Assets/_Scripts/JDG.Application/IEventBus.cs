using System;

namespace JDG.Application
{
    /// <summary>
    /// Central event bus for decoupled communication between systems.
    /// Replaces static UnityEvents with a testable, injectable event system.
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// Publishes an event to all subscribers.
        /// </summary>
        /// <typeparam name="T">Event type (must be struct)</typeparam>
        /// <param name="eventData">The event data to publish</param>
        void Publish<T>(T eventData) where T : struct;

        /// <summary>
        /// Subscribes to an event type.
        /// </summary>
        /// <typeparam name="T">Event type (must be struct)</typeparam>
        /// <param name="handler">Handler to call when event is published</param>
        /// <returns>Disposable subscription - dispose to unsubscribe</returns>
        IDisposable Subscribe<T>(Action<T> handler) where T : struct;

        /// <summary>
        /// Clears all subscriptions for a specific event type.
        /// </summary>
        /// <typeparam name="T">Event type to clear</typeparam>
        void ClearSubscriptions<T>() where T : struct;

        /// <summary>
        /// Clears all subscriptions for all event types.
        /// </summary>
        void ClearAllSubscriptions();
    }
}
