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