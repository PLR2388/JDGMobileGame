

using _Scripts.Units.Invocation;

public enum EquipmentAbilityName
{
    MultiplyDefBy2ButPreventAttack
}

public abstract class EquipmentAbility
{
    public EquipmentAbilityName Name { get; set; }

    protected string Description { get; set; }

    public virtual void ApplyEffect(InGameInvocationCard invocationCard)
    {
        
    }

    public virtual void OnTurnStart(InGameInvocationCard invocationCard)
    {
        
    }

    public virtual void RemoveEffect(InGameInvocationCard invocationCard)
    {
        
    }
}
