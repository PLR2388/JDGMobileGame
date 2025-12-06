using _Scripts.Cards.InvocationCards;
using _Scripts.Units.Invocation;
using JDG.Infrastructure.DI;
using OnePlayer;

namespace Cards.InvocationCards
{
    /// <summary>
    /// Tutorial-specific invocation functions.
    /// Phase 6: Updated to use CardPlacementService like InvocationFunctions.
    /// Phase 17-18: Removed CardManager singleton dependency via ICardCollectionService.
    /// </summary>
    public class TutoInvocationFunctions : InvocationFunctions
    {
        // Phase 6: Use service for business logic
        private ICardPlacementService TutoCardPlacementService => ServiceLocator.Get<ICardPlacementService>();

        // Phase 17-18: Use service for card collection access
        private ICardCollectionService CardCollectionService => ServiceLocator.Get<ICardCollectionService>();

        private void Start()
        {
            InGameMenuScript.InvocationCardEvent.AddListener(PutInvocationCard);
        }

        /// <summary>
        /// Places the invocation card on the field and applies its effect.
        /// Phase 6: Delegates to CardPlacementService, then applies tutorial-specific logic.
        /// </summary>
        /// <param name="invocationCard">The invocation card to place on the field.</param>
        private void PutInvocationCard(InGameInvocationCard invocationCard)
        {
            // Place card using service
            bool success = TutoCardPlacementService.PlaceInvocationCard(invocationCard, canvas);

            if (success)
            {
                // Apply tutorial-specific effect after placement
                ApplyTutorialSpecificEffect(invocationCard);
            }
        }

        /// <summary>
        /// Applies tutorial-specific effect for certain cards.
        /// </summary>
        /// <param name="invocationCard">The invocation card whose effect should be applied.</param>
        private void ApplyTutorialSpecificEffect(InGameInvocationCard invocationCard)
        {
            if (invocationCard.Title == CardNameMappings.CardNameMap[CardNames.ClichéRaciste])
            {
                var cardName = CardNameMappings.CardNameMap[CardNames.Tentacules];
                // Phase 17-18: Use ICardCollectionService instead of CardManager.Instance
                var playerCards = CardCollectionService.GetCurrentPlayerCards();
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