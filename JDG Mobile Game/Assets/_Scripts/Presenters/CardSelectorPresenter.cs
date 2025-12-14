using System.Collections.Generic;
using _Scripts.Units.Invocation;
using Cards;
using JDG.Application.Services;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Presenter for card selection UI (MVP pattern).
/// Replaces UIManager's opponent selection responsibility.
/// Part of Phase 5 - UIManager decomposition.
/// Phase 34: Updated to use ILocalizationService instead of LocalizationSystem.Instance.
/// Phase 40: Updated to use IDialogService instead of CardSelector.Instance/MessageBox.Instance.
///
/// Note: This presenter is in the default assembly because it depends on legacy types
/// (InGameCard, InGameInvocationCard). It will be moved to JDG.Presentation once
/// these types are refactored.
/// </summary>
public class CardSelectorPresenter
{
    private readonly Transform _canvas;
    private readonly GameObject _nextPhaseButton;
    private readonly ILocalizationService _localizationService;
    private readonly IDialogService _dialogService;

    public CardSelectorPresenter(Transform canvas, GameObject nextPhaseButton, ILocalizationService localizationService, IDialogService dialogService)
    {
        _canvas = canvas;
        _nextPhaseButton = nextPhaseButton;
        _localizationService = localizationService;
        _dialogService = dialogService;
    }

    /// <summary>
    /// Displays opponent selection UI or a warning if no opponents are available.
    /// </summary>
    /// <param name="availableTargets">List of cards that can be targeted.</param>
    /// <param name="onCardSelected">Action to execute when a card is selected.</param>
    /// <param name="onCancelled">Action to execute when selection is cancelled.</param>
    public void ShowOpponentSelector(
        List<InGameCard> availableTargets,
        UnityAction<InGameInvocationCard> onCardSelected,
        UnityAction onCancelled)
    {
        if (_nextPhaseButton != null)
        {
            _nextPhaseButton.SetActive(false);
        }

        if (availableTargets != null && availableTargets.Count > 0)
        {
            ShowCardSelection(availableTargets, onCardSelected, onCancelled);
        }
        else
        {
            ShowNoTargetsWarning();
        }
    }

    private void ShowCardSelection(
        List<InGameCard> cards,
        UnityAction<InGameInvocationCard> onCardSelected,
        UnityAction onCancelled)
    {
        var config = new CardSelectorConfig(
            _localizationService.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOOSE_OPPONENT),
            cards,
            showNegativeButton: true,
            showPositiveButton: true,
            positiveAction: (invocationCard) =>
            {
                onCardSelected?.Invoke(invocationCard as InGameInvocationCard);
                EnableNextPhaseButton();
            },
            negativeAction: () =>
            {
                onCancelled?.Invoke();
                EnableNextPhaseButton();
            }
        );

        // Phase 40: Use IDialogService instead of CardSelector.Instance
        _dialogService.ShowCardSelectorLegacy(_canvas, config);
    }

    private void ShowNoTargetsWarning()
    {
        var config = new MessageBoxConfig(
            _localizationService.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
            _localizationService.GetLocalizedValue(LocalizationKeys.WARNING_CANNOT_ATTACK_MESSAGE),
            showOkButton: true
        );

        // Phase 40: Use IDialogService instead of MessageBox.Instance
        _dialogService.ShowMessageBoxLegacy(_canvas, config);
    }

    private void EnableNextPhaseButton()
    {
        if (_nextPhaseButton != null)
        {
            _nextPhaseButton.SetActive(true);
        }
    }
}
