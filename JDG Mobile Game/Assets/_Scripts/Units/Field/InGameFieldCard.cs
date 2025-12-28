using System.Collections.Generic;
using System.Linq;
using Cards;
using Cards.FieldCards;
using JDG.Application.Cards;

/// <summary>
/// Represents a card on the field in the game with additional runtime behaviors.
/// Phase 49: Implements IInGameFieldCard for complete abstraction.
/// </summary>
public class InGameFieldCard : InGameCard, IInGameFieldCard
{
    private readonly FieldCard baseFieldCard;
    private readonly IFieldAbilityProvider _abilityProvider;

    public CardFamily Family { get; private set; }

    /// <summary>
    /// List of abilities associated with the field card.
    /// </summary>
    public List<FieldAbility> FieldAbilities = new List<FieldAbility>();

    /// <summary>
    /// Initializes a new instance of <see cref="InGameFieldCard"/> using the base <see cref="FieldCard"/> data.
    /// Phase 48: Added abilityProvider parameter for DI.
    /// Phase 61: Made abilityProvider required (removed fallback to legacy singleton).
    /// </summary>
    /// <param name="fieldCard">The base field card data.</param>
    /// <param name="cardOwner">The owner of the card.</param>
    /// <param name="abilityProvider">Provider for field abilities (required).</param>
    public InGameFieldCard(FieldCard fieldCard, CardOwner cardOwner, IFieldAbilityProvider abilityProvider)
    {
        baseFieldCard = fieldCard;
        CardOwner = cardOwner;
        _abilityProvider = abilityProvider;
        Reset();
    }

    /// <summary>
    /// Resets the card's properties based on the underlying base field card.
    /// </summary>
    private void Reset()
    {
        title = baseFieldCard.Title;
        Description = baseFieldCard.Description;
        BaseCard = baseFieldCard;
        DetailedDescription = baseFieldCard.DetailedDescription;
        type = baseFieldCard.Type;
        materialCard = baseFieldCard.MaterialCard;
        collector = baseFieldCard.Collector;
        Family = baseFieldCard.Family;

        // Phase 61: Use injected provider (fallback removed)
        FieldAbilities = baseFieldCard.FieldAbilities
            .Select(name => _abilityProvider.GetAbility(name))
            .Where(ability => ability != null)
            .ToList();
    }

    #region IInGameFieldCard Implementation

    /// <summary>
    /// Gets the field abilities as a read-only list of objects.
    /// Phase 49: Explicit implementation for IInGameFieldCard interface.
    /// </summary>
    IReadOnlyList<object> IInGameFieldCard.FieldAbilities =>
        FieldAbilities.Cast<object>().ToList().AsReadOnly();

    #endregion
}