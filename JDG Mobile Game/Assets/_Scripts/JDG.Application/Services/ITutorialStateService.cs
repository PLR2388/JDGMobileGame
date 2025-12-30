using System;

namespace JDG.Application.Services
{
    /// <summary>
    /// Service for managing tutorial state.
    /// Phase 90: Replaces DialogueTutoHandler singleton.
    /// </summary>
    public interface ITutorialStateService
    {
        /// <summary>
        /// Gets the current dialog index in the tutorial.
        /// </summary>
        int CurrentDialogIndex { get; }

        /// <summary>
        /// Sets the current dialog index.
        /// </summary>
        /// <param name="index">The new dialog index.</param>
        void SetDialogIndex(int index);

        /// <summary>
        /// Event triggered when the dialog index changes.
        /// </summary>
        event Action<int> DialogIndexChanged;
    }
}
