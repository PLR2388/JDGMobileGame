using System;
using _Scripts.Units.Invocation;
using Cards;
using Cards.EffectCards;
using Cards.EquipmentCards;
using Cards.FieldCards;
using Cards.InvocationCards;

/// <summary>
/// Provides functionality to create specific instances of InGameCard based on the provided card type.
/// </summary>
public class CardFactory
{
    /// <summary>
    /// Creates an instance of InGameCard based on the type and owner of the provided card.
    /// </summary>
    /// <param name="card">The base card for which the InGameCard is to be created.</param>
    /// <param name="cardOwner">The owner of the card.</param>
    /// <returns>An instance of a specific InGameCard subtype based on the card provided.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an unsupported card type is provided.</exception>
    public static InGameCard CreateInGameCard(Card card, CardOwner cardOwner)
    {
        return card switch
        {
            EquipmentCard equipmentCard => InGameEquipmentCard.Init(equipmentCard, cardOwner),
            EffectCard effectCard => InGameEffectCard.Init(effectCard, cardOwner),
            InvocationCard invocationCard => InGameInvocationCard.Init(invocationCard, cardOwner),
            FieldCard fieldCard => InGameFieldCard.Init(fieldCard, cardOwner),
            _ => throw new InvalidOperationException("Unsupported card type provided.")
        };
    }
}