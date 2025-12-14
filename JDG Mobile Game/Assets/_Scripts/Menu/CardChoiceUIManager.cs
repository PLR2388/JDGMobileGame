using JDG.Application;
using JDG.Application.Services;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Menu
{
    /// <summary>
    /// Manages the UI related to the card choice in the game menu.
    /// Phase 34: Uses ILocalizationService instead of LocalizationSystem.Instance.
    /// Phase 35: Uses IDialogService instead of MessageBox.Instance.
    /// </summary>
    public class CardChoiceUIManager : StaticInstance<CardChoiceUIManager>
    {
        [SerializeField] private GameObject container;
        [SerializeField] private Transform canvas;

        [SerializeField] private GameObject choiceCardMenu;
        [SerializeField] private GameObject twoPlayerModeMenu;

        [SerializeField] private Text title;
        [SerializeField] private Text buttonText;

        // Phase 34: Injected via VContainer
        private ILocalizationService _localizationService;

        // Phase 35: Injected via VContainer
        private IDialogService _dialogService;

        [Inject]
        public void Construct(ILocalizationService localizationService, IDialogService dialogService)
        {
            _localizationService = localizationService;
            _dialogService = dialogService;
        }

        /// <summary>
        /// Updates the title and button text based on the given player's choice.
        /// Phase 34: Uses injected ILocalizationService.
        /// </summary>
        /// <param name="isPlayerOne">True if player one, false for player two.</param>
        public void UpdateTitleAndButtonTextForPlayer(bool isPlayerOne)
        {
            title.text = _localizationService.GetLocalizedValue(isPlayerOne ? LocalizationKeys.CARD_CHOICE_TITLE_PLAYER_ONE : LocalizationKeys.CARD_CHOICE_TITLE_PLAYER_TWO);
            buttonText.text = _localizationService.GetLocalizedValue(isPlayerOne ? LocalizationKeys.CARD_CHOICE_BUTTON_PLAYER_ONE : LocalizationKeys.CARD_CHOICE_BUTTON_PLAYER_TWO);
        }

        /// <summary>
        /// Shows or hides the card choice menu based on the given state.
        /// </summary>
        /// <param name="isActive">True to show the menu, false to hide.</param>
        public void ShowChoiceCardMenu(bool isActive)
        {
            choiceCardMenu.SetActive(isActive);
        }

        /// <summary>
        /// Shows or hides the two player mode menu based on the given state.
        /// </summary>
        /// <param name="isActive">True to show the menu, false to hide.</param>
        public void ShowTwoPlayerModeMenu(bool isActive)
        {
            twoPlayerModeMenu.SetActive(isActive);
        }

        /// <summary>
        /// Displays a message box with the number of remained cards.
        /// Phase 34: Uses injected ILocalizationService.
        /// Phase 35: Uses injected IDialogService.
        /// </summary>
        /// <param name="remainedCards">The number of cards remaining to be chosen.</param>
        public void DisplayMessageBox(int remainedCards)
        {
            _dialogService.ShowMessageBox(canvas, new MessageBoxOptions
            {
                Title = _localizationService.GetLocalizedValue(LocalizationKeys.MODIFY_DECK_TITLE),
                Message = string.Format(_localizationService.GetLocalizedValue(LocalizationKeys.MODIFY_DECK_MESSAGE), remainedCards),
                ShowOkButton = true
            });
        }
    }
}