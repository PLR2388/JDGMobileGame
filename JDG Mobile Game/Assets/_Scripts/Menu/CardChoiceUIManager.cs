using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class CardChoiceUIManager : StaticInstance<CardChoiceUIManager>
    {
        [SerializeField] private GameObject container;
        [SerializeField] private Transform canvas;

        [SerializeField] private GameObject choiceCardMenu;
        [SerializeField] private GameObject twoPlayerModeMenu;

        [SerializeField] private Text title;
        [SerializeField] private Text buttonText;

        private CardChoice cardChoice;

        private void Start()
        {
            cardChoice = GetComponent<CardChoice>();
            if(cardChoice == null)
            {
                Debug.LogError("CardChoice component not found on the same GameObject!");
            }
        }

        public void UpdateTitleAndButtonTextForPlayer(bool isPlayerOne)
        {
            title.text = LocalizationSystem.Instance.GetLocalizedValue(isPlayerOne ? LocalizationKeys.CARD_CHOICE_TITLE_PLAYER_ONE : LocalizationKeys.CARD_CHOICE_TITLE_PLAYER_TWO);
            buttonText.text = LocalizationSystem.Instance.GetLocalizedValue(isPlayerOne ? LocalizationKeys.CARD_CHOICE_BUTTON_PLAYER_ONE : LocalizationKeys.CARD_CHOICE_BUTTON_PLAYER_TWO);
        }

        public void ShowChoiceCardMenu(bool isActive)
        {
            choiceCardMenu.SetActive(isActive);
        }

        public void ShowTwoPlayerModeMenu(bool isActive)
        {
            twoPlayerModeMenu.SetActive(isActive);
        }

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