// Executed when player put invocation card on field
namespace Cards.InvocationCards
{
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
        Condition,
        SacrificeFieldIncrement
    }
}