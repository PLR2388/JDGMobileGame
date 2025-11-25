namespace JDG.Domain.Enums
{
    /// <summary>
    /// Enumeration representing different types of field card abilities.
    /// Field abilities provide passive bonuses to cards on the field.
    /// </summary>
    public enum FieldAbilityName
    {
        Earn1DEFForSpatialFamily,
        Earn1HalfDEFAndMinusHalfATKForDevFamily,
        ChangePatronInfogramFamilyToDev,
        ChangeJMBruitagesFamilyToDev,
        Earn2DEFAndMinusOneATKForIncarnationFamily,
        EarnHalfHPPerWizardInvocationEachTurn,
        Earn1ATKForJapanFamily,
        Earn1HalfATKAndMinusHalfDEFForHCFamily,
        DrawOneMoreCard,
        EarnHalfATKAndDefForRpgFamily,
        SkipDrawToGetFistilandInvocation,
        Earn2ATKAndMinus1DEFForComicsFamily
    }
}
