using JDG.Application.Cards;
using UnityEngine;
using DomainCardOwner = JDG.Domain.CardOwner;
using DomainCardType = JDG.Domain.Enums.CardType;

namespace Cards
{
    /// <summary>
    /// Represents a card used in the game with various properties and attributes.
    /// Phase 39: Implements IInGameCard to enable presenter migration to JDG.Presentation.
    /// </summary>
    public class InGameCard : IInGameCard
    {
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
        /// Gets the title of the card.
        /// </summary>
        public string Title => title;

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
        /// </summary>
        string IInGameCard.Description => Description;

        /// <summary>
        /// Gets the detailed description or lore of the card.
        /// Phase 39: Explicit implementation for IInGameCard interface.
        /// </summary>
        string IInGameCard.DetailedDescription => DetailedDescription;

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

        #endregion
    }
}