using System;
using System.Collections.Generic;
using System.Linq;
using JDG.Application.Cards;
using JDG.Application.Services;
using UnityEngine;

namespace JDG.Presentation.Presenters
{
    /// <summary>
    /// Presenter for card selection UI (MVP pattern).
    /// Replaces UIManager's opponent selection responsibility.
    /// Part of Phase 5 - UIManager decomposition.
    /// Phase 34: Updated to use ILocalizationService instead of LocalizationSystem.Instance.
    /// Phase 40: Updated to use IDialogService instead of CardSelector.Instance/MessageBox.Instance.
    /// Phase 51: Migrated to JDG.Presentation using IInGameCard interfaces.
    /// </summary>
    public class CardSelectorPresenter
    {
        private readonly Transform _canvas;
        private readonly GameObject _nextPhaseButton;
        private readonly ILocalizationService _localizationService;
        private readonly IDialogService _dialogService;

        // Localization keys for dialog text
        private const string KEY_CHOOSE_OPPONENT = "CARDS_SELECTOR_TITLE_CHOOSE_OPPONENT";
        private const string KEY_WARNING_TITLE = "WARNING_TITLE";
        private const string KEY_WARNING_CANNOT_ATTACK = "WARNING_CANNOT_ATTACK_MESSAGE";

        public CardSelectorPresenter(
            Transform canvas,
            GameObject nextPhaseButton,
            ILocalizationService localizationService,
            IDialogService dialogService)
        {
            _canvas = canvas;
            _nextPhaseButton = nextPhaseButton;
            _localizationService = localizationService;
            _dialogService = dialogService;
        }

        /// <summary>
        /// Displays opponent selection UI or a warning if no opponents are available.
        /// Phase 51: Updated to use IInGameCard interfaces instead of concrete types.
        /// </summary>
        /// <param name="availableTargets">List of cards that can be targeted.</param>
        /// <param name="onCardSelected">Action to execute when a card is selected.</param>
        /// <param name="onCancelled">Action to execute when selection is cancelled.</param>
        public void ShowOpponentSelector(
            IReadOnlyList<IInGameCard> availableTargets,
            Action<IInGameInvocationCard> onCardSelected,
            Action onCancelled)
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
            IReadOnlyList<IInGameCard> cards,
            Action<IInGameInvocationCard> onCardSelected,
            Action onCancelled)
        {
            var options = new CardSelectorOptions
            {
                Title = _localizationService.GetLocalizedValue(KEY_CHOOSE_OPPONENT),
                Cards = cards.Cast<object>().ToList(),
                ShowPositiveButton = true,
                ShowNegativeButton = true,
                OnPositiveSingle = (selectedCard) =>
                {
                    onCardSelected?.Invoke(selectedCard as IInGameInvocationCard);
                    EnableNextPhaseButton();
                },
                OnNegative = () =>
                {
                    onCancelled?.Invoke();
                    EnableNextPhaseButton();
                }
            };

            _dialogService.ShowCardSelector(_canvas, options);
        }

        private void ShowNoTargetsWarning()
        {
            var options = new MessageBoxOptions
            {
                Title = _localizationService.GetLocalizedValue(KEY_WARNING_TITLE),
                Message = _localizationService.GetLocalizedValue(KEY_WARNING_CANNOT_ATTACK),
                ShowOkButton = true
            };

            _dialogService.ShowMessageBox(_canvas, options);
        }

        private void EnableNextPhaseButton()
        {
            if (_nextPhaseButton != null)
            {
                _nextPhaseButton.SetActive(true);
            }
        }
    }
}
