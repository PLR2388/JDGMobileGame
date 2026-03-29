using System;
using Cards;
using Cards.EffectCards;
using Cards.EquipmentCards;
using Cards.FieldCards;
using Cards.InvocationCards;
using JDG.Application;
using JDG.Application.Cards;
using JDG.Application.Services;

namespace JDG.Infrastructure.Cards
{
/// <summary>
/// Provides functionality to create specific instances of InGameCard based on the provided card type.
/// Phase 24-25: Updated to accept dependencies for InGameInvocationCard constructor.
/// Phase 42ag: IAbilityProvider is now required (legacy AbilityLibrary removed).
/// Phase 48: Added card-type ability providers for Field, Equipment, and Effect cards.
/// Phase 49: Implements ICardFactory for complete abstraction.
/// Phase 56: Added IConditionProvider for InGameInvocationCard conditions.
/// </summary>
public class CardFactory : ICardFactory
{
    private readonly IEventBus _eventBus;
    private readonly ICardCollectionProvider _cardCollectionService;
    private readonly IAbilityProvider _abilityProvider;
    private readonly IFieldAbilityProvider _fieldAbilityProvider;
    private readonly IEquipmentAbilityProvider _equipmentAbilityProvider;
    private readonly IEffectAbilityProvider _effectAbilityProvider;
    private readonly IConditionProvider _conditionProvider;

    /// <summary>
    /// Creates a new CardFactory with injected dependencies.
    /// Phase 49: Added for ICardFactory implementation.
    /// Phase 56: Added IConditionProvider for InGameInvocationCard conditions.
    /// Phase 61: Made all providers required (removed fallback patterns in InGameCard classes).
    /// </summary>
    public CardFactory(
        IEventBus eventBus,
        ICardCollectionProvider cardCollectionService,
        IAbilityProvider abilityProvider,
        IFieldAbilityProvider fieldAbilityProvider,
        IEquipmentAbilityProvider equipmentAbilityProvider,
        IEffectAbilityProvider effectAbilityProvider,
        IConditionProvider conditionProvider)
    {
        _eventBus = eventBus;
        _cardCollectionService = cardCollectionService;
        _abilityProvider = abilityProvider;
        _fieldAbilityProvider = fieldAbilityProvider;
        _equipmentAbilityProvider = equipmentAbilityProvider;
        _effectAbilityProvider = effectAbilityProvider;
        _conditionProvider = conditionProvider;
    }

    /// <summary>
    /// Default constructor for backward compatibility when using static method.
    /// Phase 49: Kept for legacy code that uses static CreateInGameCard.
    /// Phase 61: Only use for static method calls - instance methods require provider injection.
    /// </summary>
    [System.Obsolete("Use the constructor with dependencies for proper DI. This exists only for static CreateInGameCard calls.")]
    public CardFactory()
    {
    }

    #region ICardFactory Implementation

    /// <inheritdoc />
    public IInGameCard CreateCard(object card, JDG.Domain.CardOwner owner)
    {
        var legacyOwner = (CardOwner)(int)owner;
        return CreateInGameCard((Card)card, legacyOwner, _eventBus, _cardCollectionService, _abilityProvider,
            _fieldAbilityProvider, _equipmentAbilityProvider, _effectAbilityProvider, _conditionProvider);
    }

    /// <inheritdoc />
    public IInGameInvocationCard CreateInvocationCard(object invocationCard, JDG.Domain.CardOwner owner)
    {
        var legacyOwner = (CardOwner)(int)owner;
        return new InGameInvocationCard((InvocationCard)invocationCard, legacyOwner, _eventBus, _cardCollectionService, _abilityProvider, _conditionProvider);
    }

    /// <inheritdoc />
    public IInGameEffectCard CreateEffectCard(object effectCard, JDG.Domain.CardOwner owner)
    {
        var legacyOwner = (CardOwner)(int)owner;
        return new InGameEffectCard((EffectCard)effectCard, legacyOwner, _effectAbilityProvider);
    }

    /// <inheritdoc />
    public IInGameFieldCard CreateFieldCard(object fieldCard, JDG.Domain.CardOwner owner)
    {
        var legacyOwner = (CardOwner)(int)owner;
        return new InGameFieldCard((FieldCard)fieldCard, legacyOwner, _fieldAbilityProvider);
    }

    /// <inheritdoc />
    public IInGameEquipmentCard CreateEquipmentCard(object equipmentCard, JDG.Domain.CardOwner owner)
    {
        var legacyOwner = (CardOwner)(int)owner;
        return new InGameEquipmentCard((EquipmentCard)equipmentCard, legacyOwner, _equipmentAbilityProvider);
    }

    #endregion

    /// <summary>
    /// Creates an instance of InGameCard based on the type and owner of the provided card.
    /// Phase 24-25: Added eventBus and cardCollectionService parameters for InGameInvocationCard dependency injection.
    /// Phase 42ag: IAbilityProvider is now required (legacy AbilityLibrary removed).
    /// Phase 48: Added providers for Field, Equipment, and Effect cards.
    /// Phase 56: Added IConditionProvider for InGameInvocationCard conditions.
    /// Phase 61: Made all providers required (removed fallback patterns in InGameCard classes).
    /// </summary>
    /// <param name="card">The base card for which the InGameCard is to be created.</param>
    /// <param name="cardOwner">The owner of the card.</param>
    /// <param name="eventBus">EventBus for publishing domain events (required).</param>
    /// <param name="cardCollectionService">Service for accessing player cards (required).</param>
    /// <param name="abilityProvider">Provider for invocation abilities (required).</param>
    /// <param name="fieldAbilityProvider">Provider for field abilities (required).</param>
    /// <param name="equipmentAbilityProvider">Provider for equipment abilities (required).</param>
    /// <param name="effectAbilityProvider">Provider for effect abilities (required).</param>
    /// <param name="conditionProvider">Provider for conditions (required).</param>
    /// <returns>An instance of a specific InGameCard subtype based on the card provided.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an unsupported card type is provided.</exception>
    public static InGameCard CreateInGameCard(
        Card card,
        CardOwner cardOwner,
        IEventBus eventBus,
        ICardCollectionProvider cardCollectionService,
        IAbilityProvider abilityProvider,
        IFieldAbilityProvider fieldAbilityProvider,
        IEquipmentAbilityProvider equipmentAbilityProvider,
        IEffectAbilityProvider effectAbilityProvider,
        IConditionProvider conditionProvider)
    {
        return card switch
        {
            EquipmentCard equipmentCard => new InGameEquipmentCard(equipmentCard, cardOwner, equipmentAbilityProvider),
            EffectCard effectCard => new InGameEffectCard(effectCard, cardOwner, effectAbilityProvider),
            InvocationCard invocationCard => new InGameInvocationCard(invocationCard, cardOwner, eventBus, cardCollectionService, abilityProvider, conditionProvider),
            FieldCard fieldCard => new InGameFieldCard(fieldCard, cardOwner, fieldAbilityProvider),
            _ => throw new InvalidOperationException("Unsupported card type provided.")
        };
    }
}
}