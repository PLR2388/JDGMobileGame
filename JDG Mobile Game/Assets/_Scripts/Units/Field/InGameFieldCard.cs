using System.Collections.Generic;
using System.Linq;
using Cards;
using Cards.FieldCards;

/// <summary>
/// Represents a card on the field in the game with additional runtime behaviors.
/// </summary>
public class InGameFieldCard : InGameCard
{
    private readonly FieldCard baseFieldCard;

    public CardFamily Family { get; private set; }

    /// <summary>
    /// List of abilities associated with the field card.
    /// </summary>
    public List<FieldAbility> FieldAbilities = new List<FieldAbility>();

    /// <summary>
    /// Initializes a new instance of <see cref="InGameFieldCard"/> using the base <see cref="FieldCard"/> data.
    /// </summary>
    /// <param name="fieldCard">The base field card data.</param>
    /// <param name="cardOwner">The owner of the card.</param>
    /// <returns>An instance of <see cref="InGameFieldCard"/>.</returns>
    public InGameFieldCard(FieldCard fieldCard, CardOwner cardOwner)
    {
        baseFieldCard = fieldCard;
        CardOwner = cardOwner;
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
        FieldAbilities = baseFieldCard.FieldAbilities.Select(
            fieldAbilityName => FieldAbilityLibrary.Instance.fieldAbilityDictionary[fieldAbilityName]
        ).ToList();
    }
}