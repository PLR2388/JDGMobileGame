using JDG.Application;
using JDG.Application.Services;

namespace JDG.Infrastructure.Cards.Handlers
{
    /// <summary>
    /// Represents a base class for handling card-specific behaviors within the game.
    /// Phase 17-18: Added ICardCollectionService for card state access.
    /// Phase 34: Added ILocalizationService for localized strings.
    /// Phase 36: Added IEventBus for static UnityEvent migration.
    /// Phase 166: Moved to JDG.Infrastructure. Replaced InGameMenuScript with ICardMenuView,
    /// ICardCollectionService with ICardCollectionProvider, removed unused IPlayerStatusProvider.
    /// </summary>
    public abstract class CardHandler
    {
        /// <summary>
        /// View abstraction for card menu UI interactions.
        /// Phase 166: Replaced InGameMenuScript with ICardMenuView interface.
        /// </summary>
        protected ICardMenuView menuView;

        /// <summary>
        /// Phase 166: Service for accessing player card collections via clean interface.
        /// Replaced ICardCollectionService with ICardCollectionProvider.
        /// </summary>
        protected ICardCollectionProvider cardCollectionProvider;

        /// <summary>
        /// Phase 34: Service for localized text values.
        /// </summary>
        protected ILocalizationService localizationService;

        /// <summary>
        /// Phase 36: EventBus for publishing card play events.
        /// </summary>
        protected IEventBus eventBus;

        /// <summary>
        /// Initializes a new instance of the <see cref="CardHandler"/> class.
        /// Phase 166: Simplified constructor - removed unused IPlayerStatusProvider,
        /// replaced InGameMenuScript with ICardMenuView, ICardCollectionService with ICardCollectionProvider.
        /// </summary>
        /// <param name="menuView">The card menu view for UI interactions.</param>
        /// <param name="cardCollectionProvider">The provider for accessing player card collections.</param>
        /// <param name="localizationService">The service for localized text values.</param>
        /// <param name="eventBus">The event bus for publishing card events.</param>
        public CardHandler(
            ICardMenuView menuView,
            ICardCollectionProvider cardCollectionProvider,
            ILocalizationService localizationService,
            IEventBus eventBus)
        {
            this.menuView = menuView;
            this.cardCollectionProvider = cardCollectionProvider;
            this.localizationService = localizationService;
            this.eventBus = eventBus;
        }

        /// <summary>
        /// Provides behavior definitions when a card is interacted with or activated.
        /// Implementations should define how the UI or other game elements respond to this interaction.
        /// </summary>
        /// <param name="card">The in-game card to be handled.</param>
        public abstract void HandleCard(InGameCard card);

        /// <summary>
        /// Provides behavior definitions when a card is placed or positioned within the game.
        /// Implementations should define the game's response to placing the card, such as triggering effects.
        /// </summary>
        /// <param name="card">The in-game card that is being placed.</param>
        public abstract void HandleCardPut(InGameCard card);
    }
}
