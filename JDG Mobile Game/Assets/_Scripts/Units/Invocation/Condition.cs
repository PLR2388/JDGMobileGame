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

public abstract class Condition
{
    public ConditionName Name { get; set; }
    protected string Description { get; set; }

    public abstract bool CanBeSummoned(PlayerCards playerCards, PlayerCards opponentPlayerCards);
}
