using System;
using JDG.Application.Services;
using UnityEngine;

namespace JDG.Presentation.Presenters
{
    /// <summary>
    /// Presenter for displaying dialog boxes and message boxes (MVP pattern).
    /// Replaces UIManager's message box responsibility.
    /// Part of Phase 5 - UIManager decomposition.
    /// Phase 34: Updated to use ILocalizationService instead of LocalizationSystem.Instance.
    /// Phase 40: Updated to use IDialogService instead of MessageBox.Instance.
    /// Phase 43: Migrated to JDG.Presentation using MessageBoxOptions instead of legacy MessageBoxConfig.
    /// </summary>
    public class DialogPresenter
    {
        private readonly Transform _canvas;
        private readonly ILocalizationService _localizationService;
        private readonly IDialogService _dialogService;

        /// <summary>
        /// Phase 157: Added null validation for critical dependencies.
        /// </summary>
        public DialogPresenter(Transform canvas, ILocalizationService localizationService, IDialogService dialogService)
        {
            // Phase 157: Validate required dependencies to prevent NullReferenceException at runtime
            _canvas = canvas ?? throw new ArgumentNullException(nameof(canvas),
                "DialogPresenter requires a canvas for displaying dialogs");
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService),
                "DialogPresenter requires ILocalizationService for localized text");
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService),
                "DialogPresenter requires IDialogService for showing dialogs");
        }

        /// <summary>
        /// Displays a pause menu with the given action.
        /// </summary>
        /// <param name="onPositiveAction">Action to execute when user confirms.</param>
        public void ShowPauseMenu(Action onPositiveAction)
        {
            var options = new MessageBoxOptions
            {
                Title = _localizationService.GetLocalizedValue("PAUSE_TITLE"),
                Message = _localizationService.GetLocalizedValue("PAUSE_MESSAGE"),
                ShowPositiveButton = true,
                ShowNegativeButton = true,
                OnPositive = onPositiveAction
            };

            _dialogService.ShowMessageBox(_canvas, options);
        }

        /// <summary>
        /// Displays a generic message box.
        /// </summary>
        /// <param name="title">Dialog title.</param>
        /// <param name="message">Dialog message.</param>
        /// <param name="onPositiveAction">Optional action for positive button.</param>
        /// <param name="onNegativeAction">Optional action for negative button.</param>
        public void ShowMessageBox(
            string title,
            string message,
            Action onPositiveAction = null,
            Action onNegativeAction = null)
        {
            var options = new MessageBoxOptions
            {
                Title = title,
                Message = message,
                ShowPositiveButton = onPositiveAction != null,
                ShowNegativeButton = onNegativeAction != null,
                ShowOkButton = onPositiveAction == null && onNegativeAction == null,
                OnPositive = onPositiveAction,
                OnNegative = onNegativeAction
            };

            _dialogService.ShowMessageBox(_canvas, options);
        }

        /// <summary>
        /// Displays a warning message box.
        /// </summary>
        /// <param name="message">Warning message to display.</param>
        public void ShowWarning(string message)
        {
            var options = new MessageBoxOptions
            {
                Title = _localizationService.GetLocalizedValue("WARNING_TITLE"),
                Message = message,
                ShowOkButton = true
            };

            _dialogService.ShowMessageBox(_canvas, options);
        }
    }
}
