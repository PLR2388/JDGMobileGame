using System;
using JDG.Application.Services;

namespace JDG.Infrastructure.Services
{
    /// <summary>
    /// Implementation of ITutorialStateService.
    /// Phase 90: Replaces DialogueTutoHandler singleton for tutorial state management.
    /// </summary>
    public class TutorialStateService : ITutorialStateService
    {
        private int _currentDialogIndex;

        /// <inheritdoc />
        public int CurrentDialogIndex => _currentDialogIndex;

        /// <inheritdoc />
        public event Action<int> DialogIndexChanged;

        /// <inheritdoc />
        public void SetDialogIndex(int index)
        {
            if (_currentDialogIndex != index)
            {
                _currentDialogIndex = index;
                DialogIndexChanged?.Invoke(index);
            }
        }
    }
}
