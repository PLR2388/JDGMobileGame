using Cards;
using JDG.Application.Cards;
using JDG.Application.Services;
using JDG.Domain;
using JDG.Domain.Enums;
using JDG.Infrastructure.Services;
using UnityEngine;

namespace JDG.Infrastructure.Cards
{
    /// <summary>
    /// Represents a card used in the game with various properties and attributes.
    /// Phase 39: Implements IInGameCard to enable presenter migration to JDG.Presentation.
    /// Phase 126: Added multilanguage support via ILocalizationService.
    /// Phase 166: Uses domain enums directly (legacy enum wrappers removed).
    /// </summary>
    public class InGameCard : IInGameCard
    {
        #region Static Localization Service (Phase 126)

        /// <summary>
        /// Static localization service for card text resolution.
        /// Phase 126: Enables lazy text resolution with fallback to ScriptableObject.
        /// </summary>
        private static ILocalizationService _localizationService;

        /// <summary>
        /// Sets the localization service for all InGameCard instances.
        /// Called during game initialization.
        /// Phase 126: Added for card multilanguage support.
        /// </summary>
        /// <param name="service">The localization service instance.</param>
        public static void SetLocalizationService(ILocalizationService service)
        {
            _localizationService = service;
        }

        #endregion

        /// <summary>
        /// The base card information.
        /// </summary>
        public Card BaseCard;

        /// <summary>
        /// Title of the card.
        /// </summary>
        protected string title = "";

        /// <summary>
        /// Brief description of the card.
        /// Note: Capitalized to maintain backward compatibility with subclasses.
        /// </summary>
        protected string Description;

        /// <summary>
        /// Detailed description or lore of the card.
        /// Note: Capitalized to maintain backward compatibility with subclasses.
        /// </summary>
        protected string DetailedDescription;

        /// <summary>
        /// Type classification of the card.
        /// Phase 166: Now uses JDG.Domain.Enums.CardType directly.
        /// </summary>
        protected CardType type;

        /// <summary>
        /// Material (visual) associated with the card.
        /// </summary>
        protected Material materialCard;

        /// <summary>
        /// Flag to determine if the card is a collector's item.
        /// </summary>
        protected bool collector;

        /// <summary>
        /// Gets the owner of the card.
        /// Phase 166: Now uses JDG.Domain.CardOwner directly.
        /// </summary>
        public CardOwner CardOwner { get; protected set; } = CardOwner.NotDefined;

        /// <summary>
        /// Gets the card ID for localization lookup.
        /// Generated from the ScriptableObject asset name.
        /// Phase 126: Added for card multilanguage support.
        /// </summary>
        public string CardId => CardIdGenerator.GenerateCardId(BaseCard?.name ?? title);

        /// <summary>
        /// Gets the title of the card.
        /// Phase 126: Uses lazy resolution via localization service with fallback.
        /// </summary>
        public string Title => _localizationService?.GetCardTitle(CardId) ?? title;

        /// <summary>
        /// Gets the brief description of the card.
        /// Phase 126: Uses lazy resolution via localization service with fallback.
        /// </summary>
        /// <returns>Localized description or fallback to ScriptableObject text.</returns>
        public string GetDescription() => _localizationService?.GetCardDescription(CardId) ?? Description;

        /// <summary>
        /// Gets the detailed description of the card.
        /// Phase 126: Uses lazy resolution via localization service with fallback.
        /// </summary>
        /// <returns>Localized detailed description or fallback to ScriptableObject text.</returns>
        public string GetDetailedDescription() => _localizationService?.GetCardDetailedDescription(CardId) ?? DetailedDescription;

        /// <summary>
        /// Gets the type classification of the card.
        /// Phase 166: Now returns JDG.Domain.Enums.CardType directly.
        /// </summary>
        public CardType Type => type;

        /// <summary>
        /// Gets the material (visual) associated with the card.
        /// </summary>
        public Material MaterialCard => materialCard;

        /// <summary>
        /// Gets a value indicating whether the card is a collector's item.
        /// </summary>
        public bool Collector => collector;

        #region IInGameCard Implementation (Domain Types)

        /// <summary>
        /// Gets the card owner using domain type.
        /// Phase 166: Direct pass-through, no conversion needed.
        /// </summary>
        CardOwner IInGameCard.CardOwner => CardOwner;

        /// <summary>
        /// Gets the card type using domain type.
        /// Phase 166: Direct pass-through, no conversion needed.
        /// </summary>
        CardType IInGameCard.Type => type;

        /// <summary>
        /// Gets the brief description of the card.
        /// Phase 39: Explicit implementation for IInGameCard interface.
        /// Phase 126: Uses lazy resolution via GetDescription() for localization.
        /// </summary>
        string IInGameCard.Description => GetDescription();

        /// <summary>
        /// Gets the detailed description or lore of the card.
        /// Phase 39: Explicit implementation for IInGameCard interface.
        /// Phase 126: Uses lazy resolution via GetDetailedDescription() for localization.
        /// </summary>
        string IInGameCard.DetailedDescription => GetDetailedDescription();

        /// <summary>
        /// Gets a value indicating whether the card is a collector's item.
        /// Phase 39: Explicit implementation for IInGameCard interface.
        /// </summary>
        bool IInGameCard.Collector => collector;

        /// <summary>
        /// Gets the unique identifier for the card's visual.
        /// Uses the card title as the visual ID since materials are mapped by title.
        /// Phase 39: Added for IInGameCard interface.
        /// </summary>
        public string VisualId => title;

        #endregion
    }
}
