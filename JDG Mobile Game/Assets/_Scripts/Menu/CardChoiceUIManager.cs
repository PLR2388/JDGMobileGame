using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    /// <summary>
    /// Manages the UI related to the card choice in the game menu.
    /// </summary>
    public class CardChoiceUIManager : StaticInstance<CardChoiceUIManager>
    {
        [SerializeField] private GameObject container;
        [SerializeField] private Transform canvas;

        [SerializeField] private GameObject choiceCardMenu;
        [SerializeField] private GameObject twoPlayerModeMenu;

        [SerializeField] private Text title;
        [SerializeField] private Text buttonText;

        /// <summary>
        /// Updates the title and button text based on the given player's choice.
        /// </summary>
        /// <param name="isPlayerOne">True if player one, false for player two.</param>
        public void UpdateTitleAndButtonTextForPlayer(bool isPlayerOne)
        {
            title.text = LocalizationSystem.Instance.GetLocalizedValue(isPlayerOne ? LocalizationKeys.CARD_CHOICE_TITLE_PLAYER_ONE : LocalizationKeys.CARD_CHOICE_TITLE_PLAYER_TWO);
            buttonText.text = LocalizationSystem.Instance.GetLocalizedValue(isPlayerOne ? LocalizationKeys.CARD_CHOICE_BUTTON_PLAYER_ONE : LocalizationKeys.CARD_CHOICE_BUTTON_PLAYER_TWO);
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
        /// </summary>
        /// <param name="remainedCards">The number of cards remaining to be chosen.</param>
        public void DisplayMessageBox(int remainedCards)
        {
            var config = new MessageBoxConfig(
                LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.MODIFY_DECK_TITLE),
                string.Format(LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.MODIFY_DECK_MESSAGE), remainedCards),
                showOkButton: true
            );
            MessageBox.Instance.CreateMessageBox(canvas, config);
        }
    }
}