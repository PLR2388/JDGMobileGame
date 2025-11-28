using System;
using JDG.Domain.ValueObjects;

namespace JDG.Application.Services
{
    /// <summary>
    /// Touch event data.
    /// </summary>
    public struct TouchEventData
    {
        public Vector2 Position;
        public float Timestamp;
        public int FingerId;
    }

    /// <summary>
    /// Service for handling input events.
    /// Replaces InputManager singleton and static UnityEvents.
    /// </summary>
    public interface IInputService
    {
        /// <summary>
        /// Subscribe to touch started events.
        /// Returns an IDisposable for unsubscribing.
        /// </summary>
        IDisposable SubscribeToTouchStarted(Action<TouchEventData> handler);

        /// <summary>
        /// Subscribe to touch ended events.
        /// Returns an IDisposable for unsubscribing.
        /// </summary>
        IDisposable SubscribeToTouchEnded(Action<TouchEventData> handler);

        /// <summary>
        /// Subscribe to long touch events.
        /// Returns an IDisposable for unsubscribing.
        /// </summary>
        IDisposable SubscribeToLongTouch(Action<TouchEventData> handler);

        /// <summary>
        /// Subscribe to back button pressed events.
        /// Returns an IDisposable for unsubscribing.
        /// </summary>
        IDisposable SubscribeToBackButton(System.Action handler);

        /// <summary>
        /// Gets the current touch position (if touching).
        /// </summary>
        Vector2? GetCurrentTouchPosition();

        /// <summary>
        /// Checks if currently touching the screen.
        /// </summary>
        bool IsTouching { get; }
    }
}
