using UnityEngine;

namespace Cards
{
    /// <summary>
    /// Represents a card used in the game with various properties and attributes.
    /// </summary>
    public class InGameCard
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
        /// </summary>
        protected string Description;
        
        /// <summary>
        /// Detailed description or lore of the card.
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
        /// Gets the owner of the card.
        /// </summary>
        public CardOwner CardOwner { get; protected set; } = CardOwner.NotDefined;

        /// <summary>
        /// Gets the title of the card.
        /// </summary>
        public string Title => title;
        
        /// <summary>
        /// Gets the type classification of the card.
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
    }
}