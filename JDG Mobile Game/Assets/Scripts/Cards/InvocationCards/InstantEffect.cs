// Equipment
namespace Cards.InvocationCards
{
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
}