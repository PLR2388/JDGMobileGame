using System;
using _Scripts.Units.Invocation;
using Cards;
using Cards.EffectCards;
using Cards.EquipmentCards;
using Cards.FieldCards;
using Cards.InvocationCards;
using JDG.Application;

/// <summary>
/// Provides functionality to create specific instances of InGameCard based on the provided card type.
/// Phase 24-25: Updated to accept dependencies for InGameInvocationCard constructor.
/// Phase 7: Added IAbilityProvider for ability system migration.
/// </summary>
public class CardFactory
{
    /// <summary>
    /// Creates an instance of InGameCard based on the type and owner of the provided card.
    /// Phase 24-25: Added eventBus and cardCollectionService parameters for InGameInvocationCard dependency injection.
    /// Phase 7: Added abilityProvider parameter for ability system migration.
    /// </summary>
    /// <param name="card">The base card for which the InGameCard is to be created.</param>
    /// <param name="cardOwner">The owner of the card.</param>
    /// <param name="eventBus">EventBus for publishing domain events (required for InvocationCard).</param>
    /// <param name="cardCollectionService">Service for accessing player cards (required for InvocationCard).</param>
    /// <param name="abilityProvider">Provider for abilities (optional, falls back to AbilityLibrary.Instance if null).</param>
    /// <returns>An instance of a specific InGameCard subtype based on the card provided.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an unsupported card type is provided.</exception>
    public static InGameCard CreateInGameCard(
        Card card,
        CardOwner cardOwner,
        IEventBus eventBus = null,
        ICardCollectionService cardCollectionService = null,
        IAbilityProvider abilityProvider = null)
    {
        return card switch
        {
            EquipmentCard equipmentCard => new InGameEquipmentCard(equipmentCard, cardOwner),
            EffectCard effectCard => new InGameEffectCard(effectCard, cardOwner),
            InvocationCard invocationCard => new InGameInvocationCard(invocationCard, cardOwner, eventBus, cardCollectionService, abilityProvider),
            FieldCard fieldCard => new InGameFieldCard(fieldCard, cardOwner),
            _ => throw new InvalidOperationException("Unsupported card type provided.")
        };
    }
}