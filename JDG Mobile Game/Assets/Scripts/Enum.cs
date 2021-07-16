public enum CardType
{
    Contre,
    Effect,
    Equipment,
    Field,
    Invocation
}

public enum CardFamily
{
    Comics,
    Developer,
    Fistiland,
    HardCorner,
    Human,
    Incarnation,
    Japan,
    Monster,
    Police,
    Rpg,
    Spatial,
    Wizard
}

public enum Condition
{
    SpecificCardOnField,
    SacrificeSpecificCard,
    SpecificEquipmentAttached,
    SpecificField,
    SacrificeFamily,
    SpecificFamilyOnField,
    NumberCard,
    SacrificeThresholdAtk,
    SacrificeThresholdDef,
    NumberInvocationCardInYellowTrash,
    ComeFromYellowTrash //true or false
}

public enum StartEffect
{
    GetSpecificCard,
    GetSpecificTypeCard,
    GetCardFamily,
    GetCardSource,
    RemoveAllInvocationCards,
    InvokeSpecificCard,
    PutField,
    DestroyField,
    Divide2Atk,
    Divide2Def,
    SendToDeath,
    DrawXCards,
    Condition
}

public enum PermEffect
{
    CanOnlyAttackIt,
    GiveStat,
    IncreaseAtk,
    IncreaseDef, //Increase ATK or DEF depending of the number of same family cards on field
    Family,
    PreventInvocationCards,
    ProtectBehind,
    ImpossibleAttackByInvocation,
    ImpossibleToBeAffectedByEffect,
    Condition,
    NumberTurn
}

public enum ActionEffect
{
    SacrificeInvocation,
    SpecificField,
    IncreaseStarsAtkAndDef,
    GiveAtk,
    GiveDef,
    SpecificFamily,
    BackToLife,
    SkipOpponentAttack
}

public enum DeathEffect
{
    ComeBackToHand,
    GetSpecificCard,
    GetCardSource,
    KillAlsoOtherCard
}

// Equipment
public enum InstantEffect //Happen only at the beginning
{
    AddDef,
    AddAtk,
    MultiplyAtk,
    MultiplyDef,
    SetAtk,
    SetDef,
    BlockAtk,
    DirectAtk, // Direct attack opponent stars
    SwitchEquipment, // Change previous equipmentCard by this one
    DisableBonus, // Remove native card bonus
}

public enum PermanentEffect //Must be frequently change
{
    AddAtkBaseOnHandCards,
    AddDefBaseOnHandCards,
    BlockOpponentDuringInvocation, // Prevent opponent's attack during invocation
    PreventAttackOnInvocation, // Prevent opponent to attack this card
}

// Effect
public enum Effect
{
    AffectPv, // Indicate unit numberofPv
    AffectOpponent,
    NumberInvocationCard,
    NumberHandCard,
    DestroyCards,
    SacrificeInvocation,
    SameFamily,
    CheckTurn,
    ChangeHandCards,
    Sources,
    HandMax,
    SeeOpponentHand,
    RemoveCardOption,
    RemoveHand,
    RemoveDeck,
    SpecialInvocation,
    DivideInvocation,
    Duration,
    Combine,
    RevertStat,
    TakeControl,
    NumberAttacks,
    SkipAttack,
    SeeCards,
    ChangeOrder,
    AttackDirectly, // if PV less than or equal to this number, can attack directly
    ProtectAttack,
    SkipFieldsEffect,
    ChangeField,
    SkipContre
}