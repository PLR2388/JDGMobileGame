/// <summary>
/// Represents a condition that checks whether a specific field card, identified by its name,
/// is present on the field to determine if a card can be summoned.
/// </summary>
public class FieldCardOnFieldCondition : Condition
{
    
    /// <summary>
    /// The name of the field card to check for its presence on the field.
    /// </summary>
    private readonly string fieldName;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="FieldCardOnFieldCondition"/> class.
    /// </summary>
    /// <param name="name">The unique name identifier of the condition.</param>
    /// <param name="description">A description that explains the condition.</param>
    /// <param name="fieldName">The name of the field card to be checked for its presence on the field.</param>
    public FieldCardOnFieldCondition(ConditionName name, string description, string fieldName)
    {
        Name = name;
        Description = description;
        this.fieldName = fieldName;
    }

    /// <summary>
    /// Evaluates if a card can be summoned based on the presence of a specific field card on the field.
    /// </summary>
    /// <param name="playerCards">A collection of player cards to evaluate the condition against.</param>
    /// <returns><c>true</c> if the specified field card is on the field; otherwise, <c>false</c>.</returns>
    public override bool CanBeSummoned(PlayerCards playerCards)
    {
        return playerCards.FieldCard != null && playerCards.FieldCard.Title == fieldName;
    }
}
