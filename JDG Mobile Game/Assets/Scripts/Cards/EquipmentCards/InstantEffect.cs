// Equipment

namespace Cards.EquipmentCards
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
        ProtectInvocation // Destroy if opponent attack invocation instead of the invocation (TurboCradozaure)
    }
}