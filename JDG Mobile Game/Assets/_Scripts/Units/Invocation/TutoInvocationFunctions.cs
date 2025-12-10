using _Scripts.Cards.InvocationCards;
using _Scripts.Units.Invocation;
using JDG.Application;
using OnePlayer;
using VContainer;

namespace Cards.InvocationCards
{
    /// <summary>
    /// Tutorial-specific invocation functions.
    /// Phase 6: Updated to use CardPlacementService like InvocationFunctions.
    /// Phase 17-18: Removed CardManager singleton dependency via ICardCollectionService.
    /// Phase 24-25: Removed ServiceLocator, using VContainer DI.
    /// </summary>
    public class TutoInvocationFunctions : InvocationFunctions
    {
        // Phase 24-25: Injected via VContainer
        private ICardCollectionService _cardCollectionService;

        /// <summary>
        /// VContainer method injection for dependencies.
        /// Calls base class Construct to inject base dependencies.
        /// Phase 24-25: Inject ICardCollectionService instead of ServiceLocator.
        /// </summary>
        [Inject]
        public void Construct(ICardPlacementService cardPlacementService, ICardCollectionService cardCollectionService, JDG.Application.IEventBus eventBus)
        {
            // Call base class to inject base dependencies
            base.Construct(cardPlacementService, eventBus);
            _cardCollectionService = cardCollectionService;
        }

        private void Start()
        {
            InGameMenuScript.InvocationCardEvent.AddListener(PutInvocationCard);
        }

        /// <summary>
        /// Places the invocation card on the field and applies its effect.
        /// Phase 6: Delegates to CardPlacementService, then applies tutorial-specific logic.
        /// Phase 24-25: Uses inherited _cardPlacementService from base class.
        /// </summary>
        /// <param name="invocationCard">The invocation card to place on the field.</param>
        private void PutInvocationCard(InGameInvocationCard invocationCard)
        {
            // Place card using inherited service from base class
            bool success = _cardPlacementService.PlaceInvocationCard(invocationCard, canvas);

            if (success)
            {
                // Apply tutorial-specific effect after placement
                ApplyTutorialSpecificEffect(invocationCard);
            }
        }

        /// <summary>
        /// Applies tutorial-specific effect for certain cards.
        /// Phase 24-25: Uses injected _cardCollectionService.
        /// </summary>
        /// <param name="invocationCard">The invocation card whose effect should be applied.</param>
        private void ApplyTutorialSpecificEffect(InGameInvocationCard invocationCard)
        {
            if (invocationCard.Title == CardNameMappings.CardNameMap[CardNames.ClichéRaciste])
            {
                var cardName = CardNameMappings.CardNameMap[CardNames.Tentacules];
                // Phase 24-25: Use injected _cardCollectionService
                var playerCards = _cardCollectionService.GetCurrentPlayerCards();
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