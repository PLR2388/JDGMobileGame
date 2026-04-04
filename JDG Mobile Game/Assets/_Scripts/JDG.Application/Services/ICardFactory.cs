using JDG.Application.Cards;
using JDG.Domain;

namespace JDG.Application.Services
{
    /// <summary>
    /// Factory interface for creating in-game card instances.
    /// Phase 49: Created to abstract card creation and enable testability.
    /// </summary>
    public interface ICardFactory
    {
        /// <summary>
        /// Creates an in-game card from a card definition.
        /// </summary>
        /// <param name="card">The card definition object.</param>
        /// <param name="owner">The owner of the card.</param>
        /// <returns>An in-game card instance.</returns>
        IInGameCard CreateCard(object card, CardOwner owner);

        /// <summary>
        /// Creates an in-game invocation card.
        /// </summary>
        /// <param name="invocationCard">The invocation card definition.</param>
        /// <param name="owner">The owner of the card.</param>
        /// <returns>An in-game invocation card instance.</returns>
        IInGameInvocationCard CreateInvocationCard(object invocationCard, CardOwner owner);

        /// <summary>
        /// Creates an in-game effect card.
        /// </summary>
        /// <param name="effectCard">The effect card definition.</param>
        /// <param name="owner">The owner of the card.</param>
        /// <returns>An in-game effect card instance.</returns>
        IInGameEffectCard CreateEffectCard(object effectCard, CardOwner owner);

        /// <summary>
        /// Creates an in-game field card.
        /// </summary>
        /// <param name="fieldCard">The field card definition.</param>
        /// <param name="owner">The owner of the card.</param>
        /// <returns>An in-game field card instance.</returns>
        IInGameFieldCard CreateFieldCard(object fieldCard, CardOwner owner);

        /// <summary>
        /// Creates an in-game equipment card.
        /// </summary>
        /// <param name="equipmentCard">The equipment card definition.</param>
        /// <param name="owner">The owner of the card.</param>
        /// <returns>An in-game equipment card instance.</returns>
        IInGameEquipmentCard CreateEquipmentCard(object equipmentCard, CardOwner owner);
    }
}
