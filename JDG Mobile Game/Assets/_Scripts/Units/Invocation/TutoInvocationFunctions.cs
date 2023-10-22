using _Scripts.Cards.InvocationCards;
using _Scripts.Units.Invocation;
using OnePlayer;

namespace Cards.InvocationCards
{
    public class TutoInvocationFunctions : InvocationFunctions
    {
        private void Start()
        {
            InGameMenuScript.InvocationCardEvent.AddListener(PutInvocationCard);
        }
        
        /// <summary>
        /// Places the invocation card on the field and applies its effect.
        /// </summary>
        /// <param name="invocationCard">The invocation card to place on the field.</param>

        private void PutInvocationCard(InGameInvocationCard invocationCard)
        {
            if (CanAddCardToField())
            {
                AddCardToField(invocationCard);
                ApplyCardEffect(invocationCard);
            }
        }

        /// <summary>
        /// Applies the effect of the specified invocation card.
        /// </summary>
        /// <param name="invocationCard">The invocation card whose effect should be applied.</param>
        private void ApplyCardEffect(InGameInvocationCard invocationCard)
        {
            if (invocationCard.Title == CardNameMappings.CardNameMap[CardNames.ClichéRaciste])
            {
                var cardName = CardNameMappings.CardNameMap[CardNames.Tentacules];
                var playerCards = CardManager.Instance.GetCurrentPlayerCards();
                var config = new MessageBoxConfig(
                    LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_TITLE),
                    string.Format(
                        LocalizationSystem.Instance.GetLocalizedValue(LocalizationKeys.QUESTION_INVOKE_SPECIFIC_CARD_MESSAGE),
                        cardName
                    ),
                    showOkButton: true,
                    okAction: () =>
                    {
                        InGameInvocationCard card = playerCards.Deck.Find(card => card.Title == cardName) as InGameInvocationCard;
                        playerCards.Deck.Remove(card);
                        playerCards.InvocationCards.Add(card);
                        HighLightPlane.Highlight.Invoke(HighlightElement.InHandButton, true);
                    }
                );
                MessageBox.Instance.CreateMessageBox(canvas, config);
            }
        }
    }
}