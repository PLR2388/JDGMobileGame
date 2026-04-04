using System;
using _Scripts.Cards.InvocationCards;
using JDG.Application;
using JDG.Application.Services;
using JDG.Domain.Events;
using JDG.Infrastructure.Cards;
using OnePlayer;
using VContainer;

namespace Cards.InvocationCards
{
    /// <summary>
    /// Tutorial-specific invocation functions.
    /// Phase 6: Updated to use CardPlacementService like InvocationFunctions.
    /// Phase 17-18: Removed CardManager singleton dependency via ICardCollectionService.
    /// Phase 24-25: Removed ServiceLocator, using VContainer DI.
    /// Phase 34: Uses ILocalizationService instead of LocalizationSystem.Instance.
    /// Phase 35: Uses IDialogService instead of MessageBox.Instance.
    /// Phase 109: Migrated to EventBus - removed static event subscription.
    /// </summary>
    public class TutoInvocationFunctions : InvocationFunctions
    {
        // Phase 24-25: Injected via VContainer
        private ICardCollectionService _cardCollectionService;
        // Note: _dialogService is inherited from InvocationFunctions (Phase 35)

        // Phase 109: EventBus subscription for card play
        private IEventBus _tutoEventBus;
        private IDisposable _tutoInvocationPlaySubscription;

        /// <summary>
        /// VContainer method injection for dependencies.
        /// Calls base class Construct to inject base dependencies.
        /// Phase 24-25: Inject ICardCollectionService instead of ServiceLocator.
        /// Phase 34: Inject ILocalizationService for localized strings.
        /// Phase 35: Inject IDialogService for dialog display.
        /// </summary>
        [Inject]
        public new void Construct(
            ICardPlacementService cardPlacementService,
            ICardCollectionService cardCollectionService,
            JDG.Application.IEventBus eventBus,
            ILocalizationService localizationService,
            IDialogService dialogService)
        {
            // Call base class to inject base dependencies (including localizationService and dialogService)
            base.Construct(cardPlacementService, eventBus, localizationService, dialogService);
            _cardCollectionService = cardCollectionService;
            _tutoEventBus = eventBus;
        }

        private void Start()
        {
            // Phase 109: Subscribe to EventBus instead of static event
            _tutoInvocationPlaySubscription = _tutoEventBus?.Subscribe<InvocationCardPlayRequestedEvent>(OnTutoInvocationCardPlayRequested);
        }

        private void OnDestroy()
        {
            _tutoInvocationPlaySubscription?.Dispose();
        }

        /// <summary>
        /// Tutorial-specific event handler for invocation card play.
        /// Phase 109: Replaces static UnityEvent listener.
        /// </summary>
        private void OnTutoInvocationCardPlayRequested(InvocationCardPlayRequestedEvent evt)
        {
            if (evt.InvocationCard is InGameInvocationCard invocationCard)
            {
                PutInvocationCard(invocationCard);
            }
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
        /// Phase 35: Uses injected _dialogService instead of MessageBox.Instance.
        /// </summary>
        /// <param name="invocationCard">The invocation card whose effect should be applied.</param>
        private void ApplyTutorialSpecificEffect(InGameInvocationCard invocationCard)
        {
            if (invocationCard.Title == CardNameMappings.CardNameMap[CardNames.ClichéRaciste])
            {
                var cardName = CardNameMappings.CardNameMap[CardNames.Tentacules];
                // Phase 24-25: Use injected _cardCollectionService
                var playerCards = _cardCollectionService.GetCurrentPlayerCards();
                // Phase 34: Use inherited _localizationService from base class
                // Phase 35: Use injected _dialogService instead of MessageBox.Instance
                var options = new MessageBoxOptions
                {
                    Title = _localizationService.GetLocalizedValue(LocalizationKeys.QUESTION_TITLE),
                    Message = string.Format(
                        _localizationService.GetLocalizedValue(LocalizationKeys.QUESTION_INVOKE_SPECIFIC_CARD_MESSAGE),
                        cardName
                    ),
                    ShowOkButton = true,
                    OnOk = () =>
                    {
                        InGameInvocationCard card = playerCards.Deck.Find(card => card.Title == cardName) as InGameInvocationCard;
                        playerCards.Deck.Remove(card);
                        playerCards.InvocationCards.Add(card);
                        // Phase 122: Publish via EventBus
                        _tutoEventBus?.Publish(new HighlightRequestedEvent { Element = (int)HighlightElement.InHandButton, IsActivated = true });
                    }
                };
                _dialogService.ShowMessageBox(canvas, options);
            }
        }
    }
}