/// <summary>
/// Enumerates the names of various conditions in the game.
/// </summary>
public enum ConditionName
{
    BenzaieJeuneOrBenzaieOnField,
    ArchibalVonGrenierOnField,
    ZozanKebabOnField,
    BenzaieJeuneCassetteVhsEquiped,
    JoueurDuGrenierCanarangEquiped,
    ThreeAtk3Def,
    JoueurDuGrenierOnFieldCondition,
    ForetDesElfesSylvainsOnField,
    WizardOnField,
    LyceeMagiqueGeorgesPompidouOnField,
    Developer3Atk3Def2Cards,
    HardCorner3Atk3Def2Cards,
    Japan2Cards,
    TenDeathYellowTrash,
    ComicsOnField,
    Incarnation2Cards,
    GranolaxAlreadyDead,
    HumanOnField,
    SebDuGrenierMerdePlastiqueBleuEquiped,
    SebDuGrenierOnField,
    ClicheRacisteMerdeRoseEquiped,
    MechaGronolaxOrGranolaxOnField,
    JapanOnField
}

/// <summary>
/// Represents an abstract condition that can be evaluated against player cards.
/// </summary>
public abstract class Condition
{
    /// <summary>
    /// Gets or sets the name of the condition.
    /// </summary>
    public ConditionName Name { get; set; }
    
    /// <summary>
    /// Gets or sets the description of the condition.
    /// </summary>
    protected string Description { get; set; }

    /// <summary>
    /// Determines whether the condition can be met with the given player cards.
    /// </summary>
    /// <param name="playerCards">The player cards to evaluate the condition against.</param>
    /// <returns><c>true</c> if the condition can be met; otherwise, <c>false</c>.</returns>
    public abstract bool CanBeSummoned(PlayerCards playerCards);
}
