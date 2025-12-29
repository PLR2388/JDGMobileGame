namespace JDG.Application.Services
{
    /// <summary>
    /// Provides access to the game canvas for UI operations.
    /// Phase 65: Created to abstract Unity Transform canvas from UseCases.
    /// </summary>
    public interface ICanvasProvider
    {
        /// <summary>
        /// Gets the game canvas for dialog and UI display.
        /// Returns as object to avoid Unity dependency in JDG.Application layer.
        /// Callers should cast to UnityEngine.Transform.
        /// </summary>
        object GetGameCanvas();
    }
}
