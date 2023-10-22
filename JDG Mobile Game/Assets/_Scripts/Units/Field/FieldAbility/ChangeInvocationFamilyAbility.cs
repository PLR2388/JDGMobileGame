using System.Linq;
using _Scripts.Units.Invocation;
using Cards;

/// <summary>
/// Represents a field ability that can change the family of a specific invocation card.
/// </summary>
public class ChangeInvocationFamilyAbility : FieldAbility
{
    private readonly string invocationName;
    private readonly CardFamily family;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChangeInvocationFamilyAbility"/> class.
    /// </summary>
    /// <param name="name">The name of the field ability.</param>
    /// <param name="description">The description of the field ability.</param>
    /// <param name="invocationName">The name of the invocation card to be affected.</param>
    /// <param name="family">The card family to be set.</param>
    public ChangeInvocationFamilyAbility(FieldAbilityName name, string description, string invocationName,
        CardFamily family)
    {
        Name = name;
        Description = description;
        this.invocationName = invocationName;
        this.family = family;
    }

    /// <summary>
    /// Notifies other field abilities about the change in family for an invocation card.
    /// </summary>
    /// <param name="playerCards">The player's cards context.</param>
    /// <param name="previousFamilies">Previous families of the invocation card.</param>
    /// <param name="invocationCard">The invocation card that has its family changed.</param>
    private static void NotifyChangeFamilyFieldCard(PlayerCards playerCards, CardFamily[] previousFamilies,
        InGameInvocationCard invocationCard)
    {
        if (playerCards.FieldCard == null) return;
        foreach (var fieldCardFieldAbility in playerCards.FieldCard.FieldAbilities)
        {
            fieldCardFieldAbility.OnInvocationChangeFamily(previousFamilies, invocationCard);
        }
    }

    /// <summary>
    /// Applies the effect, changing the family of the specified invocation card.
    /// </summary>
    /// <param name="playerCards">The player's cards context.</param>
    public override void ApplyEffect(PlayerCards playerCards)
    {
        base.ApplyEffect(playerCards);
        var invocationCard = playerCards.InvocationCards.FirstOrDefault(card => card.Title == invocationName);
        if (invocationCard != null)
        {
            var previousFamilies = invocationCard.Families;
            invocationCard.Families = new[] { family };

            NotifyChangeFamilyFieldCard(playerCards, previousFamilies, invocationCard);
        }
    }

    /// <summary>
    /// Handles the event when a new invocation card is added to the player's context.
    /// </summary>
    /// <param name="invocationCard">The added invocation card.</param>
    /// <param name="playerCards">The player's cards context.</param>
    public override void OnInvocationCardAdded(InGameInvocationCard invocationCard, PlayerCards playerCards)
    {
        base.OnInvocationCardAdded(invocationCard, playerCards);
        if (invocationCard.Title == invocationName)
        {
            var previousFamilies = invocationCard.Families;
            invocationCard.Families = new[] { family };
            NotifyChangeFamilyFieldCard(playerCards, previousFamilies, invocationCard);
        }
    }

    /// <summary>
    /// Handles the event when a field card is removed from the player's context.
    /// </summary>
    /// <param name="playerCards">The player's cards context.</param>
    public override void OnFieldCardRemoved(PlayerCards playerCards)
    {
        base.OnFieldCardRemoved(playerCards);
        var invocationCard = playerCards.InvocationCards.FirstOrDefault(card => card.Title == invocationName);
        if (invocationCard != null)
        {
            var previousFamilies = invocationCard.Families;
            invocationCard.Families = invocationCard.BaseInvocationCard.BaseInvocationCardStats.Families;
            NotifyChangeFamilyFieldCard(playerCards, previousFamilies, invocationCard);
        }
    }
}