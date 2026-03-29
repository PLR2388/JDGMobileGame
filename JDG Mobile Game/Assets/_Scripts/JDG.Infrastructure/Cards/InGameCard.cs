using Cards;
using JDG.Application.Cards;
using JDG.Application.Services;
using JDG.Infrastructure.Services;
using UnityEngine;
using DomainCardOwner = JDG.Domain.CardOwner;
using DomainCardType = JDG.Domain.Enums.CardType;

namespace JDG.Infrastructure.Cards
{
    /// <summary>
    /// Represents a card used in the game with various properties and attributes.
    /// Phase 39: Implements IInGameCard to enable presenter migration to JDG.Presentation.
    /// Phase 126: Added multilanguage support via ILocalizationService.
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
        /// Gets the owner of the card (legacy type).
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
        /// Gets the type classification of the card (legacy type).
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

        #region IInGameCard Explicit Implementation (Domain Types)

        /// <summary>
        /// Gets the card owner using domain type.
        /// Phase 39: Explicit implementation for IInGameCard interface.
        /// </summary>
        DomainCardOwner IInGameCard.CardOwner => ConvertToDomainOwner(CardOwner);

        /// <summary>
        /// Gets the card type using domain type.
        /// Phase 39: Explicit implementation for IInGameCard interface.
        /// </summary>
        DomainCardType IInGameCard.Type => ConvertToDomainType(type);

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

        #region Type Conversion Helpers

        /// <summary>
        /// Converts legacy CardOwner to domain CardOwner.
        /// </summary>
        private static DomainCardOwner ConvertToDomainOwner(CardOwner owner)
        {
            return owner switch
            {
                CardOwner.Player1 => DomainCardOwner.Player1,
                CardOwner.Player2 => DomainCardOwner.Player2,
                _ => DomainCardOwner.NotDefined
            };
        }

        /// <summary>
        /// Converts legacy CardType to domain CardType.
        /// </summary>
        private static DomainCardType ConvertToDomainType(CardType cardType)
        {
            return cardType switch
            {
                CardType.Invocation => DomainCardType.Invocation,
                CardType.Equipment => DomainCardType.Equipment,
                CardType.Field => DomainCardType.Field,
                CardType.Effect => DomainCardType.Effect,
                CardType.Contre => DomainCardType.Contre,
                _ => DomainCardType.Invocation // Default fallback
            };
        }

        /// <summary>
        /// Safely converts legacy CardOwner to domain CardOwner using int cast.
        /// Phase 146: Added validation to catch enum definition mismatches.
        /// </summary>
        public static DomainCardOwner SafeConvertOwner(CardOwner owner)
        {
            var intValue = (int)owner;
            if (!System.Enum.IsDefined(typeof(DomainCardOwner), intValue))
            {
                UnityEngine.Debug.LogWarning($"[InGameCard] CardOwner value {intValue} is not defined in DomainCardOwner. Using NotDefined.");
                return DomainCardOwner.NotDefined;
            }
            return (DomainCardOwner)intValue;
        }

        /// <summary>
        /// Safely converts legacy CardFamily to domain CardFamily using int cast.
        /// Phase 146: Added validation to catch enum definition mismatches.
        /// </summary>
        public static JDG.Domain.Enums.CardFamily SafeConvertFamily(CardFamily family)
        {
            var intValue = (int)family;
            if (!System.Enum.IsDefined(typeof(JDG.Domain.Enums.CardFamily), intValue))
            {
                UnityEngine.Debug.LogWarning($"[InGameCard] CardFamily value {intValue} is not defined in domain CardFamily. Using default.");
                return JDG.Domain.Enums.CardFamily.None;
            }
            return (JDG.Domain.Enums.CardFamily)intValue;
        }

        #endregion
    }
}