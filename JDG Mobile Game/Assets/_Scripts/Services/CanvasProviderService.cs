using JDG.Application.Services;
using UnityEngine;

namespace JDG.Infrastructure.Services
{
    /// <summary>
    /// Infrastructure implementation of ICanvasProvider.
    /// Phase 65: Provides the game canvas for UI operations.
    /// Phase 144: Added ClearCanvas() for proper scene transition handling.
    /// </summary>
    public class CanvasProviderService : ICanvasProvider
    {
        private Transform _gameCanvas;

        /// <summary>
        /// Sets the game canvas. Should be called during scene initialization.
        /// </summary>
        public void SetCanvas(Transform canvas)
        {
            _gameCanvas = canvas;
#if UNITY_EDITOR
            Debug.Log($"CanvasProviderService: Canvas set to {canvas?.name ?? "null"}");
#endif
        }

        /// <summary>
        /// Clears the canvas reference. Should be called during scene unload.
        /// Phase 144: Prevents stale references after scene destruction.
        /// </summary>
        public void ClearCanvas()
        {
            _gameCanvas = null;
#if UNITY_EDITOR
            Debug.Log("CanvasProviderService: Canvas cleared");
#endif
        }

        /// <summary>
        /// Gets the game canvas for dialog and UI display.
        /// Returns null if canvas is not set or has been destroyed.
        /// </summary>
        public object GetGameCanvas()
        {
            // Phase 144: Check for destroyed Unity objects
            if (_gameCanvas == null)
            {
                Debug.LogWarning("CanvasProviderService: Canvas not set or destroyed. Returning null.");
                return null;
            }

            // Unity-specific: check if the object was destroyed
            if (_gameCanvas.Equals(null))
            {
                Debug.LogWarning("CanvasProviderService: Canvas was destroyed. Clearing reference.");
                _gameCanvas = null;
                return null;
            }

            return _gameCanvas;
        }
    }
}
