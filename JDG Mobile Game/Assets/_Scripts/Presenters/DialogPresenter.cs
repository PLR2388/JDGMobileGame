using JDG.Application.Services;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Presenter for displaying dialog boxes and message boxes (MVP pattern).
/// Replaces UIManager's message box responsibility.
/// Part of Phase 5 - UIManager decomposition.
/// Phase 34: Updated to use ILocalizationService instead of LocalizationSystem.Instance.
/// Phase 40: Updated to use IDialogService instead of MessageBox.Instance.
///
/// Note: This presenter is in the default assembly during migration.
/// It will be moved to JDG.Presentation once dependencies are refactored.
/// </summary>
public class DialogPresenter
{
    private readonly Transform _canvas;
    private readonly ILocalizationService _localizationService;
    private readonly IDialogService _dialogService;

    public DialogPresenter(Transform canvas, ILocalizationService localizationService, IDialogService dialogService)
    {
        _canvas = canvas;
        _localizationService = localizationService;
        _dialogService = dialogService;
    }

    /// <summary>
    /// Displays a pause menu with the given action.
    /// </summary>
    /// <param name="onPositiveAction">Action to execute when user confirms.</param>
    public void ShowPauseMenu(UnityAction onPositiveAction)
    {
        var config = new MessageBoxConfig(
            _localizationService.GetLocalizedValue(LocalizationKeys.PAUSE_TITLE),
            _localizationService.GetLocalizedValue(LocalizationKeys.PAUSE_MESSAGE),
            showPositiveButton: true,
            showNegativeButton: true,
            positiveAction: onPositiveAction
        );

        // Phase 40: Use IDialogService instead of MessageBox.Instance
        _dialogService.ShowMessageBoxLegacy(_canvas, config);
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
        UnityAction onPositiveAction = null,
        UnityAction onNegativeAction = null)
    {
        var config = new MessageBoxConfig(
            title,
            message,
            showPositiveButton: onPositiveAction != null,
            showNegativeButton: onNegativeAction != null,
            showOkButton: onPositiveAction == null && onNegativeAction == null,
            positiveAction: onPositiveAction,
            negativeAction: onNegativeAction
        );

        // Phase 40: Use IDialogService instead of MessageBox.Instance
        _dialogService.ShowMessageBoxLegacy(_canvas, config);
    }

    /// <summary>
    /// Displays a warning message box.
    /// </summary>
    /// <param name="message">Warning message to display.</param>
    public void ShowWarning(string message)
    {
        var config = new MessageBoxConfig(
            _localizationService.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
            message,
            showOkButton: true
        );

        // Phase 40: Use IDialogService instead of MessageBox.Instance
        _dialogService.ShowMessageBoxLegacy(_canvas, config);
    }
}
