namespace JDG.Domain.Enums
{
    /// <summary>
    /// Enumeration of names for various effect card abilities.
    /// Effect abilities trigger one-time or turn-based effects.
    /// </summary>
    public enum EffectAbilityName
    {
        LimitHandCardTo5,
        Lose2Point5StarsByInvocations,
        ApplyFamilyFieldToInvocations,
        DestroyAllCardsUnderManyConditions,
        GetHPFor1Sacrifice3ATKDEFCondition,
        DirectAttackIfUnder5HP,
        ChangeFieldCardFromDeck,
        DestroyOneCardByRemovingOneHandCard,
        DestroyFieldFor7HalfCost,
        Get7HalfHPFor1Sacrifice,
        GetCardFromYellowDeck,
        ManiabilitePourrieSkipAttackForOpponent,
        SwitchAtkDef,
        LookAndOrderDeckCards,
        LooseHPBasedOnNumberInvocation,
        DestroyEquipmentCard,
        LookOpponentHandCardsAndChangeIt,
        DoubleAttackPerTurn,
        InvokeCardFromYellowTrash,
        DivideDEFOpponentBy2,
        Add3ShieldsForUser,
        DestroyOpponentInvocationCard,
        Loose1HPPerOpponentHandCards,
        GetBackAllHPBySacrifice5AtkDef,
        Control1OpponentInvocationCard
    }
}
