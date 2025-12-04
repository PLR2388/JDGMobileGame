using System.Collections.Generic;
using _Scripts.Units.Invocation;
using Cards;
using UnityEngine;
using UnityEngine.Events;

namespace JDG.Presentation.Presenters
{
    /// <summary>
    /// Presenter for card selection UI (MVP pattern).
    /// Replaces UIManager's opponent selection responsibility.
    /// Part of Phase 5 - UIManager decomposition.
    /// </summary>
    public class CardSelectorPresenter
    {
        private readonly Transform _canvas;
        private readonly GameObject _nextPhaseButton;

        public CardSelectorPresenter(Transform canvas, GameObject nextPhaseButton)
        {
            _canvas = canvas;
            _nextPhaseButton = nextPhaseButton;
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
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.CARDS_SELECTOR_TITLE_CHOOSE_OPPONENT),
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

            CardSelector.Instance.CreateCardSelection(_canvas, config);
        }

        private void ShowNoTargetsWarning()
        {
            var config = new MessageBoxConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_TITLE),
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.WARNING_CANNOT_ATTACK_MESSAGE),
                showOkButton: true
            );

            MessageBox.Instance.CreateMessageBox(_canvas, config);
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
