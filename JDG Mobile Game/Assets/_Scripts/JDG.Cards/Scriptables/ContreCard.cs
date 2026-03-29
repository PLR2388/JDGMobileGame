using Cards;
using UnityEngine;

namespace _Scripts.Scriptables
{
    /// <summary>
    /// Represents a "Contre" type card in the game, which extends the base functionality of the Card class.
    /// </summary>
    [CreateAssetMenu(fileName = "NewContreCard", menuName = "ContreCard")]
    public class ContreCard : Card
    {
        /// <summary>
        /// Initialization method called when an instance of this class is created.
        /// Sets the card type to "Contre".
        /// </summary>
        private void Awake()
        {
            type = CardType.Contre;
        }
    }
}