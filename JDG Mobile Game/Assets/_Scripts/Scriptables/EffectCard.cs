using System.Collections.Generic;
using UnityEngine;

namespace Cards.EffectCards
{
    /// <summary>
    /// Represents an effect card in the game, which extends the base functionality of the Card class.
    /// </summary>
    [CreateAssetMenu(fileName = "NewEffectCard", menuName = "EffectCard")]
    public class EffectCard : Card
    {
        /// <summary>
        /// List of abilities associated with this effect card.
        /// </summary>
        public List<EffectAbilityName> EffectAbilities = new List<EffectAbilityName>();

        /// <summary>
        /// Initialization method called when an instance of this class is created.
        /// Sets the card type to "Effect".
        /// </summary>
        private void Awake()
        {
            type = CardType.Effect;
        }
    }
}