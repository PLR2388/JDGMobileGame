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
/// Phase 42ag: IAbilityProvider is now required (legacy AbilityLibrary removed).
/// Phase 48: Added card-type ability providers for Field, Equipment, and Effect cards.
/// </summary>
public class CardFactory
{
    /// <summary>
    /// Creates an instance of InGameCard based on the type and owner of the provided card.
    /// Phase 24-25: Added eventBus and cardCollectionService parameters for InGameInvocationCard dependency injection.
    /// Phase 42ag: IAbilityProvider is now required (legacy AbilityLibrary removed).
    /// Phase 48: Added optional providers for Field, Equipment, and Effect cards.
    /// </summary>
    /// <param name="card">The base card for which the InGameCard is to be created.</param>
    /// <param name="cardOwner">The owner of the card.</param>
    /// <param name="eventBus">EventBus for publishing domain events (required for InvocationCard).</param>
    /// <param name="cardCollectionService">Service for accessing player cards (required for InvocationCard).</param>
    /// <param name="abilityProvider">Provider for invocation abilities (required for InvocationCard).</param>
    /// <param name="fieldAbilityProvider">Optional provider for field abilities.</param>
    /// <param name="equipmentAbilityProvider">Optional provider for equipment abilities.</param>
    /// <param name="effectAbilityProvider">Optional provider for effect abilities.</param>
    /// <returns>An instance of a specific InGameCard subtype based on the card provided.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an unsupported card type is provided.</exception>
    public static InGameCard CreateInGameCard(
        Card card,
        CardOwner cardOwner,
        IEventBus eventBus,
        ICardCollectionService cardCollectionService,
        IAbilityProvider abilityProvider,
        IFieldAbilityProvider fieldAbilityProvider = null,
        IEquipmentAbilityProvider equipmentAbilityProvider = null,
        IEffectAbilityProvider effectAbilityProvider = null)
    {
        return card switch
        {
            EquipmentCard equipmentCard => new InGameEquipmentCard(equipmentCard, cardOwner, equipmentAbilityProvider),
            EffectCard effectCard => new InGameEffectCard(effectCard, cardOwner, effectAbilityProvider),
            InvocationCard invocationCard => new InGameInvocationCard(invocationCard, cardOwner, eventBus, cardCollectionService, abilityProvider),
            FieldCard fieldCard => new InGameFieldCard(fieldCard, cardOwner, fieldAbilityProvider),
            _ => throw new InvalidOperationException("Unsupported card type provided.")
        };
    }
}