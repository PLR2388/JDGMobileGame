using JDG.Application.Services;
using UnityEngine;

namespace JDG.Infrastructure.Services
{
    /// <summary>
    /// Infrastructure implementation of ICanvasProvider.
    /// Phase 65: Provides the game canvas for UI operations.
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
        }

        /// <summary>
        /// Gets the game canvas for dialog and UI display.
        /// </summary>
        public object GetGameCanvas()
        {
            if (_gameCanvas == null)
            {
                Debug.LogWarning("CanvasProviderService: Canvas not set. Returning null.");
            }
            return _gameCanvas;
        }
    }
}
