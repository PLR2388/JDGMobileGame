namespace JDG.Domain.Enums
{
    /// <summary>
    /// Enumerates different names for equipment card abilities.
    /// Equipment abilities modify invocation card stats and behavior.
    /// </summary>
    public enum EquipmentAbilityName
    {
        MultiplyDefBy2ButPreventAttack,
        Earn1ATKAndMinus1DEF,
        DirectAttack,
        EarnOneQuarterATKPerHandCards,
        PreventNewOpponentToAttack,
        Remove1ATKAnd1DEF,
        SetATKToOne,
        CantBeAttackByOtherInvocations,
        SetDefToZero,
        MultiplyAtkBy3,
        Earn2ATK,
        Earn3ATKAndMinus1DEF,
        Earn1ATKAnd1DEF,
        MultiplyAtkBy2AndDefByHalf,
        EarnOneQuarterDEFPerHandCards,
        SwitchEquipmentCard,
        Loose2ATK,
        ProtectOneTimeFromDestruction,
        CancelInvocationAbility
    }
}
